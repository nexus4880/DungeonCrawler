using DungeonCrawler.Core.Helpers;

namespace DungeonCrawler.Core;

public static class LNHashCache {
	private static Dictionary<Type, UInt64> _hashes = new Dictionary<Type, UInt64>();

	public static UInt64 GetHash<T>() {
		return LNHashCache.GetHash(typeof(T));
	}

	public static UInt64 GetHash(Type type) {
		if (!LNHashCache._hashes.TryGetValue(type, out UInt64 hash)) {
			LNHashCache._hashes[type] = hash = StringHelper.HashString(type.ToString());
		}

		return hash;
	}
}