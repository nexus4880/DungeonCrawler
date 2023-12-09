using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public class DroppedLootItem : Entity
{
	public Guid SpawnId { get; set; }
	public Item Item { get; set; }

	public override void Serialize(NetDataWriter writer)
	{
		base.Serialize(writer);
		writer.Put(this.SpawnId);
		writer.Put(this.Item);
	}

	public override void Deserialize(NetDataReader reader)
	{
		base.Deserialize(reader);
		this.SpawnId = reader.GetGuid();
		this.Item = reader.GetItem();
	}
}