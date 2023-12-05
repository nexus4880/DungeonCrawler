namespace DungeonCrawler.Core.Packets;

public class LootItemDroppedPacket {
	public Guid Id { get; set; }
	public DroppedLootItem LootItem { get; set; }
}