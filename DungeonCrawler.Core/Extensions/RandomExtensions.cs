using System.Numerics;

namespace DungeonCrawler.Core.Extensions;

public static class RandomExtensions {
	public static Single NextSingleValue(this Random random, Single min, Single max) {
		return random.NextSingle() * (max - min) + min;
	}

	public static Vector2 NextVector2(this Random random, Vector2 min, Vector2 max) {
		Single x = random.NextSingleValue(min.X, max.X);
		Single y = random.NextSingleValue(min.Y, max.Y);

		return new Vector2(x, y);
	}
}