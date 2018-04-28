using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Verse;
using RimWorld;

namespace Random_Research
{
	[HarmonyPatch(typeof(MainTabWindow_Research), "PreOpen")]
	class HideCurrentResearch_Open
	{
		//public override void PreOpen()
		public static void Postfix(MainTabWindow_Research __instance)
		{
			AccessTools.Field(typeof(MainTabWindow_Research), "selectedProject").SetValue(__instance, null);
			//__instance.selectedProject = null;
			AccessTools.Property(typeof(MainTabWindow_Research), "CurTab").GetSetMethod(true).Invoke(__instance, new object[] { ResearchTabDefOf.Main });
			//__instance.CurTab = ResearchTabDefOf.Main;
		}
	}

	[HarmonyPatch(typeof(MainTabWindow_Research), "DrawLeftRect")]
	class HideCurrentResearch_LeftRect
	{
		//private void DrawLeftRect(Rect leftOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo AndShowIt = AccessTools.Method(typeof(HideCurrentResearch_LeftRect), "AndShowIt");

			foreach (CodeInstruction i in instructions)
			{
				if(i.opcode == OpCodes.Bne_Un)	// if (i1 != i2)
				{
					yield return new CodeInstruction(OpCodes.Ceq);//bool result = i1 == i2
					yield return new CodeInstruction(OpCodes.Call, AndShowIt);	//result = AndShowIt(result)
					yield return new CodeInstruction(OpCodes.Brfalse, i.operand);//if (!result)
				}
				else
					yield return i;
			}
		}

		public static bool AndShowIt(bool selectedCurrent)
		{
			return selectedCurrent && false;
		}
	}
}
