using System.Collections;

namespace DungeonCrawler.Core.Extensions;

public static class StackExtensions {
	public static T PopValue<T>(this Stack stack, T defaultValue = default) {
		if (stack.Count == 0) {
			return defaultValue;
		}

		return (T)stack.Pop();
	}
}