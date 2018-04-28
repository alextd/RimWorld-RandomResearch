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
			return DebugSettings.godMode ||
				Find.ResearchManager.currentProj?.ProgressPercent >= 0.5f;
		}

		public static bool CanChangeCurrent()
		{
			return DebugSettings.godMode;
		}
	}
}
