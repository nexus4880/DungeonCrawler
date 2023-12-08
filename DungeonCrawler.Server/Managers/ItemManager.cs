using System.Collections;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Managers;

public static class ItemManager
{
	private static Dictionary<Guid, Item> _items = new Dictionary<Guid, Item>();

	public static T CreateItem<T>(params Object[] properties) where T : Item, new()
	{
		return (T)ItemManager.CreateItem(typeof(T), properties);
	}

	public static Item CreateItem(Type itemType, params Object[] properties)
	{
		Item item = (Item)Activator.CreateInstance(itemType)!;
		item.Id = Guid.NewGuid();
		ItemManager._items[item.Id] = item;
		item.Initialize(new Queue(properties));

		return item;
	}

	public static Boolean ItemExists(Guid id)
	{
		return ItemManager._items.ContainsKey(id);
	}

	public static void RemoveItem(Guid id)
	{
		if (ItemManager._items.Remove(id))
		{
			NetDataWriter writer = new NetDataWriter();
			GameServer.PacketProcessor.Write(writer, new RemoveItemPacket { ItemId = id });
			GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}
	}
}