using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Client.Renderers;
using Raylib_CsLo;
using DungeonCrawler.Core.Packets;
using System.Globalization;
namespace DungeonCrawler.Client;

public static class GameManager
{
    private static Dictionary<Guid, Entity> _entities = new Dictionary<Guid, Entity>();
    public static PlayerEntity localPlayer;
    private static Dictionary<Guid, DroppedLootItem> _lootItems = new Dictionary<Guid, DroppedLootItem>();

    public static void AddEntity(Entity entity)
    {
        _entities[entity.EntityId] = entity;
    }

    public static Entity GetEntityByID(Guid id)
    {
        if (!_entities.TryGetValue(id, out Entity value))
        {
            return null;
        }

        return value;
    }

    public static void RemoveEntity(Guid entityId)
    {
        if (_entities.Remove(entityId, out Entity entity))
        {
            entity.OnDestroy();
        }
    }

    public static void AddLootItem(DroppedLootItem lootItem)
    {
        _lootItems[lootItem.SpawnId] = lootItem;
    }

    public static void RemoveLootItem(Guid lootItemId)
    {
        _lootItems.Remove(lootItemId, out _);
    }

    public static void Update()
    {
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
            IRenderer renderer = entity.GetComponent<IRenderer>();
            if (renderer != null)
            {
                renderer.Draw();
            }
        }
    }
}