using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Random_Research
{
	public class Alert_NoResearchProject : Alert
	{
		public Alert_NoResearchProject()
		{
			this.defaultLabel = "TD.NeedResearchEquipment".Translate();
			this.defaultExplanation = "TD.NeedResearchEquipmentDesc".Translate();
		}

		public override AlertReport GetReport()
		{
			if (Find.AnyPlayerHomeMap == null
				|| Find.ResearchManager.currentProj != null
				|| !BlindResearch.Active())
					return false;

			foreach(Map map in Find.Maps)
				if (map.IsPlayerHome && map.listerBuildings.ColonistsHaveResearchBench())
						return !Find.ResearchManager.AnyProjectIsAvailable &&
						DefDatabase<ResearchProjectDef>.AllDefsListForReading.Any(x => !x.IsFinished);
				
			return false;
		}
	}
}
