using DungeonCrawler.Core.Map;
using Raylib_CsLo;

namespace DungeonCrawler.Client;

public class ClientBaseTile : BaseTile
{
    private Boolean _initialized;
    private Texture _texture;

    public unsafe void Initialize()
    {
        Byte* bytes = Networking.currentVFS.PinnedBytes(this.TilesetSource, out Int32 length);
        if (bytes == null)
        {
            return;
        }

        Image image = LoadImageFromMemory(".png", bytes, length);
        Image target = GenImageColor(GameManager.tileSize.width, GameManager.tileSize.height, DARKGRAY);
        // TODO: this.X/Y * width/height is not right... I need to correlate it to the X and Y on the sprite sheet
        // I'm pretty sure X and Y should be the tile index X and Y... think more on this tomorrow...
        ImageDraw(&target, image, new Rectangle(this.X * GameManager.tileSize.width, this.Y * GameManager.tileSize.height, GameManager.tileSize.width, GameManager.tileSize.height), new Rectangle(0, 0, GameManager.tileSize.width, GameManager.tileSize.height), WHITE);
        this._texture = LoadTextureFromImage(target);
        this._initialized = true;
    }

    public void Draw()
    {
        Int32 x = this.X * GameManager.tileSize.width;
        Int32 y = this.Y * GameManager.tileSize.height;
        if (_initialized)
        {
            DrawTexture(this._texture, x, y, WHITE);
        }
        else
        {
            DrawRectangle(x, y, GameManager.tileSize.width, GameManager.tileSize.height, DARKGRAY);
        }
    }
}