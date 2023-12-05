using DungeonCrawler.Core.Helpers;

namespace DungeonCrawler.Core;

public static class LNHashCache {
	private static Dictionary<Type, UInt64> _typeToHash = new Dictionary<Type, UInt64>();
	private static Dictionary<UInt64, Type> _hashToType = new Dictionary<UInt64, Type>();

	public static Int32 RegisterAllOfType<T>() {
		Int32 registeredTypes = 0;
		foreach (Type itemType in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes())
			.Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(T)))) {
			LNHashCache.RegisterType(itemType);
			registeredTypes++;
		}

		return registeredTypes;
	}

	public static void RegisterType<T>() {
		LNHashCache.RegisterType(typeof(T));
	}

	public static void RegisterType(Type type) {
		UInt64 hash = StringHelper.HashString(type.ToString());
		LNHashCache._typeToHash[type] = hash;
		LNHashCache._hashToType[hash] = type;
	}

	public static UInt64 GetHash<T>() {
		return LNHashCache.GetHash(typeof(T));
	}

	public static UInt64 GetHash(Type type) {
		if (!LNHashCache._typeToHash.TryGetValue(type, out UInt64 hash)) {
			LNHashCache._typeToHash[type] = hash = StringHelper.HashString(type.ToString());
			LNHashCache._hashToType[hash] = type;
		}

		return hash;
	}

	public static Type GetType(UInt64 hash) {
		return LNHashCache._hashToType.GetValueOrDefault(hash);
	}
}