using System.Collections;
using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Server.EntityComponents;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

public abstract class Entity : IInventoryOwner, INetSerializable {
	private List<IEntityComponent> _entityComponents = [];

	public Vector2 position;

	public Guid EntityId { get; init; }

	public virtual Inventory GetInventory() {
		return null;
	}

	public abstract void Update(Single deltaTime);

	public T GetComponent<T>() where T : class, IEntityComponent {
		return this._entityComponents.FirstOrDefault(comp => comp is T) as T;
	}

	public T AddComponent<T>(params Object[] properties) where T : IEntityComponent, new() {
		T component = new T {
			Owner = this
		};

		Stack props = new Stack(properties);
		component.Initialize(props);
		this._entityComponents.Add(component);

		return component;
	}

	public virtual void OnDestroy() {
	}

	public void Serialize(NetDataWriter writer) {
		writer.Put(LNHashCache.GetHash(this.GetType()));
		writer.Put(this.EntityId);
		writer.Put(this.position);
		Inventory inventory = this.GetInventory();
		Boolean hasInventory = inventory is not null;
		writer.Put(hasInventory);
		if (hasInventory) {
			writer.Put(inventory);
		}
	}

	public void Deserialize(NetDataReader reader) {
	}
}