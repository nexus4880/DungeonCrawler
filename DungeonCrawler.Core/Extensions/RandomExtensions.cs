using System.Numerics;

namespace DungeonCrawler.Core.Extensions;

public static class RandomExtensions {
	public static float NextSingleValue(this Random random, float min, float max) {
		return (random.NextSingle() * (max - min)) + min;
	}

	public static Vector2 NextVector2(this Random random, Vector2 min, Vector2 max) {
		var x = random.NextSingleValue(min.X, max.X);
		var y = random.NextSingleValue(min.Y, max.Y);

		return new Vector2(x, y);
	}
}