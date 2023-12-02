using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetPacketProcessorExtensions {
	public static void Initialize(this NetPacketProcessor packetProcessor) {
		packetProcessor.RegisterNestedType((writer, value) => writer.Put(value), reader => reader.GetGuid());
		packetProcessor.RegisterNestedType<PlayerData>();
		packetProcessor.RegisterNestedType<PlayerInputs>();
		packetProcessor.RegisterNestedType<DroppedLootItem>();
	}
}