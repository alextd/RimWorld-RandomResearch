using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;

namespace Random_Research
{
	[HarmonyPatch(typeof(MainTabWindow_Research), "PreOpen")]
	class HideCurrentResearch
	{
		//public override void PreOpen()
		public static void Postfix(MainTabWindow_Research __instance)
		{
			AccessTools.Field(typeof(MainTabWindow_Research), "selectedProject").SetValue(__instance, null);
			//__instance.selectedProject = null;
			AccessTools.Property(typeof(MainTabWindow_Research), "CurTab").GetSetMethod(true).Invoke(__instance, new object[] { ResearchTabDefOf.Main });
			//__instance.CurTab = ResearchTabDefOf.Main;
		}
	}
}
