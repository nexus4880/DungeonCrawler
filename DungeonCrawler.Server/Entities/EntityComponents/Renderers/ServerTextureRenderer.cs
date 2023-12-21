using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;

namespace DungeonCrawler.Server.Entities.EntityComponents.Renderers;

[HashAs(typeof(TextureRenderer))]
public class ServerTextureRenderer : TextureRenderer {
	public override void Draw() {
		throw new NotImplementedException();
	}
}