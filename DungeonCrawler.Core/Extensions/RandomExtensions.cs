using System.Numerics;

namespace DungeonCrawler.Core.Extensions;

public static class RandomExtensions {
	public static Vector2 NextVector2(this Random random, Vector2 min, Vector2 max) {
		Single x = random.NextSingle() * (max.X - min.X) + min.X;
		Single y = random.NextSingle() * (max.Y - min.Y) + min.Y;

		return new Vector2(x, y);
	}
}