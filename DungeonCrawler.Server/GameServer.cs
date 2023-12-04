using System.Net;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Entities;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server;

public static class GameServer {
	public static EventBasedNetListener EventBasedNetListener { get; } = new EventBasedNetListener();
	public static NetManager NetManager { get; private set; }
	public static NetPacketProcessor PacketProcessor { get; } = new NetPacketProcessor();

	public static void Initialize(IPAddress ipv4, IPAddress ipv6, Int32 port) {
		GameServer.PacketProcessor.Initialize();
		GameServer.EventBasedNetListener.NetworkReceiveEvent += GameServer.OnNetworkReceive;
		GameServer.EventBasedNetListener.ConnectionRequestEvent += GameServer.OnConnectionRequest;
		GameServer.EventBasedNetListener.PeerConnectedEvent += GameServer.OnPeerConnected;
		GameServer.EventBasedNetListener.PeerDisconnectedEvent += GameServer.OnPeerDisonnected;
		GameServer.NetManager = new NetManager(GameServer.EventBasedNetListener);
		String ip = $"{ipv4}:{port}";
		if (!GameServer.NetManager.Start(ipv4, ipv6, port)) {
			throw new Exception($"Failed to start server on {ip}");
		}

		Console.WriteLine($"Started server on {ip}");
	}

	private static void OnPeerDisonnected(NetPeer peer, DisconnectInfo disconnectinfo) {
		Console.WriteLine("[OnPeerDisonnected]");
	}

	private static void OnPeerConnected(NetPeer peer) {
		Console.WriteLine("[OnPeerConnected]");
	}

	private static void OnConnectionRequest(ConnectionRequest request) {
		Console.WriteLine("[OnConnectionRequest]");
		NetPeer peer = request.AcceptIfKey("DungeonCrawler");
		if (peer is null) {
			return;
		}

		InitializeWorldPacket initializeWorldPacket = new InitializeWorldPacket();
		NetDataWriter writer = new NetDataWriter();
		Dictionary<Guid, Entity>.ValueCollection entities = GameManager.EntityList.Values;
		writer.Put(entities.Count);
		foreach (Entity entity in entities) {
			writer.Put(entity);
		}

		Dictionary<Guid, DroppedLootItem>.ValueCollection lootItems = GameManager.LootItems.Values;
		writer.Put(lootItems.Count);
		foreach (DroppedLootItem lootItem in lootItems) {
			writer.Put(lootItem);
		}

		GameServer.PacketProcessor.Write(writer, initializeWorldPacket);
		peer.Send(writer, DeliveryMethod.ReliableOrdered);

		GameManager.CreateEntity<PlayerEntity>(peer);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliveryMethod) {
		try {
			GameServer.PacketProcessor.ReadAllPackets(reader);
		}
		catch (ParseException) {
			Console.WriteLine($"[OnNetworkReceive] {peer.EndPoint} ({peer.Id}) sent a packet we don't understand");
		}
	}

	public static void Update(Single deltaTime) {
		GameServer.NetManager.PollEvents();
		GameManager.Update(deltaTime);
	}

	public static void Shutdown() {
		GameServer.NetManager.Stop(true);
	}
}