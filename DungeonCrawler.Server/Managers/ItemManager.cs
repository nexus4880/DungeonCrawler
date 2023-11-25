using System.Collections;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Managers;

public class ItemManager {
	private GameManager _gameManager;
	private Dictionary<Guid, Item> _items = new Dictionary<Guid, Item>();

	public ItemManager(GameManager gameManager) {
		this._gameManager = gameManager;
	}

	public T CreateItem<T>(params Object[] properties) where T : Item, new() {
		T item = new T { Id = Guid.NewGuid() };
		this._items[item.Id] = item;
		item.Initialize(new Stack(properties));

		return item;
	}

	public Boolean ItemExists(Guid id) {
		return this._items.ContainsKey(id);
	}

	public void RemoveItem(Guid id) {
		if (this._items.Remove(id)) {
			NetDataWriter writer = new NetDataWriter();
			this._gameManager.Server.PacketProcessor.Write(writer, new RemoveItemPacket { ItemId = id });
			this._gameManager.Server.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}
	}
}