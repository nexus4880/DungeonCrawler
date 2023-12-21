using System.Reflection;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Helpers;

namespace DungeonCrawler.Core;

public static class LNHashCache {
	private static Dictionary<Type, UInt64> _typeToHash = [];
	public static Dictionary<UInt64, Type> _hashToType = [];

	public static Int32 RegisterAllOfType<T>() {
		Int32 registeredTypes = 0;
		foreach (Type types in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes()).Where(t => t.IsAssignableTo(typeof(T)))) {
			try {
				LNHashCache.RegisterType(types);
				registeredTypes++;
			}
			catch (RegisterTypeException) {
			}
		}

		return registeredTypes;
	}

	public static UInt64 RegisterType<T>() {
		return LNHashCache.RegisterType(typeof(T));
	}

	private class RegisterTypeException : Exception {
		public Type Cause { get; protected set; }
	}

	private class AbstractTypeException : RegisterTypeException {
		public AbstractTypeException(Type type) {
			this.Cause = type;
		}
	}

	private class GenericTypeException : RegisterTypeException {
		public GenericTypeException(Type type) {
			this.Cause = type;
		}
	}

	public static UInt64 RegisterType(Type type) {
		ArgumentNullException.ThrowIfNull(type);
		if (type.IsAbstract) {
			throw new AbstractTypeException(type);
		}

		if (type.ContainsGenericParameters) {
			throw new GenericTypeException(type);
		}

		if (LNHashCache._typeToHash.TryGetValue(type, out UInt64 value)) {
			return value;
		}

		HashAsAttribute hashAsAttribute = type.GetCustomAttribute<HashAsAttribute>();
		UInt64 hash = StringHelper.HashString(hashAsAttribute?.Value ?? type.ToString());
		if (LNHashCache._hashToType.ContainsKey(hash)) {
			return hash;
		}

		String text =
			hashAsAttribute is not null ?
			$"Registered Type: '{type}' with fixed hash {hash}" :
			$"Registered Type: '{type}' | {hash}";
		Console.WriteLine(text);
		LNHashCache._typeToHash[type] = hash;
		LNHashCache._hashToType[hash] = type;

		return hash;
	}

	public static UInt64 GetHash<T>() {
		return LNHashCache.GetHash(typeof(T));
	}

	public static UInt64 GetHash(Type type) {
		return !LNHashCache._typeToHash.TryGetValue(type, out UInt64 hash) ? RegisterType(type) : hash;
	}

	public static Type GetTypeByName(String hashText) {
		return LNHashCache.GetType(StringHelper.HashString(hashText));
	}

	public static Type GetType(UInt64 hash) {
		return LNHashCache._hashToType.GetValueOrDefault(hash);
	}
}