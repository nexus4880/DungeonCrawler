using System.Numerics;

namespace DungeonCrawler.Core.Packets;

public class EntityMovedPacket {
	public Guid EntityId { get; set; }
	public Vector2 Position { get; set; }
}