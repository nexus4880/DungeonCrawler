using System.Drawing;
using System.Numerics;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetDataWriterExtensions
{
	public static void PutDeserializable(this NetDataWriter writer, Object value)
	{
		Type valueType = value.GetType();
		UInt64 hash = LNHashCache.GetHash(valueType);
		writer.Put(hash);
		writer.Put(value as INetSerializable);
	}

	public static void Put(this NetDataWriter writer, Guid value)
	{
		writer.PutBytesWithLength(value.ToByteArray());
	}

	public static void Put(this NetDataWriter writer, Vector2 value)
	{
		writer.Put(value.X);
		writer.Put(value.Y);
	}

	public static void Put(this NetDataWriter writer, Point value)
	{
		writer.Put(value.X);
		writer.Put(value.Y);
	}

	public static void Put(this NetDataWriter writer, Rectangle value)
	{
		writer.Put(value.X);
		writer.Put(value.Y);
		writer.Put(value.Width);
		writer.Put(value.Height);
	}
}