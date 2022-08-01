namespace RedundantTalismans
{
	public struct Slot
	{
		public uint _level;
		public Decoration _decoration;

		private const string NO_DECORATION_FORMAT = "{0}",
			WITH_DECORATION_FORMAT = "{0} ({1})";

		public Slot(uint level, Decoration decoration = default)
		{
			_level = level;
			_decoration = decoration;
		}

		public override string ToString()
		{
			if (ContainsDecoration())
			{
				return string.Format(WITH_DECORATION_FORMAT, _level, _decoration.ToString());
			}

			return string.Format(NO_DECORATION_FORMAT, _level);
		}

		public bool ContainsDecoration()
		{
			return _decoration != default;
		}
	}
}