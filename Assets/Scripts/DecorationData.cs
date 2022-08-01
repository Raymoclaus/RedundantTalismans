using System.Collections.Generic;
using UnityEngine;

namespace RedundantTalismans
{
	public static class DecorationData
	{
		private static Dictionary<string, DecorationGroup> s_decorations;
		private static bool s_parsed;

		private const string DECORATION_LIST_FILE_NAME = "Decoration List";

		private static void ParseDecorationList()
		{
			if (s_parsed) return;

			string decoList = Resources.Load<TextAsset>(DECORATION_LIST_FILE_NAME).text;
			s_decorations = Decoration.ImportDecorationData(decoList);
			s_parsed = true;
		}

		public static Decoration GetMatchingDecoration(Skill skill, uint slotLevel)
		{
			ParseDecorationList();

			string skillName = skill._name;
			if (s_decorations.ContainsKey(skillName))
			{
				return s_decorations[skillName].GetMatchingDecoration(slotLevel, skill._level);
			}

			return default;
		}

		public static uint GetMinimumSlotLevel(string skillName)
		{
			ParseDecorationList();
			
			if (s_decorations.ContainsKey(skillName))
			{
				return s_decorations[skillName].GetMinimumDecorationLevel();
			}

			return 0;
		}
	}
}