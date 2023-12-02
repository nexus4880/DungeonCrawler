namespace DungeonCrawler.Core.Packets;

public class InitializeWorldPacket {
	public PlayerData[] Players { get; set; }
	public DroppedLootItem[] LootItems { get; set; }
}