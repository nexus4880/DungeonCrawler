using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Extensions;

namespace DungeonCrawler.Server.Entities.EntityComponents.Colliders;

[NetworkIgnore]
public class CircleColliderComponent : BaseColliderComponent {
	public Single Radius { get; set; }

	public override void Initialize(IDictionary properties) {
		base.Initialize(properties);
		this.Radius = properties.GetValueAsOrThrow<Single>(nameof(this.Radius));
	}

	public override Boolean CollidesWith(BaseColliderComponent other) {
		throw new NotImplementedException();
	}

	public override Boolean ContainsPoint(Vector2 point) {

		return Vector2.Distance(point, this.Owner.Position) <= this.Radius;
	}
}
