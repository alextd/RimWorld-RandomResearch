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

namespace Random_Research
{
	[HarmonyPatch(typeof(MainTabWindow_Research), "DrawLeftRect")]
	class PreventResearchChoice
	{
		//private void DrawLeftRect(Rect leftOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo ButtonTextInfo = AccessTools.Method(typeof(Widgets), "ButtonText", new Type[]
				{typeof(Rect), typeof(string), typeof(bool), typeof(bool), typeof(bool)});

			MethodInfo HideButtonTextInfo = AccessTools.Method(typeof(PreventResearchChoice), "HideButtonText");

			foreach (CodeInstruction i in instructions)
			{
				if (i.opcode == OpCodes.Call && i.operand.Equals(ButtonTextInfo))
					i.operand = HideButtonTextInfo;
				yield return i;
			}
		}

		public static bool HideButtonText(Rect rect, string label, bool drawBackground, bool doMouseoverSound, bool active)
		{
			return BlindResearch.CanChangeTo(BlindResearch.SelectedResearch()) ? Widgets.ButtonText(rect, label, drawBackground, doMouseoverSound, active) : false;
		}
	}
}
