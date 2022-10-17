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
				{typeof(Rect), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(TextAnchor?)});

			MethodInfo HideButtonTextInfo = AccessTools.Method(typeof(PreventResearchChoice), "HideButtonText");

			foreach (CodeInstruction i in instructions)
			{
				if (i.Calls(ButtonTextInfo))
					yield return new CodeInstruction(OpCodes.Call, HideButtonTextInfo);
				else
					yield return i;
			}
		}

		public static bool HideButtonText(Rect rect, string label, bool drawBackground, bool doMouseoverSound, bool active, TextAnchor? overrideTextAnchor)
		{
			bool result = false;

			if (BlindResearch.CanChangeTo(BlindResearch.SelectedResearch()))
				result = Widgets.ButtonText(rect, label, drawBackground, doMouseoverSound, active, overrideTextAnchor);
			else
			{
				if(rect.height > 30)//Debug buttons are 30, aeh, so don't draw this for them.
				//Same as 'in-progress'
					Widgets.DrawHighlight(rect);
			}

			return result;
		}
	}
}
