namespace DungeonCrawler.Core;

public struct WorldData
{
    public struct TileData
    {
        public UInt64 textureId;
        public unsafe fixed Boolean canMoveOnto[4];

        public static unsafe TileData FromReader(BinaryReader reader)
        {
            TileData tile = new TileData();
            tile.textureId = reader.ReadUInt64();
            for (Int32 i = 0; i < 4; i++)
            {
                tile.canMoveOnto[i] = reader.ReadBoolean();
            }

            return tile;
        }

        public unsafe void Serialize(BinaryWriter writer)
        {
            writer.Write(this.textureId);
            for (Int32 i = 0; i < 4; i++)
            {
                writer.Write(this.canMoveOnto[i]);
            }
        }
    }

    public Int32 width;
    public Int32 height;
    public TileData[,] tiles;

    public static WorldData FromReader(BinaryReader reader)
    {
        WorldData world = new WorldData();
        world.width = reader.ReadInt32();
        world.height = reader.ReadInt32();
        world.tiles = new TileData[world.width, world.height];
        for (Int32 x = 0; x < world.width; x++)
        {
            for (Int32 y = 0; y < world.height; y++)
            {
                world.tiles[x, y] = TileData.FromReader(reader);
            }
        }

        return world;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(this.width);
        writer.Write(this.height);
        for (Int32 x = 0; x < this.width; x++)
        {
            for (Int32 y = 0; y < this.height; y++)
            {
                this.tiles[x, y].Serialize(writer);
            }
        }
    }
}