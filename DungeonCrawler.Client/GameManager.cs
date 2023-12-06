using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;

namespace DungeonCrawler.Client;

public static class GameManager
{
    private static Dictionary<Guid, Entity> _entities = new Dictionary<Guid, Entity>();
    private static Dictionary<Guid, DroppedLootItem> _lootItems = new Dictionary<Guid, DroppedLootItem>();

    public static void AddEntity(Entity entity)
    {
        _entities[entity.EntityId] = entity;
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
        if (_lootItems.Remove(lootItemId, out DroppedLootItem lootItem))
        {
            // Don't know if I want something to be done on the client just yet
        }
    }

    public static void Update()
    {
    }

    public static void Draw()
    {
        foreach (var entity in _entities.Values)
        {

        }
    }
}