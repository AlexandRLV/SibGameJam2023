using System;
using System.Collections.Concurrent;

namespace NetFrame.Utils
{
	public static class MainThread
	{
		private static readonly ConcurrentQueue<Action> Tasks = new();

		public static void Run(Action actionToMainThread)
		{
			Tasks.Enqueue(actionToMainThread);
		}

		public static void Pulse()
		{
			while (Tasks.TryDequeue(out var action))
			{
				action?.Invoke();
			}
		}
	}
}