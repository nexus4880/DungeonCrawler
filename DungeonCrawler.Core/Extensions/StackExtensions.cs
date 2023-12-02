using System.Collections;

namespace DungeonCrawler.Core.Extensions;

public static class StackExtensions {
	public static T PopValue<T>(this Stack stack, T defaultValue = default) {
		if (stack.Count == 0) {
			return defaultValue;
		}

		return (T)stack.Pop();
	}

	public static T PopValueOrThrow<T>(this Stack stack) {
		if (stack.Count == 0) {
			throw new Exception($"Expected {typeof(T)} in empty stack");
		}

		return (T)stack.Pop();
	}
}