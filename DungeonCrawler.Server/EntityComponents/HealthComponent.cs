using System.Collections;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Server.Entities;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.EntityComponents;

public class HealthComponent : IEntityComponent {
	public Single Value { get; set; }
	public Entity Owner { get; init; }

	public void Initialize(Stack properties) {
		this.Value = properties.PopValueOrThrow<Single>();
	}

	public void Serialize(NetDataWriter writer) {
		writer.Put(this.Value);
	}

	public void Deserialize(NetDataReader reader) {
		this.Value = reader.GetFloat();
	}
}