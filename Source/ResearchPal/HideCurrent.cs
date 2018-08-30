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
using ResearchPal;

namespace Random_Research.ResearchPal
{
	//[HarmonyPatch(typeof(Node), "Draw")]
	[StaticConstructorOnStartup]
	public static class HideCurrent
	{
		static HideCurrent()
		{
			try
			{
				Log.Message("RR trying patch RP: Hide");
				Patch();
				Log.Message("RR did patch RP: Hide");
			}
			catch (Exception) { }
		}

		public static void Patch()
		{
			HarmonyInstance harmony = Mod.Harmony();
			harmony.Patch(AccessTools.Method(typeof(ResearchNode), "Draw"), null, null,
				new HarmonyMethod(typeof(HideCurrent), "Transpiler"));
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			Log.Message("Random Research transpiling Research Pal's HideC");
			MethodInfo ProgressPercentInfo = AccessTools.Property(typeof(ResearchProjectDef), "ProgressPercent").GetGetMethod();

			MethodInfo HideProgressPercentInfo = AccessTools.Method(typeof(HideCurrent), "HideProgressPercent");

			foreach (CodeInstruction i in instructions)
			{
				yield return i;

				if (i.opcode == OpCodes.Callvirt && i.operand == ProgressPercentInfo)
				{
					Log.Message("Random Research patched Research Pal's ProgressPercent");
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
