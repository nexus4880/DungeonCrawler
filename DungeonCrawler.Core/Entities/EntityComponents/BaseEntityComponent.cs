using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents;

public abstract class BaseEntityComponent : INetSerializable
{
	public Entity Owner { get; set; }
	public Guid ComponentId { get; set; }

	public virtual void OnStateChange(IDictionary properties)
	{
	}

	public virtual void Initialize(IDictionary properties)
	{
	}

	public virtual void OnComponentRemoved()
	{
	}

	public virtual void Serialize(NetDataWriter writer)
	{
		writer.Put(this.ComponentId);
	}

	public virtual void Deserialize(NetDataReader reader)
	{
		this.ComponentId = reader.GetGuid();
	}
}