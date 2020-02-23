using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace Random_Research
{
	public class ScenPart_RandomResearch : ScenPart
	{
		public float blindThreshold;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref blindThreshold, "blindThreshold");
		}

		private string blindBuf;
		public override void DoEditInterface(Listing_ScenEdit listing)
		{
			Rect rect = listing.GetScenPartRect(this, ScenPart.RowHeight);
			Rect rectL = rect.LeftHalf().Rounded();
			Rect rectR = rect.RightHalf().Rounded();
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rectL, "TD.BlindUntilProgess".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.TextFieldPercent(rectR, ref blindThreshold, ref blindBuf, 0f, 100f);
		}

		public override string Summary(Scenario scen)
		{
			string text = "TD.RandomResearchDesc".Translate();
			if(blindThreshold>0)
				text += " " + String.Format("TD.ProjectHiddenUntilDesc".Translate(), blindThreshold);
			return text;
		}

		public override void Randomize()
		{
			blindThreshold = GenMath.RoundedHundredth(Rand.Range(0f, 1f));
		}

		public override void Tick()
		{
			base.Tick();

			if (Find.ResearchManager.currentProj == null && 
				DefDatabase<ResearchProjectDef>.AllDefsListForReading
				.Where((ResearchProjectDef x) => x.CanStartNow)
				.TryRandomElement(out ResearchProjectDef research))
			{
				Find.ResearchManager.currentProj = research;
			}
		}
	}

	[DefOf]
	public static class ScenPartDefOf
	{
		public static ScenPartDef RandomResearch;
	}

	[HarmonyPatch(typeof(Dialog_DebugActionsMenu), "DoListingItems_AllModePlayActions")]
	public static class Debug_AddRandomResearch
	{
		public static MethodInfo DebugActionInfo = AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DebugAction");
		public static FieldInfo partsInfo = AccessTools.Field(typeof(Scenario), "parts");

		[DebugAction(DebugActionCategories.General, null, allowedGameStates = AllowedGameStates.Playing)]
		public static void MakeResearchRandom()
		{
			if (BlindResearch.Active()) return;

			List<ScenPart> list = (List<ScenPart>)partsInfo.GetValue(Find.Scenario);
			list.Add(ScenarioMaker.MakeScenPart(ScenPartDefOf.RandomResearch));
		}

		[DebugAction(DebugActionCategories.General, null, allowedGameStates = AllowedGameStates.Playing)]
		public static void RemoveRandomResearch()
		{
			if (!BlindResearch.Active()) return;

			List<ScenPart> list = (List<ScenPart>)partsInfo.GetValue(Find.Scenario);
			list.RemoveAll(p => p is ScenPart_RandomResearch);
		}
	}
}

