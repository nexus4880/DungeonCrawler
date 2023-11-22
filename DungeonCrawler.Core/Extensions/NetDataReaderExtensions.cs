using System.Numerics;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetDataReaderExtensions {
	public static Vector2 GetVector2(this NetDataReader reader) {
		return new Vector2(reader.GetFloat(), reader.GetFloat());
	}
}