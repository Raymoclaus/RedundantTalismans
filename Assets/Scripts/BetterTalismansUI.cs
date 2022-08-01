using System.Collections.Generic;
using UnityEngine;

namespace RedundantTalismans
{
	public class BetterTalismansUI : MonoBehaviour
	{
		[SerializeField] private BetterTalismanUIEntry betterTalismanPrefab;
		[SerializeField] private Transform betterTalismanHolder;
		
		public void SetBetterTalismans(List<Talisman> talismans)
		{
			ClearAllContents();

			foreach (Talisman talisman in talismans)
			{
				BetterTalismanUIEntry entry = Instantiate(betterTalismanPrefab, betterTalismanHolder);
				entry.SetBetterTalisman(talisman);
			}
		}

		public void ClearAllContents()
		{
			betterTalismanHolder.DestroyAllChildren();
		}
	}
}