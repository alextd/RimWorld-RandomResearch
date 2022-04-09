using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Random_Research.ResearchTreeSupport
{
	class CompletionLetter
	{
		[HarmonyPatch(typeof(ResearchManager), "FinishProject")]
		[HarmonyBefore(new string[] { "Fluffy.ResearchTree", "rimworld.ResearchPal" })]
		public class DoCompletionDialog
		{
			//Restore completion letter when there's no queue
			//DoCompletionLetter should probably go in ResearchPal inside TryStartNext but who wants to transpile that or tell them to fix it
			//public void FinishProject(ResearchProjectDef proj, bool doCompletionDialog = false, Pawn researcher = null)
			static void Prefix(ResearchProjectDef proj, bool doCompletionDialog)
			{
				if (!doCompletionDialog || !BlindResearch.Active()) return;
				try { DoCompletionDialogEx(proj); } catch (Exception) { }
			}

			//        private static void DoCompletionLetter( ResearchProjectDef current, ResearchProjectDef next )

			public static Delegate DoCompletionLetter;
			static DoCompletionDialog()
			{
				MethodInfo CompletionLetterInfo = AccessTools.Method(AccessTools.TypeByName("ResearchPal.Queue"), "DoCompletionLetter");
				if (CompletionLetterInfo == null)
					CompletionLetterInfo = AccessTools.Method(AccessTools.TypeByName("FluffyResearchTree.Queue"), "DoCompletionLetter");

				if (CompletionLetterInfo == null) return;

				DoCompletionLetter = CompletionLetterInfo.CreateDelegate(typeof(Action<ResearchProjectDef, ResearchProjectDef>));
			}
			static void DoCompletionDialogEx(ResearchProjectDef proj)
			{
				DoCompletionLetter.DynamicInvoke(proj, null);
			}
		}
	}
}
