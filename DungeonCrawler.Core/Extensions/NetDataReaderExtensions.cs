using System.Collections;
using System.Drawing;
using System.Numerics;
using DungeonCrawler.Core.Items;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetDataReaderExtensions {
	public static object GetDeserializable(this NetDataReader reader) {
		var hash = reader.GetULong();
		var type = LNHashCache.GetType(hash);
		if (type is null) {
			throw new Exception($"Cannot deserialize hash: {hash}");
		}

		Console.WriteLine($"Deserialized {type}");
		var result = Activator.CreateInstance(type);
		if (result is null) {
			throw new Exception($"Failed to create instance of {type}");
		}

		if (result is INetSerializable serializable) {
			serializable.Deserialize(reader);
		}

		return result;
	}

	public static T GetDeserializable<T>(this NetDataReader reader) {
		return (T)reader.GetDeserializable();
	}

	public static Item GetItem(this NetDataReader reader) {
		var hash = reader.GetULong();
		var type = LNHashCache.GetType(hash);
		if (type is null) {
			throw new Exception($"Cannot deserialize item hash {hash}");
		}

		var item = (Item)Activator.CreateInstance(type) ?? throw new Exception($"Cannot instantiate {type}");
		item.Deserialize(reader);

		return item;
	}

	public static Guid GetGuid(this NetDataReader reader) {
		return new Guid(reader.GetBytesWithLength());
	}

	public static Vector2 GetVector2(this NetDataReader reader) {
		return new Vector2(reader.GetFloat(), reader.GetFloat());
	}

	public static Point GetPoint(this NetDataReader reader) {
		return new Point(reader.GetInt(), reader.GetInt());
	}

	public static Rectangle GetRectangle(this NetDataReader reader) {
		return new Rectangle(reader.GetInt(), reader.GetInt(), reader.GetInt(), reader.GetInt());
	}

	public static IDictionary GetDictionary(this NetDataReader reader) {
		var length = reader.GetByte();
		var result = new Hashtable(length);
		for (byte i = 0; i < length; i++) {
			var key = reader.GetString();
			var typeCode = (TypeCode)reader.GetByte();
			switch (typeCode) {
				case TypeCode.Byte: {
					result[key] = reader.GetByte();

					break;
				}
				case TypeCode.Empty: {
					throw new Exception("Cannot read empty TypeCode... how is it empty?");
				}
				default: {
					throw new Exception($"Unsure how to read TypeCode: '{typeCode}'");
				}
			}
		}

		return result;
	}
}