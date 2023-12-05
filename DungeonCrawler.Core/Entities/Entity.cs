using System.Collections;
using System.Numerics;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities;

public interface IEntity : INetSerializable {
	public Guid EntityId { get; set; }
	public Vector2 Position { get; set; }

	void Update(Single deltaTime);
	void Initialize(Stack properties);
	void OnDestroy();
}