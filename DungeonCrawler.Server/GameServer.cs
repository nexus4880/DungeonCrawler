using System.Net;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server;

public class GameServer {
	public GameServer(IPAddress ipv4, IPAddress ipv6, Int32 port) {
		this.PacketProcessor.Setup();
		this.EventBasedNetListener.ConnectionRequestEvent += this.OnConnectionRequest;
		this.EventBasedNetListener.PeerConnectedEvent += this.OnPeerConnected;
		this.EventBasedNetListener.PeerDisconnectedEvent += this.OnPeerDisconnected;
		this.EventBasedNetListener.NetworkReceiveEvent += this.OnNetworkReceive;
		this.PacketProcessor.SubscribeReusable<SetInputsPacket, NetPeer>(this.OnPlayerSetInputsPacket);
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

	private void OnPlayerSetInputsPacket(SetInputsPacket packet, NetPeer peer) {
		PlayerController controller = this.GameManager.GetControllerById(peer.Id);
		if (controller is null) {
			return;
		}

		controller.inputs = packet.Inputs;
	}

	private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel, DeliveryMethod deliverymethod) {
		this.PacketProcessor.ReadAllPackets(reader);
	}

	private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo) {
		if (!this.GameManager.RemovePlayer(peer.Id)) {
			Console.WriteLine($"[OnPeerDisconnected] Failed to remove player {peer.Id}");

			return;
		}

		NetDataWriter writer = new NetDataWriter();
		this.PacketProcessor.Write(writer, new PlayerDisconnectedPacket { Id = peer.Id });
		this.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
	}

	public void Update() {
		this.NetManager.PollEvents();
		this.GameManager.Update();
	}

	private void OnPeerConnected(NetPeer peer) {
		PlayerController controller = this.GameManager.GetControllerById(peer.Id);
		if (controller is null) {
			Console.WriteLine($"[OnPeerConnected] failed to find PlayerController for {peer.Id}");
			peer.Disconnect();

			return;
		}

		NetDataWriter writer = new NetDataWriter();
		this.PacketProcessor.Write(writer, new PlayerJoinedPacket {
			Id = peer.Id,
			PlayerData = controller.GetPlayerData()
		});

		this.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, peer);
		Console.WriteLine("[OnPeerConnected]");
	}

	private void OnConnectionRequest(ConnectionRequest request) {
		NetPeer peer = request.AcceptIfKey("DungeonCrawler");
		if (peer is null) {
			Console.WriteLine("[OnConnectionRequest] key is not 'DungeonCrawler'");

			return;
		}

		NetDataWriter writer = new NetDataWriter();
		InitializeWorldPacket packet = new InitializeWorldPacket {
			Players = this.GameManager.AllPlayers.Values.Select(controller => controller.GetPlayerData()).ToArray()
		};

		this.PacketProcessor.Write(writer, packet);
		Console.WriteLine("[OnConnectionRequest]");
	}

	public void Shutdown() {
		Console.WriteLine("Shutting down...");
		NetDataWriter writer = new NetDataWriter();
		this.PacketProcessor.Write(writer, new ServerShutdownPacket());
		this.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		Thread.Sleep(1000);
	}
}