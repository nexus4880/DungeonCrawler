using System.Numerics;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetDataWriterExtensions {
	public static void PutDeserializable<T>(this NetDataWriter writer, T value) where T : INetSerializable {
		writer.Put(LNHashCache.GetHash<T>());
		writer.Put(value);
	}
	
	public static void Put(this NetDataWriter writer, Guid value) {
		writer.PutArray(value.ToByteArray(), 1);
	}

	public static void Put(this NetDataWriter writer, Vector2 value) {
		writer.Put(value.X);
		writer.Put(value.Y);
	}
}