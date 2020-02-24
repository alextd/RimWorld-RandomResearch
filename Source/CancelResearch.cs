using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using HarmonyLib;
using UnityEngine;

namespace Random_Research
{
	[HarmonyPatch(typeof(MainTabWindow_Research), "DrawLeftRect")]
	static class CancelResearch
	{
		//private void DrawLeftRect(Rect leftOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			//public static Rect FillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
			MethodInfo FillableBarInfo = AccessTools.Method(typeof(Widgets), "FillableBar", new Type[]
				{ typeof(Rect), typeof(float), typeof(Texture2D), typeof(Texture2D), typeof(bool)});
			//Whoops this method is replaced so let's patch it too:
			MethodInfo HideFillableBarInfo = AccessTools.Method(typeof(HideCurrentResearch_LeftRect), "HideFillableBar");
			

			foreach (CodeInstruction i in instructions)
			{
				yield return i;

				if (i.Calls(FillableBarInfo) || i.Calls(HideFillableBarInfo))
				{
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CancelResearch), nameof(CancelResearch.DrawCancelButton)));
				}
			}
		}
		
		public static Rect DrawCancelButton(Rect rect)
		{
			if (BlindResearch.Active()
				&& BlindResearch.SelectedResearch() == Find.ResearchManager.currentProj
				&& BlindResearch.CanSeeCurrent())
			{
				Rect iconRect = rect.ContractedBy(2);
				iconRect.width = iconRect.height;
				if (Widgets.ButtonImage(iconRect, ContentFinder<Texture2D>.Get("UI/Designators/Cancel")))
				{
					Find.ResearchManager.currentProj = null;
				}
			}
			return rect;
		}
	}
}
