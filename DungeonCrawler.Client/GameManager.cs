using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using Raylib_CsLo;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;
namespace DungeonCrawler.Client;

using TileSize = (Int32 width, Int32 height);

public static class GameManager
{
    private static Dictionary<Guid, Entity> _entities = new Dictionary<Guid, Entity>();
    public static PlayerEntity localPlayer;
    public static ClientBaseTile[,] tiles;
    public static TileSize tileSize;
    public static Camera2D camera = new Camera2D { zoom = 1f };

    public static void AddEntity(Entity entity)
    {
        _entities[entity.EntityId] = entity;
    }

    public static Entity GetEntityByID(Guid id)
    {
        return _entities.GetValueOrDefault(id);
    }

    public static void RemoveEntity(Guid entityId)
    {
        if (_entities.Remove(entityId, out Entity entity))
        {
            entity.OnDestroy();
        }
    }

    public static void Update()
    {
        camera.offset.X = GetRenderWidth() / 2;
        camera.offset.Y = GetRenderHeight() / 2;
        if (localPlayer is null)
        {
            return;
        }

        camera.target = localPlayer.Position;
        var currentInputs = new PlayerInputs
        {
            MoveUp = IsKeyDown(KeyboardKey.KEY_W),
            MoveDown = IsKeyDown(KeyboardKey.KEY_S),
            MoveLeft = IsKeyDown(KeyboardKey.KEY_A),
            MoveRight = IsKeyDown(KeyboardKey.KEY_D)
        };

        if (localPlayer.CurrentInputs != currentInputs)
        {
            Networking.PacketProcessor.Write(Networking.Writer, new SetInputsPacket
            {
                Inputs = currentInputs
            });

            localPlayer.CurrentInputs = currentInputs;
        }
    }

    public static void Draw()
    {
        BeginMode2D(camera);
        foreach (var tile in GameManager.tiles)
        {
            tile.Draw();
        }

        foreach (var entity in _entities.Values)
        {
            BaseRenderer renderer = entity.GetComponent<BaseRenderer>();
            if (renderer != null)
            {
                renderer.Draw();
            }
        }

        EndMode2D();
    }
}