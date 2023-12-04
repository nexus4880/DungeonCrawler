﻿using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Server.EntityComponents;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

public class Entity : IEntity {
	private List<IEntityComponent> _entityComponents = [];

	public virtual void Serialize(NetDataWriter writer) {
		writer.Put(this.EntityId);
		writer.Put(this.Position);
		writer.Put((Byte)this._entityComponents.Count);
		foreach (IEntityComponent entityComponent in this._entityComponents) {
			writer.PutDeserializable(entityComponent);
		}
	}

	public virtual void Deserialize(NetDataReader reader) {
		this.EntityId = reader.GetGuid();
		this.Position = reader.GetVector2();
	}

	public Guid EntityId { get; set; }
	public Vector2 Position { get; set; }

	public virtual void Update(Single deltaTime) {
	}

	public virtual void Initialize(Stack properties) {
	}

	public virtual void OnDestroy() {
	}

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
}