using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace Random_Research.ResearchPal
{
	class CompletionLetter
	{
		[HarmonyPatch(typeof(ResearchManager), "FinishProject")]
		[HarmonyPriority(Priority.First)]
		public class DoCompletionDialog
		{
			//Restore completion letter when there's no queue
			//DoCompletionLetter should probably go in ResearchPal inside TryStartNext but who wants to transpile that or tell them to fix it
			//public void FinishProject(ResearchProjectDef proj, bool doCompletionDialog = false, Pawn researcher = null)
			static void Prefix(ResearchProjectDef proj, bool doCompletionDialog)
			{
				if (!doCompletionDialog) return;

				MethodInfo CompletionLetterInfo = AccessTools.Method(typeof(global::ResearchPal.Queue), "DoCompletionLetter");
				if (CompletionLetterInfo == null) return;

				CompletionLetterInfo.Invoke(null, new object[] { proj, null});
			}
		}
	}
}
