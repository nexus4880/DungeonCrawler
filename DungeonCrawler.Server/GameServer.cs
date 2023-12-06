using System.Net;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
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

	public static void Initialize(IPAddress ipv4, IPAddress ipv6, Int32 port)
	{
		GameServer.PacketProcessor.Initialize();
		GameServer.EventBasedNetListener.NetworkReceiveEvent += GameServer.OnNetworkReceive;
		GameServer.EventBasedNetListener.ConnectionRequestEvent += GameServer.OnConnectionRequest;
		GameServer.EventBasedNetListener.PeerConnectedEvent += GameServer.OnPeerConnected;
		GameServer.EventBasedNetListener.PeerDisconnectedEvent += GameServer.OnPeerDisonnected;
		GameServer.NetManager = new NetManager(GameServer.EventBasedNetListener);
		String ip = $"{ipv4}:{port}";
		if (!GameServer.NetManager.Start(ipv4, ipv6, port))
		{
			throw new Exception($"Failed to start server on {ip}");
		}

		Console.WriteLine($"Started server on {ip}");
	}

	private static void OnPeerDisonnected(NetPeer peer, DisconnectInfo disconnectinfo)
	{
		Console.WriteLine("[OnPeerDisonnected]");
		PlayerEntity playerEntity = GameManager.GetEntities<PlayerEntity>().FirstOrDefault(player => player.NetPeer.Id == peer.Id);
		if (playerEntity is null)
		{
			return;
		}

		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new EntityDestroyPacket { EntityId = playerEntity.EntityId });
		GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnPeerConnected(NetPeer peer)
	{
		Console.WriteLine("[OnPeerConnected]");
		PlayerEntity thisPlayer = GameManager.CreateEntity<PlayerEntity>(peer);
		NetDataWriter writer = new NetDataWriter();
		EntityCreatePacket entityCreatePacket = new EntityCreatePacket();
		GameServer.PacketProcessor.Write(writer, entityCreatePacket);
		writer.Put(thisPlayer);
	}

	private static void OnConnectionRequest(ConnectionRequest request)
	{
		Console.WriteLine("[OnConnectionRequest]");
		NetPeer peer = request.AcceptIfKey("DungeonCrawler");
		if (peer is null)
		{
			return;
		}

		Dictionary<Guid, Entity>.ValueCollection entities = GameManager.EntityList.Values;
		Dictionary<Guid, DroppedLootItem>.ValueCollection lootItems = GameManager.LootItems.Values;
		InitializeWorldPacket initializeWorldPacket = new InitializeWorldPacket { EntitiesCount = entities.Count, LootItemsCount = lootItems.Count };
		NetDataWriter writer = new NetDataWriter();
		foreach (Entity entity in entities)
		{
			writer.Put(entity);
		}

		foreach (DroppedLootItem lootItem in lootItems)
		{
			writer.Put(lootItem);
		}

		GameServer.PacketProcessor.Write(writer, initializeWorldPacket);
		peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliveryMethod)
	{
		try
		{
			GameServer.PacketProcessor.ReadAllPackets(reader);
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