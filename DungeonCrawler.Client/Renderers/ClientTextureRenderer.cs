using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;

namespace DungeonCrawler.Client.Renderers;

[HashAs(typeof(TextureRenderer))]
public class ClientTextureRenderer : TextureRenderer, IClientInitializable {
	private Texture _texture;

	public void ClientInitialize() {
		this._texture = GameManager.TextureHandler.GetAsset(this.TexturePath);
	}

	public override void Draw() {
		DrawTexture(this._texture, (int)this.Owner.Position.X, (int)this.Owner.Position.Y, WHITE);
	}
}