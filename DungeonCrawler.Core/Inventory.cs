using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public class Inventory : INetSerializable {
	private readonly Dictionary<EEquipmentSlot, Item> _equipment = [];
	private List<Item>[] _hotbar;

	public Inventory(IInventoryOwner owner, int hotbarLength = 0) {
		this.Owner = owner;
		if (hotbarLength > 1) {
			this._hotbar = new List<Item>[hotbarLength];
			for (var i = 0; i < hotbarLength; i++) {
				this._hotbar[i] = [];
			}
		}
	}

	public IInventoryOwner Owner { get; }

	public void Serialize(NetDataWriter writer) {
		writer.Put(this._equipment.Count);
		foreach ((var part, var item) in this._equipment) {
			writer.Put((byte)part);
			writer.Put(item);
		}

		var hasHotbar = this._hotbar is not null;
		writer.Put(hasHotbar);
		if (hasHotbar) {
			foreach (var hotbar in this._hotbar) {
				writer.Put(hotbar.Count);
				foreach (var item in hotbar) {
					writer.Put(item);
				}
			}
		}
	}

	public void Deserialize(NetDataReader reader) {
		var equipmentCount = reader.GetInt();
		for (var i = 0; i < equipmentCount; i++) {
			var slot = (EEquipmentSlot)reader.GetByte();
			var item = reader.GetItem();
			this._equipment[slot] = item;
		}

		var hasHotbar = reader.GetBool();
		if (hasHotbar) {
			var hotbarsCount = reader.GetInt();
			if (this._hotbar is null) {
				this._hotbar = new List<Item>[hotbarsCount];
			}
			else {
				if (this._hotbar.Length < hotbarsCount) {
					Array.Resize(ref this._hotbar, hotbarsCount);
				}
			}

			for (var i = 0; i < hotbarsCount; i++) {
				var hotbarLength = reader.GetInt();
				this._hotbar[i] = new List<Item>(hotbarLength);
				for (var j = 0; j < hotbarLength; j++) {
					this._hotbar[i].Add(reader.GetItem());
				}
			}
		}
	}

	public bool AddItem(Item item) {
		if (this._hotbar is null) {
			return false;
		}

		if (this._hotbar[0] is null or { Count: 0 }) {
			return false;
		}

		this._hotbar[0].Add(item);

		return true;
	}
}