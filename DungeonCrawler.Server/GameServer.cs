using System.Net;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server;

public class GameServer {
	public GameServer(IPAddress ipv4, IPAddress ipv6, Int32 port) {
		this.PacketProcessor.Initialize();
		this.EventBasedNetListener.ConnectionRequestEvent += this.OnConnectionRequest;
		this.EventBasedNetListener.PeerConnectedEvent += this.OnPeerConnected;
		this.EventBasedNetListener.PeerDisconnectedEvent += this.OnPeerDisconnected;
		this.EventBasedNetListener.NetworkReceiveEvent += this.OnNetworkReceive;
		this.PacketProcessor.SubscribeReusable<SetInputsPacket, NetPeer>(this.OnPlayerSetInputsPacket);
		this.PacketProcessor.SubscribeReusable<UseItemPacket, NetPeer>(this.OnUseItemPacket);
		this.NetManager = new NetManager(this.EventBasedNetListener);
		String ip = $"{ipv4}:{port}";
		if (!this.NetManager.Start(ipv4, ipv6, port)) {
			throw new Exception($"Failed to start server on {ip}");
		}

		Console.WriteLine($"Started server on {ip}");
		this.GameManager = new GameManager(this);
	}

	public EventBasedNetListener EventBasedNetListener { get; } = new EventBasedNetListener();
	public NetManager NetManager { get; }
	public NetPacketProcessor PacketProcessor { get; } = new NetPacketProcessor();
	public GameManager GameManager { get; }

	private void OnUseItemPacket(UseItemPacket packet, NetPeer peer) {
		Console.WriteLine($"[OnUseItemPacket] {peer.Id} is using item {packet.ItemId}");
		PlayerController controller = this.GameManager.GetControllerById(peer.Id);
		if (controller is null) {
			Console.WriteLine("[OnUseItemPacket] failed to find PlayerController");

			return;
		}

		try {
			controller.UseItem(packet.ItemId);
		}
		catch (Exception ex) {
			Console.WriteLine($"[OnUseItemPacket] {ex.Message}");
		}
	}

	private void OnPlayerSetInputsPacket(SetInputsPacket packet, NetPeer peer) {
		PlayerController controller = this.GameManager.GetControllerById(peer.Id);
		if (controller is null) {
			return;
		}

		controller.inputs = packet.Inputs;
	}

	private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel, DeliveryMethod deliverymethod) {
		this.PacketProcessor.ReadAllPackets(reader, peer);
	}

	private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo) {
		if (!this.GameManager.RemovePlayer(peer.Id)) {
			Console.WriteLine($"[OnPeerDisconnected] Failed to remove player {peer.Id}");

			return;
		}

		NetDataWriter writer = new NetDataWriter();
		this.PacketProcessor.Write(writer, new PlayerDisconnectedPacket { Id = peer.Id });
		this.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		Console.WriteLine($"[OnPeerDisconnected] {peer.Id} has disconnected");
	}

	public void Update(Single deltaTime) {
		this.NetManager.PollEvents();
		this.GameManager.Update(deltaTime);
	}


	private void OnPeerConnected(NetPeer peer) {
		PlayerController controller = this.GameManager.GetControllerById(peer.Id);
		if (controller is null) {
			Console.WriteLine($"[OnPeerConnected] failed to find PlayerController for {peer.Id}");
			peer.Disconnect();

			return;
		}

		NetDataWriter writer = new NetDataWriter();
		InitializeWorldPacket packet = new InitializeWorldPacket
			{ Players = this.GameManager.AllPlayers.Select(p => p.Value.GetPlayerData()).ToArray() };
		this.PacketProcessor.Write(writer, packet);
		Console.WriteLine(packet.Players[0]);
		peer.Send(writer, DeliveryMethod.ReliableOrdered);
		Console.WriteLine($"[OnPeerConnected] {peer.Id} has connected from {peer.EndPoint}");
	}

	private void OnConnectionRequest(ConnectionRequest request) {
		NetPeer peer = request.AcceptIfKey("DungeonCrawler");
		if (peer is null) {
			Console.WriteLine("[OnConnectionRequest] key is not 'DungeonCrawler'");

			return;
		}

		PlayerController controller = this.GameManager.CreatePlayer(peer);
		NetDataWriter writer = new NetDataWriter();
		this.PacketProcessor.Write(writer, new PlayerJoinedPacket {
			PlayerData = controller.GetPlayerData()
		});

		this.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, peer);
		Console.WriteLine($"[OnConnectionRequest] accepted connection from {peer.EndPoint} ({peer.Id})");
	}

	public void Shutdown() {
		Console.WriteLine("Shutting down...");
		NetDataWriter writer = new NetDataWriter();
		this.PacketProcessor.Write(writer, new ServerShutdownPacket());
		this.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		Thread.Sleep(1000);
	}
}