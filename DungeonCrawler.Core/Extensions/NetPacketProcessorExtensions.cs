using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetPacketProcessorExtensions {
	public static void Setup(this NetPacketProcessor packetProcessor) {
		packetProcessor.RegisterNestedType<PlayerData>();
		packetProcessor.RegisterNestedType<PlayerInputs>();
	}
}