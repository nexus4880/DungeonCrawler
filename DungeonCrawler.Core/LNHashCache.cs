using System.Reflection;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Helpers;

namespace DungeonCrawler.Core;

public static class LNHashCache
{
	private static Dictionary<Type, UInt64> _typeToHash = new Dictionary<Type, UInt64>();
	public static Dictionary<UInt64, Type> _hashToType = new Dictionary<UInt64, Type>();

	public static Int32 RegisterAllOfType<T>()
	{
		Int32 registeredTypes = 0;
		foreach (Type itemType in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes())
			.Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(T))))
		{
			LNHashCache.RegisterType(itemType);
			registeredTypes++;
		}

		return registeredTypes;
	}

	public static UInt64 RegisterType<T>()
	{
		return LNHashCache.RegisterType(typeof(T));
	}

	public static UInt64 RegisterType(Type type)
	{
		HashAsAttribute hashAsAttribute = type.GetCustomAttribute<HashAsAttribute>();
		UInt64 hash = StringHelper.HashString(hashAsAttribute?.Value ?? type.ToString());
		String text =
			hashAsAttribute is not null ?
			$"Registered Type: '{type}' with fixed hash {hash}" :
			$"Registered Type: '{type}' | {hash}";
		Console.WriteLine(text);
		LNHashCache._typeToHash[type] = hash;
		LNHashCache._hashToType[hash] = type;

		return hash;
	}

	public static UInt64 GetHash<T>()
	{
		return LNHashCache.GetHash(typeof(T));
	}

	public static UInt64 GetHash(Type type)
	{
		if (!LNHashCache._typeToHash.TryGetValue(type, out UInt64 hash))
		{
			return RegisterType(type);
		}

		return hash;
	}

	public static Type GetTypeByName(String hashText)
	{
		return LNHashCache.GetType(StringHelper.HashString(hashText));
	}

	public static Type GetType(UInt64 hash)
	{
		return LNHashCache._hashToType.GetValueOrDefault(hash);
	}
}