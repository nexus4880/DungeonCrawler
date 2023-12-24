using System.Collections;

namespace DungeonCrawler.Core.Extensions;

public static class DictionaryExtensions {
	public static T GetValueAs<T>(this IDictionary dictionary, String key, T defaultValue = default) {
		return dictionary.Contains(key) ? (T)dictionary[key] : defaultValue;
	}

	public static T GetValueAsOrThrow<T>(this IDictionary dictionary, String key) {
		return !dictionary.Contains(key) ? throw new Exception($"Key '{key}' not found in dictionary") : (T)dictionary[key];
	}

	public static T? GetValueAsOrNull<T>(this IDictionary dictionary, String key) where T: struct {
		return !dictionary.Contains(key) ? null : (T)dictionary[key];
	}
}