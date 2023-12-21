namespace DungeonCrawler.Core.Packets;

public class UpdateComponentPacket {
	public Guid Entity { get; set; }
	public Guid Component { get; set; }
}