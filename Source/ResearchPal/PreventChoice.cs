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
	[HarmonyPatch(typeof(Node), "Draw")]
	public class PreventChoice
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo ButtonInvisibleInfo = AccessTools.Method(typeof(Widgets), "ButtonInvisible");

			MethodInfo HideButtonInvisibleInfo = AccessTools.Method(typeof(PreventChoice), "HideButtonInvisible");

			foreach (CodeInstruction i in instructions)
			{
				if (i.opcode == OpCodes.Call && i.operand == ButtonInvisibleInfo)
					i.operand = HideButtonInvisibleInfo;
				
				yield return i;
			}
		}

		public static bool HideButtonInvisible(Rect butRect, bool doMouseoverSound)
		{
			return BlindResearch.CanChangeCurrent() ? Widgets.ButtonInvisible(butRect, doMouseoverSound) : false;
		}
	}
}
