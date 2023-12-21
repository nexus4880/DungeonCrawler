using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;

namespace DungeonCrawler.Client.Renderers;

[HashAs(typeof(CircleRenderer))]
public class ClientCircleRenderer : CircleRenderer {
	public override void Draw() {
		Action<Int32, Int32, Single, Color> targetMethod = this.Filled ? DrawCircle : DrawCircleLines;
		targetMethod((Int32)this.Owner.Position.X, (Int32)this.Owner.Position.Y, this.Radius, GetColor(this.Color));
	}
}