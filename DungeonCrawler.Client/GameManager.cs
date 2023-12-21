using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;
using TileSize = (int width, int height);

namespace DungeonCrawler.Client;

public static class GameManager {
	private static Dictionary<Guid, Entity> _entities = [];
	public static PlayerEntity localPlayer;
	public static List<ClientBaseTile> tiles = [];
	public static TileSize tileSize;
	public static Camera2D camera = new Camera2D { zoom = 1f };
	public static AssetHandler<Texture> TextureHandler { get; set; }
	public static AssetHandler<Image> ImageHandler { get; set; }

	public static void AddEntity(Entity entity) {
		_entities[entity.EntityId] = entity;
	}

	public static Entity GetEntityByID(Guid id) {
		return _entities.GetValueOrDefault(id);
	}

	public static void RemoveEntity(Guid entityId) {
		if (_entities.Remove(entityId, out Entity entity)) {
			entity.OnDestroy();
		}
	}

	public static void Update(Single deltaTime) {
		foreach (Entity entity in GameManager._entities.Values) {
			entity.Update(deltaTime);
		}

		camera.offset.X = GetRenderWidth() / 2;
		camera.offset.Y = GetRenderHeight() / 2;
		if (localPlayer is null) {
			return;
		}

		camera.target = localPlayer.Position;
	}

	public static void Draw() {
		BeginMode2D(camera);
		foreach (ClientBaseTile tile in GameManager.tiles) {
			tile.Draw();
		}

		foreach (Entity entity in _entities.Values) {
			BaseRenderer renderer = entity.GetComponent<BaseRenderer>();
			renderer?.Draw();
		}

		EndMode2D();
	}
}