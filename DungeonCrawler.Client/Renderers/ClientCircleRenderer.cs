using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;

namespace DungeonCrawler.Client.Renderers;

[HashAs(typeof(CircleRenderer))]
public class ClientCircleRenderer : CircleRenderer {
	public override void Draw() {
		Action<int, int, float, Color> targetMethod = this.Filled ? DrawCircle : DrawCircleLines;
		targetMethod((int)this.Owner.Position.X, (int)this.Owner.Position.Y, this.Radius, GetColor(this.Color));
	}
}