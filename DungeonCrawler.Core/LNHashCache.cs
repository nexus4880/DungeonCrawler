using System.Reflection;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Helpers;

namespace DungeonCrawler.Core;

public static class LNHashCache {
	private static Dictionary<Type, ulong> _typeToHash = [];
	public static Dictionary<ulong, Type> _hashToType = [];

	public static int RegisterAllOfType<T>() {
		var registeredTypes = 0;
		foreach (var types in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes()).Where(t => t.IsAssignableTo(typeof(T)))) {
			try {
				_ = LNHashCache.RegisterType(types);
				registeredTypes++;
			}
			catch (RegisterTypeException) {
			}
		}

		return registeredTypes;
	}

	public static ulong RegisterType<T>() {
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

	public static ulong RegisterType(Type type) {
		ArgumentNullException.ThrowIfNull(type);
		if (type.IsAbstract) {
			throw new AbstractTypeException(type);
		}

		if (type.ContainsGenericParameters) {
			throw new GenericTypeException(type);
		}

		if (LNHashCache._typeToHash.TryGetValue(type, out var value)) {
			return value;
		}

		var hashAsAttribute = type.GetCustomAttribute<HashAsAttribute>();
		var hash = StringHelper.HashString(hashAsAttribute?.Value ?? type.ToString());
		if (LNHashCache._hashToType.ContainsKey(hash)) {
			return hash;
		}

		var text =
			hashAsAttribute is not null ?
			$"Registered Type: '{type}' with fixed hash {hash}" :
			$"Registered Type: '{type}' | {hash}";
		Console.WriteLine(text);
		LNHashCache._typeToHash[type] = hash;
		LNHashCache._hashToType[hash] = type;

		return hash;
	}

	public static ulong GetHash<T>() {
		return LNHashCache.GetHash(typeof(T));
	}

	public static ulong GetHash(Type type) {
		return !LNHashCache._typeToHash.TryGetValue(type, out var hash) ? RegisterType(type) : hash;
	}

	public static Type GetTypeByName(string hashText) {
		return LNHashCache.GetType(StringHelper.HashString(hashText));
	}

	public static Type GetType(ulong hash) {
		return LNHashCache._hashToType.GetValueOrDefault(hash);
	}
}