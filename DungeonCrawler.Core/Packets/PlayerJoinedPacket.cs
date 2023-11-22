namespace DungeonCrawler.Core.Packets; 

public class PlayerJoinedPacket {
	public Int32 Id { get; set; }
	public PlayerData PlayerData { get; set; }
}