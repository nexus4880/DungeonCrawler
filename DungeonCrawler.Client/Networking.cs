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
		Networking.Subscribe<InitializeWorldPacket>(Networking.OnInitializeWorld);
		Networking.Subscribe<EntityMovedPacket>(Networking.OnEntityMoved);
		Networking.Subscribe<EntityCreatePacket>(OnEntityCreated);
		Networking.Subscribe<EntityDestroyPacket>(OnEntityDestroyed);
		Networking.Subscribe<InitializeAssetsPacket>(Networking.OnInitializeAssets);
		Networking.EventBasedNetListener = new EventBasedNetListener();
		Networking.EventBasedNetListener.NetworkReceiveEvent += Networking.OnNetworkReceive;
		Networking.NetManager = new NetManager(Networking.EventBasedNetListener);
	}

	private static void OnInitializeAssets(InitializeAssetsPacket packet, UserPacketEventArgs args){
		if(Directory.Exists("assets")){
			Directory.Delete("assets",true);
		}

		using MemoryStream sm = new MemoryStream(args.PacketReader.GetBytesWithLength());
		using ZipArchive zip = new ZipArchive(sm,ZipArchiveMode.Read);
		currentVFS = VFS.FromArchive(zip);
		PacketProcessor.Write(Writer,new AssetsLoadedPacket());
	}

	private static void OnEntityDestroyed(EntityDestroyPacket packet, UserPacketEventArgs args){
		GameManager.RemoveEntity(packet.EntityId);
	}	

	private static void OnEntityCreated(EntityCreatePacket packet, UserPacketEventArgs args){
		System.Console.WriteLine("Got");
		var entity = args.PacketReader.GetDeserializable<Entity>();
		
		if(entity == null){
			throw new Exception("Failed to make entity");
		}

		entity.AddComponent<CircleRenderer>();

		GameManager.AddEntity(entity);
	}

	private static void OnInitializeWorld(InitializeWorldPacket packet, UserPacketEventArgs args)
	{
		Console.WriteLine($"[OnInitializeWorld] {packet.EntitiesCount} entities | {packet.LootItemsCount} loot items");
		receievedGameState = true;
		for (Int32 i = 0; i < packet.EntitiesCount; i++)
		{
			Entity entity = args.PacketReader.GetDeserializable<Entity>();
			if (entity.EntityId == packet.LocalPlayerEntityId)
			{
				GameManager.localPlayer = (PlayerEntity)entity;
				GameManager.localPlayer.NetPeer = args.Peer;
			}

			entity.AddComponent<TextureRenderer>("assets/textures/checkmark.png");
			GameManager.AddEntity(entity);
		}

		for (Int32 i = 0; i < packet.LootItemsCount; i++)
		{
			DroppedLootItem lootItem = args.PacketReader.GetDeserializable<DroppedLootItem>();
			GameManager.AddLootItem(lootItem);
		}

	}

	private static void OnEntityMoved(EntityMovedPacket packet, UserPacketEventArgs args)
	{
		if (GameManager.GetEntityByID(packet.EntityId) == null)
		{
			return;
		}

		GameManager.GetEntityByID(packet.EntityId).Position = packet.Position;
	}

	public static void Update()
	{
		Networking.NetManager.PollEvents();
		if (Networking.Writer.Length > 0)
		{
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
		Console.WriteLine("[OnNetworkReceive]");
		try
		{
			Networking.PacketProcessor.ReadAllPackets(reader,
				new UserPacketEventArgs(peer, reader, channel, deliverymethod));
		}
		catch (ParseException)
		{
		}
	}
}