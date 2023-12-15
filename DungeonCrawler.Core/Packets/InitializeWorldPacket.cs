namespace DungeonCrawler.Core.Packets;

public class InitializeWorldPacket
{
    public Int32 EntitiesCount { get; set; }
    public Guid LocalPlayerEntityId { get; set; }
}