using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using HarmonyLib;

namespace Random_Research
{
	[HarmonyPatch(typeof(ResearchManager), nameof(ResearchManager.ResearchPerformed))]
	static class Letter_ResearchKnown
	{
		public static void Prefix(ResearchManager __instance, ref bool __state)
		{
			__state = BlindResearch.CanSeeProgress(__instance.currentProj?.ProgressPercent ?? 0);
		}
		public static int msgNum = 0;
		public const int msgCount = 3;
		public static Letter lastLetter;
		public static void RemoveLetter()
		{
			if (lastLetter != null)
			{
				Find.LetterStack.RemoveLetter(lastLetter);
				lastLetter = null;
			}
		}
		public static void Postfix(ResearchManager __instance, bool __state)
		{
			if (!__state && BlindResearch.CanSeeProgress(__instance.currentProj?.ProgressPercent ?? 0))
			{
				RemoveLetter();

				int msg = msgNum++;
				if (msgNum == msgCount) msgNum = 0;
				string letter = "TD.ResearchKnown".Translate(__instance.currentProj.LabelCap);
				string text = $"TD.ResearchKnownMsg{msg}".Translate(__instance.currentProj.LabelCap) +
					"\n\n" + "TD.ResearchKnownDesc".Translate() + 
					"\n\n" + __instance.currentProj.LabelCap+": "+__instance.currentProj.description;

				lastLetter = LetterMaker.MakeLetter(letter, text, LetterDefOf.NeutralEvent);
				Find.LetterStack.ReceiveLetter(lastLetter);
			}
		}
	}

	[HarmonyPatch(typeof(ResearchManager), nameof(ResearchManager.FinishProject))]
	public static class CloseLetterFinished
	{
		public static void Prefix()
		{
			Letter_ResearchKnown.RemoveLetter();
		}
	}

	[HarmonyPatch(typeof(MainTabWindow_Research), nameof(MainTabWindow_Research.PreOpen))]
	public static class CloseLetterResearchTab
	{
		public static void Prefix()
		{
			Letter_ResearchKnown.RemoveLetter();
		}
	}
}
