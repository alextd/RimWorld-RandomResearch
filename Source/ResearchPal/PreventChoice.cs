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
	public class PreventChoice
	{
		static PreventChoice()
		{
			try
			{
				Log.Message("RR trying patch RP: Choice");
				Patch();
				Log.Message("RR did patch RP: Choice");
			}
			catch (Exception ) { }
		}

		public static void Patch()
		{
			HarmonyInstance harmony = HarmonyInstance.Create("Uuugggg.rimworld.Random_Research.main");
			harmony.Patch(AccessTools.Method(typeof(ResearchNode), "Draw"), null, null,
				new HarmonyMethod(typeof(PreventChoice), "Transpiler"));
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			Log.Message("Random Research transpiling Research Pal's Choice");
			MethodInfo ButtonInvisibleInfo = AccessTools.Method(typeof(Widgets), "ButtonInvisible");

			MethodInfo HideButtonInvisibleInfo = AccessTools.Method(typeof(PreventChoice), "HideButtonInvisible");

			foreach (CodeInstruction i in instructions)
			{
				if (i.opcode == OpCodes.Call && i.operand == ButtonInvisibleInfo)
				{
					Log.Message("Random Research patched Research Pal's Button");
					i.operand = HideButtonInvisibleInfo;
				}
				
				yield return i;
			}
		}

		public static bool HideButtonInvisible(Rect butRect, bool doMouseoverSound)
		{
			return BlindResearch.CanChangeCurrent() ? Widgets.ButtonInvisible(butRect, doMouseoverSound) : false;
		}
	}
}
