using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedundantTalismans
{
	public struct Decoration
	{
		public readonly string _name;
		public readonly uint _level;
		public Skill _skill;

		private const string STRING_FORMAT = "{0}: {1}";

		public Decoration(string name, uint level, Skill skill)
		{
			_name = name;
			_level = level;
			_skill = skill;
		}

		private static Decoration ConvertFromString(string name, string skillString)
		{
			string[] nameData = name.Split(' ');
			uint.TryParse(nameData[^1], out uint level);
			string[] skillData = skillString.Split(" Lv ");
			if (skillData.Length < 2)
			{
				Debug.Log($"{name}, {skillString}");
			}
			uint.TryParse(skillData[1], out uint skillLevel);
			Skill skill = new Skill(skillData[0], skillLevel);
			return new Decoration(name, level, skill);
		}

		public static Dictionary<string, DecorationGroup> ImportDecorationData(string data)
		{
			data = data.Replace("\t", "").Replace("\r", "");
			string[] separatedData = data.Split('\n');
			for (int i = 0; i < separatedData.Length; i++)
			{
				separatedData[i] = separatedData[i].TrimStart(' ').TrimEnd(' ');
			}
			int tripleLineCount = separatedData.Length / 3;
			Dictionary<string, DecorationGroup> decorations = new Dictionary<string, DecorationGroup>();
			for (int i = 0; i < tripleLineCount; i++)
			{
				int dataIndex = i * 3;
				Decoration decoration = ConvertFromString(separatedData[dataIndex], separatedData[dataIndex + 1]);
				string skillName = decoration._skill._name;
				if (decorations.ContainsKey(skillName))
				{
					decorations[skillName].AddDecoration(decoration);
				}
				else
				{
					DecorationGroup decorationGroup = new DecorationGroup(skillName);
					decorationGroup.AddDecoration(decoration);
					decorations.Add(skillName, decorationGroup);
				}
			}

			return decorations;
		}

		public override string ToString()
		{
			return string.Format(STRING_FORMAT, _name, _skill);
		}

		public static bool operator ==(Decoration a, Decoration b)
		{
			bool aNameIsNull = a._name == null;
			bool bNameIsNull = b._name == null;
			if (aNameIsNull != bNameIsNull) return false;

			bool hasDifferentNames = a._name != b._name;
			if (hasDifferentNames) return false;

			bool hasDifferentLevels = a._level != b._level;
			if (hasDifferentLevels) return false;

			bool hasDifferentSkills = a._skill != b._skill;
			if (hasDifferentSkills) return false;

			return true;
		}

		public static bool operator !=(Decoration a, Decoration b)
		{
			return !(a == b);
		}
		
		public bool Equals(Decoration other)
		{
			return _name == other._name && _level == other._level && _skill.Equals(other._skill);
		}

		public override bool Equals(object obj)
		{
			return obj is Decoration other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_name, _level, _skill);
		}
	}
}