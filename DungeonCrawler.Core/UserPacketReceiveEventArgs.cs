using LiteNetLib;

namespace DungeonCrawler.Core;

public record UserPacketEventArgs(
		NetPeer Peer,
		NetPacketReader PacketReader,
		Byte Channel,
		DeliveryMethod DeliveryMethod) {
}