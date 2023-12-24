using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Extensions;

namespace DungeonCrawler.Server.Entities.EntityComponents.Colliders;

[NetworkIgnore]
public class BoxColliderComponent : BaseColliderComponent {
	public float Width { get; set; }
	public float Height { get; set; }

	public override void Initialize(IDictionary properties) {
		base.Initialize(properties);
		this.Width = properties.GetValueAsOrThrow<float>(nameof(this.Width));
		this.Height = properties.GetValueAsOrThrow<float>(nameof(this.Height));
	}

	public override bool CollidesWith(BaseColliderComponent other) {
		var posX = this.Owner.Position.X;
		var posY = this.Owner.Position.Y;
		var endX = posX + this.Width;
		var endY = posY + this.Height;

		var topLeft = new Vector2(posX, posY);
		var bottomLeft = new Vector2(posX, endY);
		var topRight = new Vector2(endX, posY);
		var bottomRight = new Vector2(endX, endY);

		return ((Vector2[])[topLeft, bottomLeft, topRight, bottomRight]).Any(other.ContainsPoint);
	}

	public override bool ContainsPoint(Vector2 point) {
		var posX = this.Owner.Position.X;
		var posY = this.Owner.Position.Y;
		var endX = posX + this.Width;
		var endY = posY + this.Height;

		return point.X >= posX && point.X <= endX && point.Y >= posY && point.Y <= endY;
	}
}
