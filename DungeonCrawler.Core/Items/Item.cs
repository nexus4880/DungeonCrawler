using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Items;

public abstract class Item : INetSerializable
{
	public Guid Id { get; set; }
	public abstract String Name { get; set; }
	public IInventoryOwner Owner { get; set; }

	public virtual void Serialize(NetDataWriter writer)
	{
		writer.Put(this.Id);
		writer.Put(this.Name);
	}

	public virtual void Deserialize(NetDataReader reader)
	{
		this.Id = reader.GetGuid();
		this.Name = reader.GetString();
	}

	/// <param name="properties"></param>
	public virtual void Initialize(Queue properties)
	{
	}
}