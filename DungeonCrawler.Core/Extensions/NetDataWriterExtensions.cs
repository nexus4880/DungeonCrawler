using System.Numerics;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions; 

public static class NetDataWriterExtensions {
	public static void Put(this NetDataWriter writer, Vector2 value) {
		writer.Put(value.X);
		writer.Put(value.Y);
	}
}