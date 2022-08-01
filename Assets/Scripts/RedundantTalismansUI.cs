using System.Collections.Generic;
using UnityEngine;

namespace RedundantTalismans
{
	public class RedundantTalismansUI : MonoBehaviour
	{
		private readonly List<RedundantTalismanUIEntry> _entries = new List<RedundantTalismanUIEntry>();
		[SerializeField] private RedundantTalismanUIEntry prefab;
		[SerializeField] private Transform holder;
		private string _currentSortedSkill;

		public RedundantTalismanUIEntry AddEntry(Talisman talisman)
		{
			RedundantTalismanUIEntry entry = Instantiate(prefab, holder);
			_entries.Add(entry);
			entry.SetRedundantTalismansUI(this);
			entry.SetRedundantTalisman(talisman);
			return entry;
		}

		public void RemoveTalismanFromAllEntries(Talisman talisman)
		{
			for (int i = _entries.Count - 1; i >= 0; i--)
			{
				RedundantTalismanUIEntry entry = _entries[i];
				entry.RemoveBetterTalisman(talisman);
			}
		}

		public void RemoveEntry(RedundantTalismanUIEntry entry)
		{
			_entries.Remove(entry);
		}

		public void ClearAllContents()
		{
			holder.DestroyAllChildren();
		}

		public void SortBySkill(string skillName)
		{
			_currentSortedSkill = skillName;
			int index = 0;

			for (int i = 0; i < _entries.Count; i++)
			{
				RedundantTalismanUIEntry entry = _entries[i];
				Talisman redundantTalisman = entry.GetRedundantTalisman();
				if (!redundantTalisman.ContainsSkillName(skillName)) continue;

				Transform entryTransform = entry.GetTransform();
				entryTransform.SetSiblingIndex(index);
				_entries.RemoveAt(i);
				_entries.Insert(index, entry);
				index++;
			}
		}

		public string GetCurrentlySortedSkill()
		{
			return _currentSortedSkill;
		}
	}
}