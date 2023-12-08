using System.Collections;

namespace DungeonCrawler.Core.Extensions;

public static class QueueExtensions
{
	public static T PopValue<T>(this Queue queue, T defaultValue = default)
	{
		if (queue.Count == 0)
		{
			return defaultValue;
		}

		return (T)queue.Dequeue();
	}

	public static T PopValueOrThrow<T>(this Queue queue)
	{
		if (queue.Count == 0)
		{
			throw new Exception($"Expected {typeof(T)} in empty queue");
		}

		return (T)queue.Dequeue();
	}
}