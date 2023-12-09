using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetPacketProcessorExtensions
{
	public static void Initialize(this NetPacketProcessor packetProcessor)
	{
		packetProcessor.RegisterNestedType((writer, value) => writer.Put(value), reader => reader.GetGuid());
		packetProcessor.RegisterNestedType((writer, value) => writer.Put(value), reader => reader.GetVector2());
		packetProcessor.RegisterNestedType<PlayerInputs>();
	}
}