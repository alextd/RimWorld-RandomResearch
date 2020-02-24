using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace Random_Research.ResearchTreeSupport
{
	//[HarmonyPatch(typeof(Node), "Draw")]
	[StaticConstructorOnStartup]
	public class PreventChoice
	{
		static PreventChoice()
		{
			try
			{
				Patch();
			}
			catch (Exception ) { }
		}

		public static FieldInfo Research;

		public static void Patch()
		{
			Harmony harmony = new Harmony("Uuugggg.rimworld.Random_Research.main");

			Type ResearchNode = AccessTools.TypeByName("FluffyResearchTree.ResearchNode")
				?? AccessTools.TypeByName("ResearchPal.ResearchNode");

			Research = AccessTools.Field(ResearchNode, "Research");

			MethodInfo patchDraw = AccessTools.Method(ResearchNode, "Draw");
			if (patchDraw != null)
				harmony.Patch(patchDraw, null, null, new HarmonyMethod(typeof(PreventChoice), "Transpiler"));

		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo ButtonInvisibleInfo = AccessTools.Method(typeof(Widgets), "ButtonInvisible");

			MethodInfo HideButtonInvisibleInfo = AccessTools.Method(typeof(PreventChoice), "HideButtonInvisible");

			foreach (CodeInstruction i in instructions)
			{
				if (i.Calls(ButtonInvisibleInfo))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, HideButtonInvisibleInfo);
				}
				else
					yield return i;
			}
		}

		public static bool HideButtonInvisible(Rect butRect, bool doMouseoverSound, object obj)
		{
			ResearchProjectDef selected = Research.GetValue(obj) as ResearchProjectDef;
			if (selected == Find.ResearchManager.currentProj)
				selected = null;
			return BlindResearch.CanChangeTo(selected) ? Widgets.ButtonInvisible(butRect, doMouseoverSound) : false;
		}
	}
}
