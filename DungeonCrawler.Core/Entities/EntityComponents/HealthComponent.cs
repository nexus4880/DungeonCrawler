using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents;

public class HealthComponent : BaseEntityComponent
{
	public Single Value { get; set; }

	public override void Initialize(Queue properties)
	{
		base.Initialize(properties);
		this.Value = properties.PopValueOrThrow<Single>();
	}

	public override void Serialize(NetDataWriter writer)
	{
		base.Serialize(writer);
		writer.Put(this.Value);
	}

	public override void Deserialize(NetDataReader reader)
	{
		base.Deserialize(reader);
		this.Value = reader.GetFloat();
	}
}