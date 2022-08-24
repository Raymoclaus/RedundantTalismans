using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedundantTalismans
{
    public readonly struct Talisman : IRedundantable<Talisman>
    {
        private readonly Skill[] _skills;
        private readonly Slot[] _slots;
        
        private const string SKILL_SEPARATOR = ", ",
            SKILL_TO_SLOT_SEPARATOR = " | ",
            SLOT_SEPARATOR = ", ",
            SKILLS_STARTER = "",
            SLOTS_STARTER = "Slots: ";

        public Talisman(Skill[] skills, Slot[] slots)
        {
            _skills = skills;
            _slots = slots;
        }

        public Talisman Copy()
        {
            Skill[] skills = new Skill[_skills.Length];
            Array.Copy(_skills, skills, _skills.Length);
            Slot[] slots = new Slot[_slots.Length];
            Array.Copy(_slots, slots, _slots.Length);

            return new Talisman(skills, slots);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="talismanData">Expected format: "skill1Name,skill1Level,skill2Name,skill2Level,slot1Level,slot2Level,slot3Level"
        /// Example: Bubbly Dance,3,Partbreaker,2,1,0,0</param>
        private static Talisman ConvertFromString(string talismanData)
        {
            string[] data = talismanData.Split(',');
            if (data.Length != 7) return default;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i].TrimEnd(' ', '\n');
            }

            List<Skill> skills = new List<Skill>();
            string firstSkillName = data[0];
            uint.TryParse(data[1], out uint firstSkillLevel);
            if (!string.IsNullOrEmpty(firstSkillName) && firstSkillLevel > 0)
            {
                skills.Add(new Skill(firstSkillName, firstSkillLevel));
            }
            string secondSkillName = data[2];
            uint.TryParse(data[3], out uint secondSkillLevel);
            if (!string.IsNullOrEmpty(secondSkillName) && secondSkillLevel > 0)
            {
                skills.Add(new Skill(secondSkillName, secondSkillLevel));
            }

            List<Slot> slots = new List<Slot>();
            uint.TryParse(data[4], out uint firstSlotLevel);
            if (firstSlotLevel > 0)
            {
                slots.Add(new Slot(firstSlotLevel));
            }
            uint.TryParse(data[5], out uint secondSlotLevel);
            if (secondSlotLevel > 0)
            {
                slots.Add(new Slot(secondSlotLevel));
            }
            uint.TryParse(data[6], out uint thirdSlotLevel);
            if (thirdSlotLevel > 0)
            {
                slots.Add(new Slot(thirdSlotLevel));
            }

            Talisman talisman = new Talisman(skills.ToArray(), slots.ToArray());
            return talisman;
        }

        public static Talisman[] ImportTalismanData(string data)
        {
            string[] talismansData = data.Split('\n');
            int talismansDataLength = talismansData.Length;
            Talisman[] talismans = new Talisman[talismansDataLength];
            for (int i = 0; i < talismansDataLength; i++)
            {
                talismans[i] = ConvertFromString(talismansData[i]);
            }

            return talismans;
        }

        public bool IsRedundantTo(Talisman other)
        {
            Talisman otherCopy = other.Copy().AddDecorationsToEncompass(this);
            
            bool hasMoreSkills = HasMoreSkills(otherCopy, true);
            if (hasMoreSkills) return false;

            bool hasMoreSlots = HasMoreSlots(otherCopy, false);
            if (hasMoreSlots) return false;

            bool hasDifferentSkills = !HasSameNameSkills(otherCopy, true);
            if (hasDifferentSkills) return false;

            bool hasHigherSkillLevels = HasHigherSkillLevels(otherCopy, true);
            if (hasHigherSkillLevels) return false;

            bool hasHigherTierSlot = HasHigherSlotLevels(otherCopy, false);
            if (hasHigherTierSlot) return false;

            bool hasHighestCombinedSlotLevels = HasHighestCombinedSlotLevels(otherCopy, false);
            return !hasHighestCombinedSlotLevels;
        }

        private Talisman AddDecorationsToEncompass(Talisman other)
        {
            List<Skill> missingSkills = GetSkillDifferences(other);
            missingSkills.Sort((a, b) =>
            {
                uint aMinSlotLevel = DecorationData.GetMinimumSlotLevel(a._name);
                uint bMinSlotLevel = DecorationData.GetMinimumSlotLevel(b._name);
                return aMinSlotLevel.CompareTo(bMinSlotLevel);
            });

            for (int i = missingSkills.Count - 1; i >= 0; i--)
            {
                int missingSkillsCount = missingSkills.Count;
                bool missingSkillFulfilled;
                do
                {
                    uint highestTierSlot = HighestSlotLevel(false);
                    if (highestTierSlot == 0) return this;
                    
                    Decoration matchingDeco = DecorationData.GetMatchingDecoration(missingSkills[i], highestTierSlot);
                    if (matchingDeco == default) break;
                    int matchingSlotIndex = GetIndexOfSlotWithAtLeastGivenLevel(matchingDeco._level);
                    _slots[matchingSlotIndex]._decoration = matchingDeco;
                    Skill.RemoveSkillLevel(matchingDeco._skill, missingSkills);
                    missingSkillFulfilled = missingSkillsCount != missingSkills.Count;
                } while (!missingSkillFulfilled);
            }

            return this;
        }

        private List<Skill> GetSkillDifferences(Talisman other)
        {
            List<Skill> differentSkills = new List<Skill>();
            
            foreach (Skill otherSkill in other._skills)
            {
                Skill currentSkill = GetSkill(otherSkill._name);
                if (currentSkill == default)
                {
                    differentSkills.Add(otherSkill);
                    continue;
                }

                if (otherSkill._level > currentSkill._level)
                {
                    uint levelDifference = otherSkill._level - currentSkill._level;
                    differentSkills.Add(new Skill(otherSkill._name, levelDifference));
                }
            }
            
            return differentSkills;
        }

        private bool HasHighestCombinedSlotLevels(Talisman other, bool includeSlotsWithDecorations = true)
        {
            uint combinedSlotLevels = CombinedSlotLevels(includeSlotsWithDecorations);
            uint otherCombinedSlotLevels = other.CombinedSlotLevels(includeSlotsWithDecorations);
            return combinedSlotLevels > otherCombinedSlotLevels;
        }

        private uint CombinedSlotLevels(bool includeSlotsWithDecorations = true)
        {
            uint total = 0;
            
            foreach (Slot slot in _slots)
            {
                if (!includeSlotsWithDecorations && slot._decoration != default) continue;
                total += slot._level;
            }

            return total;
        }

        private bool HasMoreSlots(Talisman other, bool includeSlotsWithDecorations = true)
        {
            int slotCount = GetSlotCount(includeSlotsWithDecorations);
            int otherSlotCount = other.GetSlotCount(includeSlotsWithDecorations);
            return slotCount > otherSlotCount;
        }

        private int GetSlotCount(bool includeSlotsWithDecorations = true)
        {
            if (includeSlotsWithDecorations) return _slots.Length;

            return _slots.Count(s => !s.ContainsDecoration());
        }

        private bool HasMoreSkills(Talisman other, bool includeDecorations = false)
        {
            int skillCount = GetSkillCount(includeDecorations);
            int otherSkillCount = other.GetSkillCount(includeDecorations);
            return skillCount > otherSkillCount;
        }

        public int GetSkillCount(bool includeDecorations = false)
        {
            return GetSkills(includeDecorations).Count;
        }

        private List<Skill> GetSkills(bool includeDecorations = false)
        {
            List<Skill> skills = new List<Skill>();
            foreach (Skill skill in _skills)
            {
                int indexOfSkill = skills.FindIndex(s => s._name == skill._name);
                if (indexOfSkill == -1)
                {
                    skills.Add(skill);
                }
                else
                {
                    Skill indexedSkill = skills[indexOfSkill];
                    skills[indexOfSkill] = new Skill(indexedSkill._name, indexedSkill._level + skill._level);
                }
            }

            if (!includeDecorations) return skills;

            foreach (Slot slot in _slots)
            {
                Skill skill = slot._decoration._skill;
                if (skill._name == null) continue;
                
                int indexOfSkill = skills.FindIndex(s => s._name == skill._name);
                if (indexOfSkill == -1)
                {
                    skills.Add(skill);
                }
                else
                {
                    Skill indexedSkill = skills[indexOfSkill];
                    skills[indexOfSkill] = new Skill(indexedSkill._name, indexedSkill._level + skill._level);
                }
            }

            return skills;
        }

        private int GetIndexOfNthSlot(int n, bool includeSlotsWithDecorations = true)
        {
            int count = 0;
            
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].ContainsDecoration() && !includeSlotsWithDecorations) continue;

                if (count == n) return i;

                count++;
            }

            return -1;
        }

        private bool HasHigherSlotLevels(Talisman other, bool includeSlotsWithDecorations = true)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                int currentSlotIndex = GetIndexOfNthSlot(i, false);
                int otherSlotIndex = other.GetIndexOfNthSlot(i, false);

                if (currentSlotIndex == -1 && otherSlotIndex == -1) return false;

                if (otherSlotIndex == -1) return true;

                uint currentSlotLevel = _slots[currentSlotIndex]._level;
                uint otherSlotLevel = other._slots[otherSlotIndex]._level;

                if (currentSlotLevel > otherSlotLevel) return true;
            }

            return false;
        }

        private uint HighestSlotLevel(bool includeSlotsWithDecorations = true)
        {
            uint highest = 0;
            foreach (Slot slot in _slots)
            {
                if (!includeSlotsWithDecorations && slot.ContainsDecoration()) continue;
                
                highest = Math.Max(highest, slot._level);
            }

            return highest;
        }

        private int GetIndexOfSlotWithAtLeastGivenLevel(uint matchingDecoLevel, bool includeSlotsWithDecorations = true)
        {
            uint lowest = uint.MaxValue;
            int index = -1;
            
            for (int i = 0; i < _slots.Length; i++)
            {
                Slot slot = _slots[i];
                if (!includeSlotsWithDecorations && slot.ContainsDecoration()) continue;

                if (slot._level >= matchingDecoLevel && slot._level < lowest)
                {
                    lowest = slot._level;
                    index = i;
                }
            }

            return index;
        }

        private int GetIndexOfHighestSlotLevel(bool includeSlotsWithDecorations = true)
        {
            uint highest = 0;
            int index = -1;

            for (uint i = 0; i < _slots.Length; i++)
            {
                Slot slot = _slots[i];
                if (!includeSlotsWithDecorations && slot.ContainsDecoration()) continue;

                if (slot._level > highest)
                {
                    highest = slot._level;
                    index = (int)i;
                }
            }

            return index;
        }

        private bool HasHigherSkillLevels(Talisman other, bool includeDecorations = false)
        {
            List<Skill> skills = GetSkills(includeDecorations);
            
            foreach (Skill skill in skills)
            {
                if (skill._level > other.GetSkill(skill._name, includeDecorations)._level) return true;
            }

            return false;
        }

        private Skill GetSkill(string skillName, bool includeDecorations = false)
        {
            Skill totalSkill = default;
            
            foreach (Skill skill in _skills)
            {
                if (skill._name == skillName)
                {
                    totalSkill._level += skill._level;
                }
            }

            if (!includeDecorations) return totalSkill;

            foreach (Slot slot in _slots)
            {
                Skill skill = slot._decoration._skill;
                if (skill._name == skillName)
                {
                    totalSkill._level += skill._level;
                }
            }

            return totalSkill;
        }

        private bool HasSameNameSkills(Talisman other, bool includeDecorations = false)
        {
            List<Skill> skills = GetSkills(includeDecorations);
            
            foreach (Skill skill in skills)
            {
                if (!other.ContainsSkillName(skill._name, includeDecorations)) return false;
            }

            return true;
        }

        public bool ContainsSkillName(string skillName, bool includeDecorations = false)
        {
            if (_skills.Any(skill => skill._name == skillName)) return true;

            if (!includeDecorations) return false;

            return _slots.Any(slot => slot._decoration._skill._name == skillName);
        }

        public Skill GetSkillAtIndex(int index)
        {
            return _skills[index];
        }

        public override string ToString()
        {
            string s = SKILLS_STARTER;
            int skillCount = _skills.Length;
            for (int i = 0; i < skillCount; i++)
            {
                s += _skills[i].ToString();
                if (i < skillCount - 1) s += SKILL_SEPARATOR;
            }

            int slotCount = _slots.Length;
            if (slotCount == 0) return s;

            s += SKILL_TO_SLOT_SEPARATOR;
            s += SLOTS_STARTER;
            for (int i = 0; i < slotCount; i++)
            {
                s += _slots[i].ToString();
                if (i < slotCount - 1) s += SLOT_SEPARATOR;
            }

            return s;
        }

        public static bool operator ==(Talisman a, Talisman b)
        {
            bool aSkillsArrayIsNull = a._skills == null;
            bool bSkillsArrayIsNull = b._skills == null;
            bool skillsArrayStateIsNotEqual = aSkillsArrayIsNull != bSkillsArrayIsNull;
            if (skillsArrayStateIsNotEqual) return false;

            bool aSlotsArrayIsNull = a._slots == null;
            bool bSlotsArrayIsNull = b._slots == null;
            bool slotsArrayStateIsNotEqual = aSlotsArrayIsNull != bSlotsArrayIsNull;
            if (slotsArrayStateIsNotEqual) return false;

            if (!aSkillsArrayIsNull)
            {
                bool hasDifferentSkillCount = a._skills.Length != b._skills.Length;
                if (hasDifferentSkillCount) return false;

                for (int i = 0; i < a._skills.Length; i++)
                {
                    if (a._skills[i] != b._skills[i]) return false;
                }
            }

            if (aSlotsArrayIsNull) return true;
            
            bool hasDifferentSlotCount = a._slots.Length != b._slots.Length;
            if (hasDifferentSlotCount) return false;

            bool hasDifferentSlotLevels = HasDifferentSlotLevels(a, b);
            if (hasDifferentSlotLevels) return false;

            return true;
        }

        private static bool HasDifferentSlotLevels(Talisman a, Talisman b)
        {
            for (int i = 0; i < a._slots.Length; i++)
            {
                if (a._slots[i]._level != b._slots[i]._level) return true;
            }

            return false;
        }

        public static bool operator !=(Talisman a, Talisman b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Talisman other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_skills, _slots);
        }
    }
}
