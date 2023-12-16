using System.Numerics;
using DungeonCrawler.Core.Map;

namespace DungeonCrawler.Client;

public class ClientBaseTile : BaseTile
{
    private Boolean _initialized;
    private Texture _texture;

    public unsafe void Initialize()
    {
        Image image = GameManager.ImageHandler.GetAsset($"assets/{this.TilesetSource}");
        Image target = GenImageColor(GameManager.tileSize.width, GameManager.tileSize.height, DARKGRAY);
        ImageDraw(&target, image, new Rectangle(this.SourceRectPosition.X, this.SourceRectPosition.Y, this.SourceRectPosition.Width, this.SourceRectPosition.Height), new Rectangle(0, 0, GameManager.tileSize.width, GameManager.tileSize.height), WHITE);
        this._texture = LoadTextureFromImage(target);
        this._initialized = true;
    }

    public void Draw()
    {
        Int32 x = this.WorldTilePosition.X * GameManager.tileSize.width;
        Int32 y = this.WorldTilePosition.Y * GameManager.tileSize.height;
        if (_initialized)
        {
            DrawTexturePro(this._texture,
             new Rectangle(this.SourceRectPosition.X, this.SourceRectPosition.Y, this.SourceRectPosition.Width, this.SourceRectPosition.Height),
              new Rectangle { X = x, Y = y, width = GameManager.tileSize.width, height = GameManager.tileSize.height },
               Vector2.Zero,
               0f,
               BLANK);
        }
        else
        {
            DrawRectangle(x, y, this.SourceRectPosition.Width, this.SourceRectPosition.Height, DARKGRAY);
        }
    }
}