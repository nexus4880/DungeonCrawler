using LiteNetLib;

namespace DungeonCrawler.Core;

public record UserPacketEventArgs(
		NetPeer Peer,
		NetPacketReader PacketReader,
		byte Channel,
		DeliveryMethod DeliveryMethod) {
}