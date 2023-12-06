using System.Collections;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;

namespace DungeonCrawler.Server.Managers;

public static class GameManager
{
	public static readonly Dictionary<Guid, Entity> EntityList = new Dictionary<Guid, Entity>();
	public static readonly Dictionary<Guid, DroppedLootItem> LootItems = new Dictionary<Guid, DroppedLootItem>();

	public static IEnumerable<T> GetEntities<T>() where T : Entity
	{
		return GameManager.EntityList.Values.Where(entity => entity is T).Cast<T>();
	}

	public static void Update(Single deltaTime)
	{
		foreach (Entity entity in GameManager.EntityList.Values)
		{
			entity.Update(deltaTime);
		}
	}

	public static T CreateEntity<T>(params Object[] properties) where T : Entity, new()
	{
		Guid entityId;
		do
		{
			entityId = Guid.NewGuid();
		} while (GameManager.EntityList.ContainsKey(entityId));

		T entity = new T { EntityId = entityId };
		GameManager.EntityList[entityId] = entity;
		entity.Initialize(new Stack(properties));

		return entity;
	}

	public static Boolean DestroyEntity(Guid id)
	{
		Boolean removed = GameManager.EntityList.Remove(id, out Entity entity);
		if (removed)
		{
			entity.OnDestroy();
		}

		return removed;
	}
}