using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Random_Research
{
	public static class BlindResearch
	{
		public static bool CanSeeCurrent()
		{
			return DebugSettings.godMode;
		}
	}
}
