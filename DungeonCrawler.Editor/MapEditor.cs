using System.Numerics;
using DungeonCrawler.Core;
namespace DungeonCrawler.Editor;
public class MapEditor
{
    private static WorldData _worldData;
    public static void GenerateEmptyWorldData(int height, int width)
    {
        _worldData = new WorldData
        {
            height = height,
            width = width
        };

        _worldData.tiles = new WorldData.TileData[_worldData.height, _worldData.width];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _worldData.tiles[i, j] = new WorldData.TileData { textureId = 0 };
            }
        }
    }

    public static void DrawMap()
    {
        int sizeX = 0;
        int sizeY = 0;
        for (int i = 0; i < _worldData.width; i++)
        {
            for (int j = 0; j < _worldData.height; j++)
            {
                if (_worldData.tiles[i, j].textureId == 0)
                {
                    DrawRectangleLines(sizeX, sizeY, 32, 32, WHITE);
                }
                else
                {
                    DrawRectangleLines(sizeX, sizeY, 32, 32, WHITE);
                    DrawTextureEx(TextureManager.textures.FirstOrDefault(x => x.id == (int)_worldData.tiles[i, j].textureId), new Vector2 { X = sizeX, Y = sizeY }, 0, 0.2f, WHITE);
                }

                sizeY += 32;
            }

            sizeY = 0;
            sizeX += 32;
        }
    }

    public static void SaveMap(string input)
    {
        using FileStream fileStream = File.OpenWrite(input);
        using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
        _worldData.Serialize(binaryWriter);
    }

    public static void LoadMap(string input){
        using FileStream fileStream = File.OpenRead(input);
        using BinaryReader binaryReader = new BinaryReader(fileStream);
        _worldData = WorldData.FromReader(binaryReader);
    }

    public static bool CreateTile(WorldData.TileData tile, int x, int y)
    {
        if (!DoesContainTile(x, y))
        {
            return false;
        }

        _worldData.tiles[x, y] = tile;
        return true;
    }

    public static WorldData.TileData? GetTile(int x, int y)
    {
        if (!DoesContainTile(x, y))
        {
            return null;
        }

        return _worldData.tiles[x, y];
    }

    public static void SetTileTextureID(int x, int y, ulong textureID)
    {
        _worldData.tiles[x, y].textureId = textureID;
    }

    public static void SetTile(int x, int y, WorldData.TileData tile)
    {
        _worldData.tiles[x, y] = tile;
    }

    public static bool DoesContainTile(int x, int y)
    {
        if (x >= _worldData.width || x < 0 || y >= _worldData.height || y < 0)
        {
            return false;
        }

        return true;
    }
}