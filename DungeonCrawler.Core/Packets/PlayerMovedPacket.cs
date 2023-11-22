using System.Numerics;

namespace DungeonCrawler.Core.Packets; 

public class PlayerMovedPacket {
	public Int32 Id { get; set; }
	public Vector2 Position { get; set; }
}