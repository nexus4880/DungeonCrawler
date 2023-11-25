using DungeonCrawler.Core.Items;

namespace DungeonCrawler.Core;

public class Inventory {
	private readonly Dictionary<EEquipmentSlot, Item> _equipment;
	private List<Item>[] _hotbar;

	public Inventory(IInventoryOwner owner) {
		this.Owner = owner;
		this._hotbar = new List<Item>[4];
	}

	public IInventoryOwner Owner { get; }

	public Boolean AddItem(Item item, Int32 count) {
		return false;
	}
}