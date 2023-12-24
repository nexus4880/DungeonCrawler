namespace DungeonCrawler.Core.Packets;

public class InitializeWorldPacket {
	public int EntitiesCount { get; set; }
	public int WorldWidth { get; set; }
	public int WorldHeight { get; set; }
	public int TileWidth { get; set; }
	public int TileHeight { get; set; }
	public int TileCount { get; set; }
}