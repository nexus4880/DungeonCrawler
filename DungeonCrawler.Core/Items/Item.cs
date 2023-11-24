using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Items;

public abstract class Item : INetSerializable {
	public Guid Id { get; set; }

	public virtual void Serialize(NetDataWriter writer) {
		writer.Put(LNHashCache.GetHash(this.GetType()));
		writer.Put(this.Id);
	}

	public virtual void Deserialize(NetDataReader reader) {
		this.Id = reader.GetGuid();
	}

	public virtual void Initialize(params Object[] properties) {
 	}
 }