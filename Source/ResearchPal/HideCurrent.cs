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
				Patch();
			}
			catch (Exception) { }
		}

		public static void Patch()
		{
			HarmonyInstance harmony = Mod.Harmony();
			harmony.Patch(AccessTools.Method(typeof(Node), "Draw"), null, null,
				new HarmonyMethod(typeof(HideCurrent), "Transpiler"));
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo ProgressPercentInfo = AccessTools.Property(typeof(ResearchProjectDef), "ProgressPercent").GetGetMethod();

			MethodInfo HideProgressPercentInfo = AccessTools.Method(typeof(HideCurrent), "HideProgressPercent");

			foreach (CodeInstruction i in instructions)
			{
				yield return i;

				if (i.opcode == OpCodes.Callvirt && i.operand == ProgressPercentInfo)
					yield return new CodeInstruction(OpCodes.Call, HideProgressPercentInfo);
			}
		}
		
		public static float HideProgressPercent(float progress)
		{
			return BlindResearch.CanSeeProgress(progress) ? progress : 0;
		}
	}
}
