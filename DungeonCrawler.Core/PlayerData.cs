using System.Numerics;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public struct PlayerData : INetSerializable {
	public Int32 id;
	public Single health;
	public Vector2 position;
	public List<Item> items;

	public void Serialize(NetDataWriter writer) {
		writer.Put(this.id);
		writer.Put(this.health);
		writer.Put(this.position);
		writer.Put(this.items.Count);
		foreach (Item item in this.items) {
			writer.Put(item);
		}
	}

	public void Deserialize(NetDataReader reader) {
		this.id = reader.GetInt();
		this.health = reader.GetFloat();
		this.position = reader.GetVector2();
		Int32 length = reader.GetInt();
		this.items = new List<Item>(length);
		for (Int32 i = 0; i < length; i++) {
			this.items.Add(reader.GetItem());
		}
	}

	public override string ToString() {
		return $"{nameof(this.id)}: {this.id}, {nameof(this.health)}: {this.health}, {nameof(this.position)}: {this.position}";
	}
}