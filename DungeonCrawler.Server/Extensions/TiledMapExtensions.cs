using TiledCS;

namespace DungeonCrawler.Server.Extensions;

public static class TiledMapExtensions {
	public static TiledLayer GetLayerByName(this TiledMap map, string layerName) {
		return map.Layers.FirstOrDefault(layer => layer.name == layerName);
	}
}