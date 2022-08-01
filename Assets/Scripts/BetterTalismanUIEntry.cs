using TMPro;
using UnityEngine;

namespace RedundantTalismans
{
	public class BetterTalismanUIEntry : MonoBehaviour
	{
		private Talisman _betterTalisman;
		[SerializeField] private TextMeshProUGUI textMesh;

		public void SetBetterTalisman(Talisman talisman)
		{
			_betterTalisman = talisman;
			textMesh.text = _betterTalisman.ToString();
		}
	}
}