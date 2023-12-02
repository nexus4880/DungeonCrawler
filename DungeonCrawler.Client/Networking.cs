using DungeonCrawler.Core.Extensions;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Client;

public static class Networking {
	public static NetPacketProcessor PacketProcessor { get; private set; }
	public static NetManager NetManager { get; private set; }
	public static EventBasedNetListener EventBasedNetListener { get; private set; }
	public static NetPeer LocalPeer { get; set; }
	public static NetDataWriter Writer { get; } = new NetDataWriter(true, UInt16.MaxValue);

	public static void Initialize() {
		Networking.PacketProcessor = new NetPacketProcessor();
		Networking.PacketProcessor.Initialize();
		Networking.EventBasedNetListener = new EventBasedNetListener();
		Networking.EventBasedNetListener.NetworkReceiveEvent += Networking.OnNetworkReceive;
		Networking.NetManager = new NetManager(Networking.EventBasedNetListener);
	}

	public static void Update() {
		Networking.NetManager.PollEvents();
		if (Networking.Writer.Length > 0) {
			Networking.LocalPeer.Send(Networking.Writer, DeliveryMethod.Unreliable);
			Networking.Writer.Reset();
		}
	}

	public static void Subscribe<T>(Action<T, UserPacketEventArgs> onReceive) where T : class, new() {
		Networking.PacketProcessor.SubscribeReusable(onReceive);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliverymethod) {
		Console.WriteLine("[OnNetworkReceive]");
		try {
			Networking.PacketProcessor.ReadAllPackets(reader,
				new UserPacketEventArgs(peer, reader, channel, deliverymethod));
		}
		catch (ParseException) {
		}
	}

	public record UserPacketEventArgs(
		NetPeer Peer,
		NetPacketReader PacketReader,
		Byte Channel,
		DeliveryMethod DeliveryMethod) {
	}
}