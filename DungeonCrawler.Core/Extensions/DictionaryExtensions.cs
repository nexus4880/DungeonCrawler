using System.Collections;

namespace DungeonCrawler.Core.Extensions;

public static class DictionaryExtensions
{
	public static T GetValueAs<T>(this IDictionary dictionary, String key, T defaultValue = default)
	{
		if (dictionary.Contains(key))
		{
			return (T)dictionary[key];
		}

		return defaultValue;
	}

	public static T GetValueAsOrThrow<T>(this IDictionary dictionary, String key)
	{
		if (!dictionary.Contains(key))
		{
			throw new Exception($"Key '{key}' not found in dictionary");
		}

		return (T)dictionary[key];
	}
}