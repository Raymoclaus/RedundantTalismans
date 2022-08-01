using UnityEngine;
using TMPro;

namespace RedundantTalismans
{
	public class CalculateButton : MonoBehaviour
	{
		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private RedundantTalismansUI redundantTalismansUI;
		
		public void Calculate()
		{
			redundantTalismansUI.ClearAllContents();
			
			string inputText = inputField.text;
			inputText = inputText.TrimEnd(' ', '\n');
			Talisman[] talismans = Talisman.ImportTalismanData(inputText);
			Talisman previousTalisman = default;
			RedundantTalismanUIEntry previousEntry = null;
			ListRedundancyChecker.FindRedundancies(talismans, (worse, better) =>
			{
				if (worse == previousTalisman)
				{
					if (previousEntry != null) previousEntry.AddBetterTalisman(better);
					return;
				}

				RedundantTalismanUIEntry entry = redundantTalismansUI.AddEntry(worse);
				entry.AddBetterTalisman(better);
				previousTalisman = worse;
				previousEntry = entry;
			});
		}
	}
}