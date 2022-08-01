using System;
using System.Collections.Generic;

namespace RedundantTalismans
{
	public struct Skill
	{
		public string _name;
		public uint _level;

		private const string SKILL_FORMAT = "{0} Lv{1}";

		public Skill(string name, uint level)
		{
			_name = name;
			_level = level;
		}

		public static void RemoveSkillLevel(Skill toRemove, List<Skill> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Skill skill = list[i];
				if (skill._name != toRemove._name) continue;

				uint amountToRemove = Math.Min(skill._level, toRemove._level);
				skill._level -= amountToRemove;
				if (skill._level == 0)
				{
					list.RemoveAt(i);
				}
				else
				{
					list[i] = skill;
				}

				toRemove._level -= amountToRemove;
				if (toRemove._level == 0) return;
			}
		}

		public override string ToString()
		{
			return string.Format(SKILL_FORMAT, _name, _level);
		}

		public static bool operator ==(Skill a, Skill b)
		{
			return a._name == b._name && a._level == b._level;
		}

		public static bool operator !=(Skill a, Skill b)
		{
			return !(a == b);
		}
		
		public bool Equals(Skill other)
		{
			return _name == other._name && _level == other._level;
		}

		public override bool Equals(object obj)
		{
			return obj is Skill other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_name, _level);
		}
	}
}