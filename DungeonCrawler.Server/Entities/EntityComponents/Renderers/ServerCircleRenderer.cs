using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;

namespace DungeonCrawler.Server.Entities.EntityComponents.Renderers;

[HashAs(typeof(CircleRenderer))]
public class ServerCircleRenderer : CircleRenderer {
	public override void Draw() {
		throw new NotImplementedException();
	}
}