using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public class Inventory : INetSerializable {
	private readonly Dictionary<EEquipmentSlot, Item> _equipment = [];
	private List<Item>[] _hotbar;

	public Inventory(IInventoryOwner owner, Int32 hotbarLength = 0) {
		this.Owner = owner;
		if (hotbarLength > 1) {
			this._hotbar = new List<Item>[hotbarLength];
			for (Int32 i = 0; i < hotbarLength; i++) {
				this._hotbar[i] = [];
			}
		}
	}

	public IInventoryOwner Owner { get; }

	public void Serialize(NetDataWriter writer) {
		writer.Put(this._equipment.Count);
		foreach ((EEquipmentSlot part, Item item) in this._equipment) {
			writer.Put((Byte)part);
			writer.Put(item);
		}

		Boolean hasHotbar = this._hotbar is not null;
		writer.Put(hasHotbar);
		if (hasHotbar) {
			foreach (List<Item> hotbar in this._hotbar) {
				writer.Put(hotbar.Count);
				foreach (Item item in hotbar) {
					writer.Put(item);
				}
			}
		}
	}

	public void Deserialize(NetDataReader reader) {
		Int32 equipmentCount = reader.GetInt();
		for (Int32 i = 0; i < equipmentCount; i++) {
			EEquipmentSlot slot = (EEquipmentSlot)reader.GetByte();
			Item item = reader.GetItem();
			this._equipment[slot] = item;
		}

		Boolean hasHotbar = reader.GetBool();
		if (hasHotbar) {
			Int32 hotbarsCount = reader.GetInt();
			if (this._hotbar is null) {
				this._hotbar = new List<Item>[hotbarsCount];
			}
			else {
				if (this._hotbar.Length < hotbarsCount) {
					Array.Resize(ref this._hotbar, hotbarsCount);
				}
			}

			for (Int32 i = 0; i < hotbarsCount; i++) {
				Int32 hotbarLength = reader.GetInt();
				this._hotbar[i] = new List<Item>(hotbarLength);
				for (Int32 j = 0; j < hotbarLength; j++) {
					this._hotbar[i].Add(reader.GetItem());
				}
			}
		}
	}

	public Boolean AddItem(Item item) {
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