using System.IO.Compression;
using System.Net;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server;

public static class GameServer
{
	public static EventBasedNetListener EventBasedNetListener { get; } = new EventBasedNetListener();
	public static NetManager NetManager { get; private set; }
	public static NetPacketProcessor PacketProcessor { get; } = new NetPacketProcessor();
	private static Byte[] _assetsBuffer;

	public static void Initialize(IPAddress ipv4, IPAddress ipv6, Int32 port)
	{
		Console.WriteLine("Zipping assets...");
		using MemoryStream memoryStream = new MemoryStream();
		ZipFile.CreateFromDirectory("./assets/", memoryStream, CompressionLevel.SmallestSize, true);
		_assetsBuffer = memoryStream.ToArray();

		Console.WriteLine("Initializing server...");
		GameServer.PacketProcessor.Initialize();
		GameServer.EventBasedNetListener.NetworkReceiveEvent += GameServer.OnNetworkReceive;
		GameServer.EventBasedNetListener.ConnectionRequestEvent += GameServer.OnConnectionRequest;
		GameServer.EventBasedNetListener.PeerDisconnectedEvent += GameServer.OnPeerDisconnected;
		SubscribePacket<SetInputsPacket>(GameServer.OnSetInputsPacket);
		SubscribePacket<AssetsLoadedPacket>(GameServer.OnAssetsLoadedPacket);
		GameServer.NetManager = new NetManager(GameServer.EventBasedNetListener);
		String ip = $"{ipv4}:{port}";
		if (!GameServer.NetManager.Start(ipv4, ipv6, port))
		{
			throw new Exception($"Failed to start server on {ip}");
		}

		Console.WriteLine($"Started server on {ip}");

		Guid spawnId = Guid.NewGuid();
		Item item = ItemManager.CreateItem<InstantHealthPotion>(369f);
		DroppedLootItem droppedLootItem = GameManager.CreateEntity<DroppedLootItem>(spawnId, item, "assets/textures/heart-bottle.png");
		droppedLootItem.Position = new System.Numerics.Vector2(300f, 300f);
	}

	public static void SubscribePacket<T>(Action<T, UserPacketEventArgs> callback) where T : class, new()
	{
		GameServer.PacketProcessor.SubscribeReusable<T, UserPacketEventArgs>(callback);
	}

	private static void OnSetInputsPacket(SetInputsPacket packet, UserPacketEventArgs args)
	{
		PlayerEntity player = GameManager.GetEntities<PlayerEntity>().FirstOrDefault(player => player.NetPeer.Id == args.Peer.Id);
		if (player is null)
		{
			Console.WriteLine($"[OnSetInputsPacket] no peer {args.Peer.Id}");

			return;
		}

		player.CurrentInputs = packet.Inputs;
	}

	private static void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo)
	{
		PlayerEntity playerEntity = GameManager.GetEntities<PlayerEntity>().FirstOrDefault(player => player.NetPeer.Id == peer.Id);
		if (playerEntity is not null)
		{
			if (GameManager.DestroyEntity(playerEntity.EntityId))
			{
				Console.WriteLine($"[OnPeerDisconnected] {playerEntity.EntityId} ({peer.Id}) disconnected");
			}
			else
			{
				Console.WriteLine($"[OnPeerDisconnected] failed to remove {playerEntity.EntityId} ({peer.Id}) from GameManager");
			}
		}
		else
		{
			Console.WriteLine($"[OnPeerDisconnected] failed to get PlayerEntity with PeerId {peer.Id}");
		}
	}

	private static void OnAssetsLoadedPacket(AssetsLoadedPacket packet, UserPacketEventArgs args)
	{
		Console.WriteLine("[OnAssetsLoadedPacket]");
		PlayerEntity thisPlayer = GameManager.CreateEntity<PlayerEntity>(args.Peer);
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new EntityCreatePacket());
		writer.PutDeserializable(thisPlayer);
		GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, args.Peer);

		Dictionary<Guid, Entity>.ValueCollection entities = GameManager.EntityList.Values;
		Dictionary<Guid, DroppedLootItem>.ValueCollection lootItems = GameManager.LootItems.Values;
		writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new InitializeWorldPacket
		{
			EntitiesCount = entities.Count,
			LootItemsCount = lootItems.Count,
			LocalPlayerEntityId = thisPlayer.EntityId
		});

		foreach (Entity entity in entities)
		{
			writer.PutDeserializable(entity);
		}

		foreach (DroppedLootItem lootItem in lootItems)
		{
			writer.PutDeserializable(lootItem);
		}

		args.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnConnectionRequest(ConnectionRequest request)
	{
		NetPeer peer = request.AcceptIfKey("DungeonCrawler");
		if (peer is null)
		{
			Console.WriteLine($"[OnConnectionRequest] invalid key");

			return;
		}

		Console.WriteLine($"[OnConnectionRequest] connection from {peer.EndPoint} accepted as ID {peer.Id}, sending {_assetsBuffer.Length} bytes worth of assets");
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new InitializeAssetsPacket { });
		writer.Put((Int32)_assetsBuffer.Length);
		writer.Put(_assetsBuffer);
		peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliveryMethod)
	{
		try
		{
			GameServer.PacketProcessor.ReadAllPackets(reader, new UserPacketEventArgs(peer, reader, channel, deliveryMethod));
		}
		catch (ParseException)
		{
			Console.WriteLine($"[OnNetworkReceive] {peer.EndPoint} ({peer.Id}) sent a packet we don't understand");
		}
	}

	public static void Update(Single deltaTime)
	{
		GameServer.NetManager.PollEvents();
		GameManager.Update(deltaTime);
	}

	public static void Shutdown()
	{
		GameServer.NetManager.Stop(true);
	}
}