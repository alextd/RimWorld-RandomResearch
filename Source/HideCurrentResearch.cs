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
	[HarmonyPatch(typeof(MainTabWindow_Research), "PreOpen")]
	class HideCurrentResearch_Open
	{
		//public override void PreOpen()

		delegate void CurTabDel(MainTabWindow_Research tab, ResearchTabDef def);
		static CurTabDel CurTabSet = AccessTools.MethodDelegate<CurTabDel>(AccessTools.Property(typeof(MainTabWindow_Research), "CurTab").GetSetMethod(true));
		public static void Postfix(MainTabWindow_Research __instance, ref ResearchProjectDef ___selectedProject)
		{
			if (BlindResearch.CanSeeCurrent()) return;

			___selectedProject = null;
			CurTabSet(__instance, ResearchTabDefOf.Main);
			//__instance.CurTab = ResearchTabDefOf.Main;
		}
	}

	[HarmonyPatch(typeof(MainTabWindow_Research), "DrawLeftRect")]
	class HideCurrentResearch_LeftRect
	{
		//private void DrawLeftRect(Rect leftOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo FillableBarInfo = AccessTools.Method(typeof(Widgets), "FillableBar", new Type[]
				{ typeof(Rect), typeof(float), typeof(Texture2D), typeof(Texture2D), typeof(bool)});
			MethodInfo GetProgressApparentInfo = AccessTools.Property(typeof(ResearchProjectDef), "ProgressApparent").GetGetMethod();

			MethodInfo InProgessStringInfo = AccessTools.Method(typeof(HideCurrentResearch_LeftRect), nameof(InProgessString));
			MethodInfo HideFillableBarInfo = AccessTools.Method(typeof(HideCurrentResearch_LeftRect), "HideFillableBar");
			MethodInfo HideProgressApparentInfo = AccessTools.Method(typeof(HideCurrentResearch_LeftRect), "HideProgressApparent");

			foreach (CodeInstruction i in instructions)
			{
				if (i.Calls(FillableBarInfo))
					yield return new CodeInstruction(OpCodes.Call, HideFillableBarInfo);
				else 
					yield return i;
				if (i.opcode == OpCodes.Ldstr && (i.operand as string).Equals("InProgress"))
					yield return new CodeInstruction(OpCodes.Call, InProgessStringInfo);

				if (i.Calls(GetProgressApparentInfo))
					yield return new CodeInstruction(OpCodes.Call, HideProgressApparentInfo);
			}
		}

		public static string InProgessString(string inProgress)
		{
			return BlindResearch.CanSeeCurrent() ? inProgress : "";
		}

		public static Rect HideFillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
		{
			if (!BlindResearch.CanSeeProgress(fillPercent))
				fillPercent = 0;
			return Widgets.FillableBar(rect, fillPercent, fillTex, bgTex, doBorder);
		}

		public static float HideProgressApparent(float progress)
		{
			return BlindResearch.CanSeeCurrent() ? progress : 0;
		}
	}

	[HarmonyPatch(typeof(MainTabWindow_Research), "DrawRightRect")]
	class HideCurrentResearch_RightRect
	{
		//private void DrawRightRect(Rect rightOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo ActiveResearchColorInfo = AccessTools.Field(typeof(TexUI), "ActiveResearchColor");

			MethodInfo ReplaceColorInfo = AccessTools.Method(typeof(HideCurrentResearch_RightRect), "ReplaceColor");

			foreach (CodeInstruction i in instructions)
			{
				yield return i;
				if (i.LoadsField(ActiveResearchColorInfo))
				{
					yield return new CodeInstruction(OpCodes.Call, ReplaceColorInfo);
				}
			}
		}

		public static Color ReplaceColor(Color activeTex)
		{
			return BlindResearch.CanSeeCurrent() ? activeTex : TexUI.AvailResearchColor;
		}
	}
}
