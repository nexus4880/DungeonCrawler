using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Extensions;

namespace DungeonCrawler.Server.Entities.EntityComponents.Colliders;

[NetworkIgnore]
public class CircleColliderComponent : BaseColliderComponent {
	public float Radius { get; set; }

	public override void Initialize(IDictionary properties) {
		base.Initialize(properties);
		this.Radius = properties.GetValueAsOrThrow<float>(nameof(this.Radius));
	}

	public override bool CollidesWith(BaseColliderComponent other) {
		var delta = this.Owner.Position = other.Owner.Position;

		return other.ContainsPoint(this.Owner.Position + (delta * this.Radius));
	}

	public override bool ContainsPoint(Vector2 point) {

		return Vector2.Distance(point, this.Owner.Position) <= this.Radius;
	}
}
