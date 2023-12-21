using System.Collections;
using System.Collections.Specialized;
using System.Numerics;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Managers;

public static class GameManager {
	public static readonly Dictionary<Guid, Entity> EntityList = [];

	public static IEnumerable<T> GetEntities<T>() where T : Entity {
		return GameManager.EntityList.Values.Where(entity => entity is T).Cast<T>();
	}

	public static void Update(Single deltaTime) {
		foreach (Entity entity in GameManager.EntityList.Values) {
			entity.Update(deltaTime);
			if (entity is PlayerEntity player) {
				foreach (DroppedLootItem droppedLootItem in GameManager.GetEntities<DroppedLootItem>()) {
					if (Vector2.Distance(droppedLootItem.Position, player.Position) < 16f) {
						switch (droppedLootItem.Item) {
							case InstantHealthPotion healthPotion: {
								HealthComponent healthComponent = player.GetComponent<HealthComponent>();
								if (healthComponent is null) {
									continue;
								}

								healthComponent.Value += healthPotion.Amount;

								break;
							}
							case SpeedPotion speedPotion: {
								MovementSpeedBuffComponent movementSpeedBuffComponent = player.GetComponent<MovementSpeedBuffComponent>();
								if (movementSpeedBuffComponent is not null) {
									if (speedPotion.Multiplier > movementSpeedBuffComponent.Value) {
										movementSpeedBuffComponent.Value = speedPotion.Multiplier;
									}

									if (speedPotion.Duration > movementSpeedBuffComponent.Duration) {
										movementSpeedBuffComponent.Duration = speedPotion.Duration;
									}
								}
								else {
									player.AddComponent<MovementSpeedBuffComponent>(new ListDictionary { { "Multiplier", speedPotion.Multiplier }, { "Duration", speedPotion.Duration } });
								}

								break;
							}
						}

						if (GameManager.DestroyEntity(droppedLootItem.EntityId)) {
						}
					}
				}
			}
		}
	}

	public static T CreateEntity<T>(IDictionary properties) where T : Entity, new() {
		Guid entityId = Guid.NewGuid();
		T entity = new T { EntityId = entityId };
		GameManager.EntityList[entityId] = entity;
		entity.Initialize(properties);

		return entity;
	}

	public static Boolean DestroyEntity(Guid id) {
		Boolean removed = GameManager.EntityList.Remove(id, out Entity entity);
		if (removed) {
			entity.OnDestroy();
			NetDataWriter writer = new NetDataWriter();
			GameServer.PacketProcessor.Write(writer, new EntityDestroyPacket { EntityId = id });
			GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}

		return removed;
	}
}