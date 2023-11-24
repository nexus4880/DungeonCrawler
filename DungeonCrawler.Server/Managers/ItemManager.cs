using DungeonCrawler.Core.Items;

namespace DungeonCrawler.Server.Managers;

public class ItemManager {
	private Dictionary<Guid, Item> _items = new Dictionary<Guid, Item>();

	public T CreateItem<T>(params Object[] properties) where T : Item, new() {
		T item = new T { Id = Guid.NewGuid() };
		this._items[item.Id] = item;
		item.Initialize(properties);

		return item;
	}

	public Boolean ItemExists(Guid id) {
		return this._items.ContainsKey(id);
	}

	public void RemoveItem(Guid id) {
		this._items.Remove(id);
	}
}