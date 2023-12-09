using System.Collections;
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
		writer.PutDeserializable(this.Item);
	}

	public override void Deserialize(NetDataReader reader)
	{
		base.Deserialize(reader);
		this.SpawnId = reader.GetGuid();
		this.Item = reader.GetDeserializable<Item>();
	}

	public override void Initialize(Queue properties)
	{
		base.Initialize(properties);
		this.SpawnId = properties.PopValueOrThrow<Guid>();
		this.Item = properties.PopValueOrThrow<Item>();
	}
}