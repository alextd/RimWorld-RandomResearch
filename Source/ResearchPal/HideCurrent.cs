using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Verse;
using RimWorld;
using UnityEngine;

namespace Random_Research.ResearchTreeSupport
{
	//[HarmonyPatch(typeof(Node), "Draw")]
	[StaticConstructorOnStartup]
	public static class HideCurrent
	{
		static HideCurrent()
		{
			try
			{
				Log.Message($"RR trying patch RP: Hide");
				Patch();
			}
			catch (Exception) { }
		}

		public static void Patch()
		{
			HarmonyInstance harmony = HarmonyInstance.Create("Uuugggg.rimworld.Random_Research.main");
			MethodInfo patchDraw = AccessTools.Method(AccessTools.TypeByName("FluffyResearchTree.ResearchNode"), "Draw");
			if(patchDraw == null) patchDraw = AccessTools.Method(AccessTools.TypeByName("ResearchPal.ResearchNode"), "Draw");
			if(patchDraw != null)
				harmony.Patch(patchDraw, null, null, new HarmonyMethod(typeof(HideCurrent), "Transpiler"));
			Log.Message($"RR did patch RP: Hide");
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			Log.Message($"Random Research transpiling Research Pal's HideC");
			MethodInfo ProgressPercentInfo = AccessTools.Property(typeof(ResearchProjectDef), "ProgressPercent").GetGetMethod();

			MethodInfo HideProgressPercentInfo = AccessTools.Method(typeof(HideCurrent), "HideProgressPercent");

			foreach (CodeInstruction i in instructions)
			{
				yield return i;

				if (i.opcode == OpCodes.Callvirt && i.operand == ProgressPercentInfo)
				{
					Log.Message($"Random Research patched Research Pal's ProgressPercent");
					yield return new CodeInstruction(OpCodes.Call, HideProgressPercentInfo);
				}
			}
		}
		
		public static float HideProgressPercent(float progress)
		{
			return BlindResearch.CanSeeProgress(progress) ? progress : 0;
		}
	}
}
