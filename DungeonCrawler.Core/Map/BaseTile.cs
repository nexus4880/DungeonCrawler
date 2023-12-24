using System.Drawing;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Map;

public class BaseTile : INetSerializable {
	public string TilesetSource { get; set; }
	public Point WorldTilePosition { get; set; }
	public Rectangle SourceRectPosition { get; set; }
	public int Layer { get; set; }

	public void Deserialize(NetDataReader reader) {
		this.TilesetSource = reader.GetString();
		this.WorldTilePosition = reader.GetPoint();
		this.SourceRectPosition = reader.GetRectangle();
		this.Layer = reader.GetInt();
	}

	public void Serialize(NetDataWriter writer) {
		writer.Put(this.TilesetSource);
		writer.Put(this.WorldTilePosition);
		writer.Put(this.SourceRectPosition);
		writer.Put(this.Layer);
	}
}