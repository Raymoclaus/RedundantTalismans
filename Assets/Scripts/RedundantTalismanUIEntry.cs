using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RedundantTalismans
{
	public class RedundantTalismanUIEntry : MonoBehaviour
	{
		private Talisman _redundantTalisman;
		private readonly List<Talisman> _betterTalismans = new List<Talisman>();
		[SerializeField] private TextMeshProUGUI textMesh;
		private static BetterTalismansUI s_betterTalismanUI;
		private static RedundantTalismanUIEntry s_currentlySelected;
		[SerializeField] private Image buttonImage;
		[SerializeField] private Color selectedTint;
		private Color _normalTint;
		private RedundantTalismansUI _redundantTalismansUI;
		private Transform _transform;
		private int _sortingCounter;

		private void Start()
		{
			_normalTint = buttonImage.color;
		}

		public BetterTalismansUI GetBetterTalismanUI()
		{
			if (s_betterTalismanUI != null) return s_betterTalismanUI;

			s_betterTalismanUI = FindObjectOfType<BetterTalismansUI>();
			return s_betterTalismanUI;
		}

		public void SetRedundantTalisman(Talisman talisman)
		{
			_redundantTalisman = talisman;
			textMesh.text = _redundantTalisman.ToString();
		}

		public void SetRedundantTalismansUI(RedundantTalismansUI redundantTalismansUI)
		{
			_redundantTalismansUI = redundantTalismansUI;
		}

		public void AddBetterTalisman(Talisman talisman)
		{
			_betterTalismans.Add(talisman);
		}

		public void UpdateBetterTalismansUI()
		{
			if (GetBetterTalismanUI() == null) return;

			GetBetterTalismanUI().SetBetterTalismans(_betterTalismans);
		}

		private void ClearBetterTalismanUI()
		{
			if (s_currentlySelected != this) return;
			if (GetBetterTalismanUI() == null) return;

			Deselect();
			GetBetterTalismanUI().ClearAllContents();
		}

		public void RemoveEntry()
		{
			_redundantTalismansUI.RemoveEntry(this);
			_redundantTalismansUI.RemoveTalismanFromAllEntries(_redundantTalisman);
			ClearBetterTalismanUI();
			Destroy(gameObject);
		}

		private void RemoveEntryWithoutUpdatingRedundantTalismanUI()
		{
			_redundantTalismansUI.RemoveEntry(this);
			ClearBetterTalismanUI();
			Destroy(gameObject);
		}

		public void Select()
		{
			if (s_currentlySelected != null && s_currentlySelected != this)
			{
				s_currentlySelected.Deselect();
			}
			s_currentlySelected = this;
			UpdateBetterTalismansUI();
			buttonImage.color = selectedTint;
		}

		private void Deselect()
		{
			buttonImage.color = _normalTint;
		}

		public void RemoveBetterTalisman(Talisman talisman)
		{
			for (int i = _betterTalismans.Count - 1; i >= 0; i--)
			{
				if (talisman == _betterTalismans[i])
				{
					_betterTalismans.RemoveAt(i);
					break;
				}
			}

			if (_betterTalismans.Count == 0)
			{
				RemoveEntryWithoutUpdatingRedundantTalismanUI();
			}
			else if (s_currentlySelected == this)
			{
				UpdateBetterTalismansUI();
			}
		}

		public void PressedSortButton()
		{
			int skillCount = _redundantTalisman.GetSkillCount();
			int i = 0;
			for (; i < skillCount; i++)
			{
				if (i < _sortingCounter) continue;
				
				Skill skill = _redundantTalisman.GetSkillAtIndex(i);
				if (skill._name == _redundantTalismansUI.GetCurrentlySortedSkill()) continue;

				break;
			}
			
			_sortingCounter = i % skillCount;
			Skill selectedSkill = _redundantTalisman.GetSkillAtIndex(_sortingCounter);
			string selectedSkillName = selectedSkill._name;
			_redundantTalismansUI.SortBySkill(selectedSkillName);
		}

		public Talisman GetRedundantTalisman()
		{
			return _redundantTalisman;
		}

		public Transform GetTransform()
		{
			return _transform ? _transform : (_transform = transform);
		}
	}
}