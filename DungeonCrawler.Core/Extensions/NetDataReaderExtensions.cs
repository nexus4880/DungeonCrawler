using System.Numerics;
using DungeonCrawler.Core.Items;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Extensions;

public static class NetDataReaderExtensions
{
	public static Object GetDeserializable(this NetDataReader reader)
	{
		UInt64 hash = reader.GetULong();
		Type type = LNHashCache.GetType(hash);
		if (type is null)
		{
			throw new Exception($"Cannot deserialize hash: {hash}");
		}

		Console.WriteLine($"Deserialized {type}");
		Object result = Activator.CreateInstance(type);
		if (result is null)
		{
			throw new Exception($"Failed to create instance of {type}");
		}

		if (result is INetSerializable serializable)
		{
			serializable.Deserialize(reader);
		}

		return result;
	}

	public static T GetDeserializable<T>(this NetDataReader reader)
	{
		return (T)reader.GetDeserializable();
	}

	public static Item GetItem(this NetDataReader reader)
	{
		UInt64 hash = reader.GetULong();
		Type type = LNHashCache.GetType(hash);
		if (type is null)
		{
			throw new Exception($"Cannot deserialize item hash {hash}");
		}

		Item item = (Item)Activator.CreateInstance(type) ?? throw new Exception($"Cannot instantiate {type}");
		item.Deserialize(reader);

		return item;
	}

	public static Guid GetGuid(this NetDataReader reader)
	{
		return new Guid(reader.GetBytesWithLength());
	}

	public static Vector2 GetVector2(this NetDataReader reader)
	{
		return new Vector2(reader.GetFloat(), reader.GetFloat());
	}
}