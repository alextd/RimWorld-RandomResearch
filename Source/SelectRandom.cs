using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;

namespace Random_Research
{
	[HarmonyPatch(typeof(WorkGiver_Researcher), "ShouldSkip")]
	public static class SelectRandom_ShouldSkip
	{
		//public override bool ShouldSkip(Pawn pawn)
		public static bool Prefix(ref bool __result)
		{
			__result = false;
			return false;
		}
	}

	[HarmonyPatch(typeof(WorkGiver_Researcher), "get_PotentialWorkThingRequest")]
	class SelectRandom_Potential
	{
		//public override ThingRequest PotentialWorkThingRequest
		public static bool Prefix(ref ThingRequest __result)
		{
			__result = ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);
			return false;
		}
	}

	[HarmonyPatch(typeof(WorkGiver_Researcher), "HasJobOnThing")]
	class SelectRandom_HasJob
	{
		//public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		public static void Prefix(Thing t)
		{
			if (t is Building_ResearchBench bench && Find.ResearchManager.currentProj == null)
				SelectRandom.DecideRandomResearch(bench);
		}
	}

	public static class SelectRandom
	{
		public static void DecideRandomResearch(Building_ResearchBench bench)
		{
			if (DefDatabase<ResearchProjectDef>.AllDefsListForReading
				.Where((ResearchProjectDef x) => x.CanStartNow && x.CanBeResearchedAt(bench, true))
				.TryRandomElement(out ResearchProjectDef research))
			{
				Find.ResearchManager.currentProj = research;
			}
		}
	}
}
