using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Random_Research
{
	public static class BlindResearch
	{
		public static bool Active()
		{
			return Find.Scenario.AllParts.Any(p => p is ScenPart_RandomResearch);
		}

		public static bool CanSeeCurrent()
		{
			return CanSeeProgress(Find.ResearchManager.currentProj?.ProgressPercent ?? 0);
		}

		public static bool CanSeeProgress(float progress)
		{
			return !Active() || DebugSettings.godMode || progress >= Find.Scenario.AllParts.Sum(p => p is ScenPart_RandomResearch rr ? rr.blindThreshold: 0);
		}

		public static bool CanChangeCurrent()
		{
			return !Active() || DebugSettings.godMode;
		}
	}
}
