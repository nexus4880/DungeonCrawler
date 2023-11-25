using System.Numerics;
using DungeonCrawler.Core.Items;

namespace DungeonCrawler.Client;

public class PlayerController {
	public Single health;
	public Vector2 position;
	public Single radius;
	public List<Item> items;

	public virtual void Update() {
	}

	public virtual void Draw() {
		DrawCircleLines((Int32)this.position.X, (Int32)this.position.Y, this.radius, RED);
	}
}