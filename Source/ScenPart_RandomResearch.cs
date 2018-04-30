using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace Random_Research
{
	public class ScenPart_RandomResearch : ScenPart
	{
		private float blindThreshold;

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
			Widgets.Label(rectL, "blind until progress:");
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.TextFieldPercent(rectR, ref blindThreshold, ref blindBuf, 0f, 100f);
		}

		public override string Summary(Scenario scen)
		{
			string text = "Research will be randomly decided.";
			if(blindThreshold>0)
				text += " " + String.Format("The current project is hidden until progress is {0:P0}", blindThreshold);
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
}

