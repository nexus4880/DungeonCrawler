namespace DungeonCrawler.Core.Packets;

public class InitializeWorldPacket {
	public Int32 EntitiesCount { get; set; }
	public Int32 WorldWidth { get; set; }
	public Int32 WorldHeight { get; set; }
	public Int32 TileWidth { get; set; }
	public Int32 TileHeight { get; set; }
	public Int32 TileCount { get; set; }
}