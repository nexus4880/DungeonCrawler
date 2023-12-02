using System.Numerics;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public struct DroppedLootItem : INetSerializable {
	public Guid SpawnId { get; set; }
	public Vector2 Position { get; set; }
	public Item Item { get; set; }

	public void Serialize(NetDataWriter writer) {
		writer.Put(this.SpawnId);
		writer.Put(this.Position);
		writer.Put(this.Item);
	}

	public void Deserialize(NetDataReader reader) {
		this.SpawnId = reader.GetGuid();
		this.Position = reader.GetVector2();
		this.Item = reader.GetItem();
	}
}