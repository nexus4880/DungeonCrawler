using System.Collections;
using DungeonCrawler.Server.Entities;

namespace DungeonCrawler.Server.EntityComponents; 

public interface IEntityComponent {
	Entity Owner { get; init; }
	public void Initialize(Stack properties);
}