using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Entities.EntityComponents;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities;

public abstract class Entity : INetSerializable
{
	private List<IEntityComponent> _entityComponents = [];

	public virtual void Serialize(NetDataWriter writer)
	{
		writer.Put(this.EntityId);
		writer.Put(this.Position);
		writer.Put((Byte)this._entityComponents.Count);
		foreach (IEntityComponent entityComponent in this._entityComponents)
		{
			writer.PutDeserializable(entityComponent);
		}
	}

	public virtual void Deserialize(NetDataReader reader)
	{
		this.EntityId = reader.GetGuid();
		this.Position = reader.GetVector2();
		Byte componentsCount = reader.GetByte();
		this._entityComponents.EnsureCapacity(componentsCount);
		for (Byte i = 0; i < componentsCount; i++)
		{
			IEntityComponent component = reader.GetDeserializable<IEntityComponent>();
			this._entityComponents.Add(component);
		}
	}

	public Guid EntityId { get; set; }
	public Vector2 Position { get; set; }

	public virtual void Update(Single deltaTime)
	{
	}

	public virtual void Initialize(Queue properties)
	{
	}

	public virtual void OnDestroy()
	{
	}

	public T GetComponent<T>() where T : class, IEntityComponent
	{
		return this._entityComponents.FirstOrDefault(comp => comp is T) as T;
	}

	public T AddComponent<T>(params Object[] properties) where T : IEntityComponent, new()
	{
		T component = new T
		{
			Owner = this
		};

		Queue props = new Queue(properties);
		component.Initialize(props);
		this._entityComponents.Add(component);

		return component;
	}
}