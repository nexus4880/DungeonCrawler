using DungeonCrawler.Core.Entities.EntityComponents.Renderers;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities;

namespace DungeonCrawler.Client.Renderers;

[HashAs("DungeonCrawler.Core.Entities.EntityComponents.Renderers.TextureRenderer")]
public class ClientTextureRenderer : TextureRenderer, IClientInitializable
{
    private Texture _texture;

    public void ClientInitialize()
    {
        this._texture = GameManager.TextureHandler.GetAsset(this.TexturePath);
    }

    public override void Draw()
    {
        DrawTexture(this._texture, (Int32)this.Owner.Position.X, (Int32)this.Owner.Position.Y, WHITE);
    }
}