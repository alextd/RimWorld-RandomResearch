using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Verse;
using RimWorld;
using HarmonyLib;

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

		public static bool CanChangeTo(ResearchProjectDef toThis = null)
		{
			return !Active() || CanSeeProgress(toThis?.ProgressPercent ?? 0) || DebugSettings.godMode;
		}

		public static AccessTools.FieldRef<MainTabWindow_Research, ResearchProjectDef> selectedProject = AccessTools.FieldRefAccess<MainTabWindow_Research, ResearchProjectDef>("selectedProject");
		public static ResearchProjectDef SelectedResearch()
		{
			if (Find.MainTabsRoot?.OpenTab?.TabWindow is MainTabWindow_Research res)
				return selectedProject(res);
			return null;
		}
	}
}
