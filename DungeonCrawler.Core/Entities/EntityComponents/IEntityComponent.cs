using System.Collections;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents;

public interface IEntityComponent : INetSerializable
{
	Entity Owner { get; init; }
	public void Initialize(Queue properties);
}