using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;
using TileSize = (int width, int height);

namespace DungeonCrawler.Client;

public static class GameManager {
	private static Dictionary<Guid, Entity> _entities = [];
	public static PlayerEntity localPlayer;
	public static List<ClientBaseTile> tiles = [];
	public static TileSize tileSize;
	public static Camera2D camera = new() { zoom = 1f };
	public static AssetHandler<Texture> TextureHandler { get; set; }
	public static AssetHandler<Image> ImageHandler { get; set; }

	public static void AddEntity(Entity entity) {
		_entities[entity.EntityId] = entity;
	}

	public static Entity GetEntityByID(Guid id) {
		return _entities.GetValueOrDefault(id);
	}

	public static void RemoveEntity(Guid entityId) {
		if (_entities.Remove(entityId, out var entity)) {
			entity.OnDestroy();
		}
	}

	public static void Update(float deltaTime) {
		foreach (var entity in GameManager._entities.Values) {
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
		foreach (var tile in GameManager.tiles) {
			tile.Draw();
		}

		foreach (var entity in _entities.Values) {
			var renderer = entity.GetComponent<BaseRenderer>();
			renderer?.Draw();
		}

		EndMode2D();
	}
}