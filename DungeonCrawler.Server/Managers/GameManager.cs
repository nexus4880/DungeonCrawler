using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

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
			if (entity is PlayerEntity player)
			{
				Vector2 movement = Vector2.Zero;
				if (player.CurrentInputs.MoveLeft)
				{
					movement.X -= 1f;
				}

				if (player.CurrentInputs.MoveRight)
				{
					movement.X += 1f;
				}

				if (player.CurrentInputs.MoveDown)
				{
					movement.Y += 1f;
				}

				if (player.CurrentInputs.MoveUp)
				{
					movement.Y -= 1f;
				}

				if (movement.Length() != 0f)
				{
					player.Position += movement;
					NetDataWriter writer = new NetDataWriter();
					GameServer.PacketProcessor.Write(writer, new EntityMovedPacket { EntityId = player.EntityId, Position = player.Position });
					GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
				}

				var lootItems = GameManager.GetEntities<DroppedLootItem>().ToArray();
				foreach (var lootItem in lootItems)
				{
					if (Vector2.Distance(lootItem.Position, player.Position) < 16f)
					{
						GameManager.DestroyEntity(lootItem.EntityId);
					}
				}
			}
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
		entity.Initialize(new Queue(properties));

		return entity;
	}

	public static Boolean DestroyEntity(Guid id)
	{
		Boolean removed = GameManager.EntityList.Remove(id, out Entity entity);
		if (removed)
		{
			entity.OnDestroy();
			NetDataWriter writer = new NetDataWriter();
			GameServer.PacketProcessor.Write(writer, new EntityDestroyPacket { EntityId = id });
			GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}

		return removed;
	}
}