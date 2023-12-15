using System.Runtime.InteropServices;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;
using DungeonCrawler.Client.Renderers;
using System.IO.Compression;
using System.Collections.Specialized;
using System.Collections;

namespace DungeonCrawler.Client;

public static class Networking
{
	public static NetPacketProcessor PacketProcessor { get; private set; }
	public static NetManager NetManager { get; private set; }
	public static EventBasedNetListener EventBasedNetListener { get; private set; }
	public static NetPeer LocalPeer { get; set; }
	public static NetDataWriter Writer { get; } = new NetDataWriter(true, UInt16.MaxValue);
	public static bool receievedGameState = false;
	public static VFS currentVFS;

	public static void Initialize()
	{
		Networking.PacketProcessor = new NetPacketProcessor();
		Networking.PacketProcessor.Initialize();
		Networking.Subscribe<InitializeAssetsPacket>(Networking.OnInitializeAssets);
		Networking.Subscribe<InitializeWorldPacket>(Networking.OnInitializeWorld);
		Networking.Subscribe<EntityMovedPacket>(Networking.OnEntityMoved);
		Networking.Subscribe<EntityCreatePacket>(Networking.OnEntityCreated);
		Networking.Subscribe<EntityDestroyPacket>(Networking.OnEntityDestroyed);
		Networking.Subscribe<SetEntityContextPacket>(Networking.SetEntityContext);
		Networking.EventBasedNetListener = new EventBasedNetListener();
		Networking.EventBasedNetListener.NetworkReceiveEvent += Networking.OnNetworkReceive;
		Networking.NetManager = new NetManager(Networking.EventBasedNetListener);
	}

	private static void OnInitializeAssets(InitializeAssetsPacket packet, UserPacketEventArgs args)
	{
		Int32 assetsBufferSize = args.PacketReader.GetInt();
		Console.WriteLine($"[OnInitializeAssets] assets buffer size: {assetsBufferSize}");
		Byte[] buffer = new byte[assetsBufferSize];
		args.PacketReader.GetBytes(buffer, buffer.Length);
		using MemoryStream sm = new MemoryStream(buffer);
		using ZipArchive zip = new ZipArchive(sm, ZipArchiveMode.Read);
		currentVFS = VFS.FromArchive(zip);
		PacketProcessor.Write(Writer, new AssetsLoadedPacket());
	}

	private static void OnEntityDestroyed(EntityDestroyPacket packet, UserPacketEventArgs args)
	{
		Console.WriteLine($"[OnEntityDestroyed] entity {packet.EntityId} destroyed");
		GameManager.RemoveEntity(packet.EntityId);
	}

	private static void SetEntityContext(SetEntityContextPacket packet, UserPacketEventArgs args)
	{
		if (packet.EntityId != Guid.Empty)
		{
			Entity entity = GameManager.GetEntityByID(packet.EntityId);
			switch (entity)
			{
				case PlayerEntity playerEntity:
					{
						Console.WriteLine($"[SetEntityContext] set entity context to {packet.EntityId}");
						GameManager.localPlayer = playerEntity;

						break;
					}
				case null:
					{
						Console.WriteLine($"[SetEntityContext] {packet.EntityId} is not an entity");

						break;
					}
				default:
					{
						Console.WriteLine($"[SetEntityContext] {packet.EntityId} is not a PlayerEntity");

						break;
					}
			}
		}
		else
		{
			GameManager.localPlayer = null;
		}
	}

	private static void OnEntityCreated(EntityCreatePacket packet, UserPacketEventArgs args)
	{
		try
		{
			Entity entity = args.PacketReader.GetDeserializable<Entity>();
			Console.WriteLine($"[OnEntityCreated] {entity.EntityId} was created at {entity.Position} of type '{entity.GetType()}'");
			GameManager.AddEntity(entity);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[OnEntityCreated] failed to create entity: '{ex.Message}'");
		}
	}

	private static void OnInitializeWorld(InitializeWorldPacket packet, UserPacketEventArgs args)
	{
		Console.WriteLine($"[OnInitializeWorld] {packet.EntitiesCount} entities");
		receievedGameState = true;
		for (Int32 i = 0; i < packet.EntitiesCount; i++)
		{
			Entity entity = args.PacketReader.GetDeserializable<Entity>();
			if (entity.EntityId == packet.LocalPlayerEntityId)
			{
				GameManager.localPlayer = (PlayerEntity)entity;
				GameManager.localPlayer.NetPeer = args.Peer;
			}

			GameManager.AddEntity(entity);
		}

		GameManager.tiles = new ClientBaseTile[packet.WorldWidth, packet.WorldHeight];
		GameManager.tileSize = (packet.TileWidth, packet.TileHeight);
		for (Int32 y = 0; y < packet.WorldHeight; y++)
		{
			for (Int32 x = 0; x < packet.WorldWidth; x++)
			{
				ClientBaseTile tile = new ClientBaseTile { X = x, Y = y };
				GameManager.tiles[x, y] = tile;
				tile.Deserialize(args.PacketReader);
				tile.Initialize();
			}
		}

		Networking.PacketProcessor.Write(Networking.Writer, new WorldLoadedPacket { });
	}

	private static void OnEntityMoved(EntityMovedPacket packet, UserPacketEventArgs args)
	{
		Entity entity = GameManager.GetEntityByID(packet.EntityId);
		if (entity is null)
		{
			Console.WriteLine($"[OnEntityMoved] {packet.EntityId} doesn't exist on client");

			return;
		}

		entity.Position = packet.Position;
	}

	public static void Update()
	{
		Networking.NetManager.PollEvents();
		if (Networking.Writer.Length > 0)
		{
			Console.WriteLine($"Sending {Networking.Writer.Length} bytes");
			Networking.LocalPeer.Send(Networking.Writer, DeliveryMethod.Unreliable);
			Networking.Writer.Reset();
		}
	}

	public static void Subscribe<T>(Action<T, UserPacketEventArgs> onReceive) where T : class, new()
	{
		Networking.PacketProcessor.SubscribeReusable(onReceive);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliverymethod)
	{
		try
		{
			Networking.PacketProcessor.ReadAllPackets(reader,
				new UserPacketEventArgs(peer, reader, channel, deliverymethod));
		}
		catch (ParseException)
		{
			Console.WriteLine("[OnNetworkReceive] failed to parse packet");
		}
	}
}