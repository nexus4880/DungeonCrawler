using System.Collections;
using System.Numerics;
using System.Reflection;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities;

public abstract class Entity : INetSerializable {
	private List<BaseEntityComponent> _entityComponents = [];

	public virtual void Serialize(NetDataWriter writer) {
		writer.Put(this.EntityId);
		writer.Put(this.Position);
		var entityComponents = this._entityComponents.Where(component => component.GetType().GetCustomAttribute<NetworkIgnoreAttribute>() is null).ToArray();
		writer.Put((byte)entityComponents.Length);
		foreach (var entityComponent in entityComponents) {
			writer.PutDeserializable(entityComponent);
		}
	}

	public virtual void Deserialize(NetDataReader reader) {
		this.EntityId = reader.GetGuid();
		this.Position = reader.GetVector2();
		var componentsCount = reader.GetByte();
		_ = this._entityComponents.EnsureCapacity(componentsCount);
		for (byte i = 0; i < componentsCount; i++) {
			var component = reader.GetDeserializable<BaseEntityComponent>();
			component.Owner = this;
			if (component is IClientInitializable clientInitializable) {
				clientInitializable.ClientInitialize();
			}

			this._entityComponents.Add(component);
		}
	}

	public Guid EntityId { get; set; }
	public virtual Vector2 Position { get; set; }

	public virtual void Update(float deltaTime) {
	}

	public virtual void Initialize(IDictionary properties) {
	}

	public virtual void OnDestroy() {
	}

	public T GetComponent<T>() where T : BaseEntityComponent {
		return this._entityComponents.FirstOrDefault(comp => comp is T) as T;
	}

	public virtual T AddComponent<T>(IDictionary properties = null) where T : BaseEntityComponent, new() {
		var componentId = Guid.NewGuid();
		var component = new T {
			Owner = this,
			ComponentId = componentId
		};

		component.Initialize(properties);
		this._entityComponents.Add(component);

		return component;
	}

	public BaseEntityComponent GetComponentByGUID(Guid componentGuid) {
		return this._entityComponents.FirstOrDefault(c => c.ComponentId == componentGuid);
	}

	public virtual bool RemoveComponentById(Guid componentId) {
		return this.RemoveComponent(component => component.ComponentId == componentId) > 0;
	}

	public virtual bool RemoveComponentByType<T>() where T : BaseEntityComponent {
		return this.RemoveComponent(component => component is T) > 0;
	}

	public virtual int RemoveComponent(Func<BaseEntityComponent, bool> predicate) {
		var removed = 0;
		for (var i = 0; i < this._entityComponents.Count;) {
			if (predicate(this._entityComponents[i])) {
				this._entityComponents.RemoveAt(i);
				removed++;
			}
			else {
				i++;
			}
		}

		return removed;
	}
}