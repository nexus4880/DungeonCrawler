using System.Numerics;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents;

namespace DungeonCrawler.Server.Entities.EntityComponents.Colliders;

[NetworkIgnore]
public abstract class BaseColliderComponent : BaseEntityComponent {
	public abstract Boolean CollidesWith(BaseColliderComponent other);
	public abstract Boolean ContainsPoint(Vector2 point);
}
