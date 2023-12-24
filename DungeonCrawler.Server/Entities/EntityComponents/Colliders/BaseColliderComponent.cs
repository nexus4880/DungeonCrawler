using System.Numerics;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents;

namespace DungeonCrawler.Server.Entities.EntityComponents.Colliders;

[NetworkIgnore]
public abstract class BaseColliderComponent : BaseEntityComponent {
	public abstract bool CollidesWith(BaseColliderComponent other);
	public abstract bool ContainsPoint(Vector2 point);
}
