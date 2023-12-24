using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Extensions;
using TiledCS;

namespace DungeonCrawler.Server.Entities.EntityComponents.Colliders;

[NetworkIgnore]
public class BoxColliderComponent : BaseColliderComponent {
	public Single Width { get; set; }
	public Single Height { get; set; }

	public override void Initialize(IDictionary properties) {
		base.Initialize(properties);
		this.Width = properties.GetValueAsOrThrow<Single>(nameof(this.Width));
		this.Height = properties.GetValueAsOrThrow<Single>(nameof(this.Height));
	}

	public override Boolean CollidesWith(BaseColliderComponent other) {
		Single posX = this.Owner.Position.X;
		Single posY = this.Owner.Position.Y;
		Single endX = posX + this.Width;
		Single endY = posY + this.Height;

		Vector2 topLeft = new Vector2(posX, posY);
		Vector2 bottomLeft = new Vector2(posX, endY);
		Vector2 topRight = new Vector2(endX, posY);
		Vector2 bottomRight = new Vector2(endX, endY);

		return ((Vector2[])[topLeft, bottomLeft, topRight, bottomRight]).Any(other.ContainsPoint);
	}

	public override Boolean ContainsPoint(Vector2 point) {
		Single posX = this.Owner.Position.X;
		Single posY = this.Owner.Position.Y;
		Single endX = posX + this.Width;
		Single endY = posY + this.Height;

		return point.X >= posX && point.X <= endX && point.Y >= posY && point.Y <= endY;
	}
}
