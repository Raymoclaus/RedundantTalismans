using System;

namespace RedundantTalismans
{
	public static class ListRedundancyChecker
	{
		public static void FindRedundancies<T>(T[] redundantables, Action<T, T> onFoundRedundant) where T : IRedundantable<T>
		{
			int count = redundantables.Length;
			for (int i = 0; i < count; i++)
			{
				IRedundantable<T> item = redundantables[i];
				for (int j = 0; j < count; j++)
				{
					if (i == j) continue;
					IRedundantable<T> other = redundantables[j];
					if (item.IsRedundantTo((T)other)) onFoundRedundant?.Invoke((T)item, (T)other);
				}
			}
		}
	}
}