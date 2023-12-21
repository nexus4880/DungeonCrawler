using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetDataWriterExtensions {
	public static void PutDeserializable(this NetDataWriter writer, Object value) {
		if (value is null) {
			throw new Exception("Cannot write null");
		}

		if (value is not INetSerializable netSerializable) {
			throw new Exception($"Cannot put {value.GetType()} as INetSerializable");
		}

		Type valueType = value.GetType();
		UInt64 hash = LNHashCache.GetHash(valueType);
		writer.Put(hash);
		writer.Put(netSerializable);
	}

	public static void Put(this NetDataWriter writer, Guid value) {
		writer.PutBytesWithLength(value.ToByteArray());
	}

	public static void Put(this NetDataWriter writer, Vector2 value) {
		writer.Put(value.X);
		writer.Put(value.Y);
	}

	public static void Put(this NetDataWriter writer, Point value) {
		writer.Put(value.X);
		writer.Put(value.Y);
	}

	public static void Put(this NetDataWriter writer, Rectangle value) {
		writer.Put(value.X);
		writer.Put(value.Y);
		writer.Put(value.Width);
		writer.Put(value.Height);
	}

	public static unsafe void Put<T>(this NetDataWriter writer, KeyValuePair<String, T> pair) where T : unmanaged {
		Byte[] bytes = new byte[sizeof(T)];
		if (!MemoryMarshal.TryWrite(bytes, pair.Value)) {
			throw new Exception($"Failed to write bytes from {pair.Value}");
		}

		writer.Put(pair.Key);
		writer.PutBytesWithLength(bytes);
	}

	public static void Put(this NetDataWriter writer, IDictionary dictionary) {
		writer.Put((Byte)dictionary.Count);
		foreach (DictionaryEntry entry in dictionary) {
			writer.Put((String)entry.Key);
			if (entry.Value is byte b) {
				writer.Put((Byte)TypeCode.Byte);
				writer.Put(b);
			}
			else {
				throw new Exception($"Cannot write '${entry.Value}', is there a check for it?");
			}
		}
	}
}