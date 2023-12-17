using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Entities.EntityComponents;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities;

public abstract class Entity : INetSerializable
{
	private List<BaseEntityComponent> _entityComponents = [];

	public virtual void Serialize(NetDataWriter writer)
	{
		writer.Put(this.EntityId);
		writer.Put(this.Position);
		writer.Put((byte)this.currentAnimation);
		writer.Put((Byte)this._entityComponents.Count);
		foreach (BaseEntityComponent entityComponent in this._entityComponents)
		{
			writer.PutDeserializable(entityComponent);
		}
	}

	public virtual void Deserialize(NetDataReader reader)
	{
		this.EntityId = reader.GetGuid();
		this.Position = reader.GetVector2();
		this.currentAnimation = (EAnimationType)reader.GetByte();
		Byte componentsCount = reader.GetByte();
		this._entityComponents.EnsureCapacity(componentsCount);
		for (Byte i = 0; i < componentsCount; i++)
		{
			BaseEntityComponent component = reader.GetDeserializable<BaseEntityComponent>();
			component.Owner = this;
			if (component is IClientInitializable clientInitializable)
			{
				clientInitializable.ClientInitialize();
			}

			this._entityComponents.Add(component);
		}
	}

	public Guid EntityId { get; set; }
	public virtual Vector2 Position { get; set; }
	public EAnimationType currentAnimation {get; set;}
	public virtual void Update(Single deltaTime)
	{
	}

	public virtual void Initialize(IDictionary properties)
	{
	}

	public virtual void OnDestroy()
	{
	}

	public T GetComponent<T>() where T : BaseEntityComponent
	{
		return this._entityComponents.FirstOrDefault(comp => comp is T) as T;
	}

	public virtual T AddComponent<T>(IDictionary properties) where T : BaseEntityComponent, new()
	{
		Guid componentId = Guid.NewGuid();
		T component = new T
		{
			Owner = this,
			ComponentId = componentId
		};

		component.Initialize(properties);
		this._entityComponents.Add(component);

		return component;
	}

	public BaseEntityComponent GetComponentByGUID(Guid componentGuid){
		return this._entityComponents.FirstOrDefault(c => c.ComponentId == componentGuid);
	}

	public virtual Boolean RemoveComponentById(Guid componentId)
	{
		return this.RemoveComponent(component => component.ComponentId == componentId) > 0;
	}

	public virtual Boolean RemoveComponentByType<T>() where T : BaseEntityComponent
	{
		return this.RemoveComponent(component => component is T) > 0;
	}

	public virtual Int32 RemoveComponent(Func<BaseEntityComponent, Boolean> predicate)
	{
		Int32 removed = 0;
		for (Int32 i = 0; i < this._entityComponents.Count;)
		{
			if (predicate(this._entityComponents[i]))
			{
				this._entityComponents.RemoveAt(i);
				removed++;
			}
			else
			{
				i++;
			}
		}

		return removed;
	}
}