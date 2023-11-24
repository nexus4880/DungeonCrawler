using System.Numerics;

namespace DungeonCrawler.Core.Packets; 

public class PlayerMovedPacket {
	public Int32 Id { get; set; }
	public Single X { get; set; }
	public Single Y { get; set; }
}