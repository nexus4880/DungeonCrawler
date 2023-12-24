namespace DungeonCrawler.Core.Helpers;

public static class StringHelper {
	public static ulong HashString(string value) {
		return value.Aggregate(14695981039346656037, (current, num2) => (current ^ num2) * 1099511628211UL);
	}
}