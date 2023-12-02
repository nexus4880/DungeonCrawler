using System.Collections;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Server.Entities;

namespace DungeonCrawler.Server.EntityComponents;

public class HealthComponent : IEntityComponent {
	public Single Value { get; set; }
	public Entity Owner { get; init; }

	public void Initialize(Stack properties) {
		this.Value = properties.PopValueOrThrow<Single>();
	}
}