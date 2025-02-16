using DungeonCrawler.Core.Map;

namespace DungeonCrawler.Client;

public class ClientBaseTile : BaseTile
{
    private Boolean _initialized;
    private Texture _texture;

    public unsafe void Initialize()
    {
        Image sourceImage = GameManager.ImageHandler.GetAsset($"assets/{this.TilesetSource}");
        Image imageBuffer = GenImageColor(GameManager.tileSize.width, GameManager.tileSize.height, BLANK);
        ImageDraw(&imageBuffer, sourceImage, new Rectangle(this.SourceRectPosition.X, this.SourceRectPosition.Y, this.SourceRectPosition.Width, this.SourceRectPosition.Height), new Rectangle(0, 0, GameManager.tileSize.width, GameManager.tileSize.height), WHITE);
        this._texture = LoadTextureFromImage(imageBuffer);
        this._initialized = true;
    }

    public void Draw()
    {
        Int32 x = this.WorldTilePosition.X * GameManager.tileSize.width;
        Int32 y = this.WorldTilePosition.Y * GameManager.tileSize.height;
        if (_initialized)
        {
            DrawTexture(this._texture, x, y, WHITE);
        }
        else
        {
            DrawRectangle(x, y, this.SourceRectPosition.Width, this.SourceRectPosition.Height, DARKGRAY);
        }
    }
}