using UnityEngine;

namespace RedundantTalismans
{
	public static class TransformExtensions
	{
		public static void DestroyAllChildren(this Transform parent)
		{
			foreach (Transform child in parent)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}
}