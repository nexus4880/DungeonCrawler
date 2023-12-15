using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Map;

public class BaseTile : INetSerializable
{
    public String TilesetSource { get; set; }
    public Int32 X { get; set; }
    public Int32 Y { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        this.TilesetSource = reader.GetString();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(this.TilesetSource);
    }
}