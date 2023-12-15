using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using Raylib_CsLo;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Core.Entities.EntityComponents.Renderers;
namespace DungeonCrawler.Client;

public static class GameManager
{
    private static Dictionary<Guid, Entity> _entities = new Dictionary<Guid, Entity>();
    public static PlayerEntity localPlayer;

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
        if (localPlayer is null)
        {
            return;
        }

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
        foreach (var entity in _entities.Values)
        {
            BaseRenderer renderer = entity.GetComponent<BaseRenderer>();
            if (renderer != null)
            {
                renderer.Draw();
            }
        }
    }
}