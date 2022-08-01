using System.Collections.Generic;

namespace RedundantTalismans
{
	public struct DecorationGroup
	{
		public string _skillName;
		public List<Decoration> _decorations;

		public DecorationGroup(string skillName)
		{
			_skillName = skillName;
			_decorations = new List<Decoration>();
		}

		public void AddDecoration(Decoration decoration)
		{
			for (int i = 0; i < _decorations.Count; i++)
			{
				if (_decorations[i]._level > decoration._level)
				{
					_decorations.Insert(i, decoration);
					return;
				}
			}
			
			_decorations.Add(decoration);
		}

		public Decoration GetMatchingDecoration(uint slotLevel, uint skillLevel)
		{
			Decoration chosenDeco = default;
			
			for (int i = _decorations.Count - 1; i >= 0; i--)
			{
				Decoration deco = _decorations[i];
				if (deco._level > slotLevel) continue;
				chosenDeco = deco;
				if (deco._skill._level <= skillLevel) break;
			}

			return chosenDeco;
		}

		public uint GetMinimumDecorationLevel() => _decorations[0]._level;
	}
}