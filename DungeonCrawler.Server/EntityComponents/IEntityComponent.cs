using System.Collections;
using DungeonCrawler.Server.Entities;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.EntityComponents; 

public interface IEntityComponent : INetSerializable {
	Entity Owner { get; init; }
	public void Initialize(Stack properties);
}