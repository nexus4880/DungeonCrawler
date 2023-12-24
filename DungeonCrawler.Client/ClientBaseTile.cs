using DungeonCrawler.Core.Map;

namespace DungeonCrawler.Client;

public class ClientBaseTile : BaseTile {
	private bool _initialized;
	private Texture _texture;

	public unsafe void Initialize() {
		var sourceImage = GameManager.ImageHandler.GetAsset($"assets/{this.TilesetSource}");
		var imageBuffer = GenImageColor(GameManager.tileSize.width, GameManager.tileSize.height, BLANK);
		ImageDraw(&imageBuffer, sourceImage, new Rectangle(this.SourceRectPosition.X, this.SourceRectPosition.Y, this.SourceRectPosition.Width, this.SourceRectPosition.Height), new Rectangle(0, 0, GameManager.tileSize.width, GameManager.tileSize.height), WHITE);
		this._texture = LoadTextureFromImage(imageBuffer);
		this._initialized = true;
	}

	public void Draw() {
		var x = this.WorldTilePosition.X * GameManager.tileSize.width;
		var y = this.WorldTilePosition.Y * GameManager.tileSize.height;
		if (this._initialized) {
			DrawTexture(this._texture, x, y, WHITE);
		}
		else {
			DrawRectangle(x, y, this.SourceRectPosition.Width, this.SourceRectPosition.Height, DARKGRAY);
		}
	}
}