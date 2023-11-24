using DungeonCrawler.Core.Items;

namespace DungeonCrawler.Core.Handlers;

public static class ItemSerializationHandler {
	private static Dictionary<UInt64, Type> _registeredTypes = new Dictionary<UInt64, Type>();

	public static void Initialize() {
		foreach (Type itemType in typeof(Item).Assembly.GetTypes()
			.Where(type => !type.IsAbstract && type.IsAssignableTo(typeof(Item)))) {
			ItemSerializationHandler.RegisterType(itemType);
		}

		Console.WriteLine($"Registered {ItemSerializationHandler._registeredTypes.Count} item types");
	}

	public static void RegisterType<T>() where T : Item {
		ItemSerializationHandler.RegisterType(typeof(T));
	}

	public static void RegisterType(Type type) {
		ItemSerializationHandler._registeredTypes[LNHashCache.GetHash(type)] = type;
	}

	public static Type GetTypeByHash(UInt64 hash) {
		return ItemSerializationHandler._registeredTypes.GetValueOrDefault(hash);
	}
}