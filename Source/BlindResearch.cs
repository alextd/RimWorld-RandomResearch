﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Verse;
using RimWorld;
using Harmony;

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
			return !Active() || CanSeeProgress(SelectedResearch().ProgressPercent) || DebugSettings.godMode;
		}

		public static FieldInfo selectedInfo = AccessTools.Field(typeof(MainTabWindow_Research), "selectedProject");
		public static ResearchProjectDef SelectedResearch()
		{
			if(Find.MainTabsRoot?.OpenTab?.TabWindow is MainTabWindow_Research res)
				 return selectedInfo.GetValue(res) as ResearchProjectDef;
			return null;
		}
	}
}
