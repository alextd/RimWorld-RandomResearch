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
using static Verse.GlowGrid;

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

	[HarmonyPatch(typeof(MainTabWindow_Research), "DrawProjectProgress")]
	class HideCurrentResearch_DrawProjectProgress
	{
		//private void DrawLeftRect(Rect leftOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo LabelInfoString = AccessTools.Method(typeof(Widgets), nameof(Widgets.Label),
				new Type[] { typeof(Rect), typeof(string) });
			MethodInfo LabelInfoTaggedString = AccessTools.Method(typeof(Widgets), nameof(Widgets.Label),
				new Type[] { typeof(Rect), typeof(TaggedString) });

			MethodInfo HideLabelInfoString = AccessTools.Method(typeof(HideCurrentResearch_DrawProjectProgress), nameof(LabelString));
			MethodInfo HideLabelInfoTaggedString = AccessTools.Method(typeof(HideCurrentResearch_DrawProjectProgress), nameof(LabelTaggedString));

			foreach (CodeInstruction i in instructions)
			{
				if (i.Calls(LabelInfoString))
					yield return new CodeInstruction(OpCodes.Call, HideLabelInfoString);
				else if (i.Calls(LabelInfoTaggedString))
					yield return new CodeInstruction(OpCodes.Call, HideLabelInfoTaggedString);
				else
					yield return i;
			}
		}

		public static void LabelString(Rect rect, string str)
		{
			if (!BlindResearch.CanSeeCurrent()) return;

			Widgets.Label(rect, str);
		}

		public static void LabelTaggedString(Rect rect, TaggedString str)
		{
			if (!BlindResearch.CanSeeCurrent()) return;

			Widgets.Label(rect, str);
		}
	}

	[HarmonyPatch(typeof(MainTabWindow_Research), "DrawStartButton")]
	class HideCurrentResearch_DrawStart
	{
		//private void DrawLeftRect(Rect leftOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo InProgessStringInfo = AccessTools.Method(typeof(HideCurrentResearch_DrawStart), nameof(InProgessString));

			foreach (CodeInstruction i in instructions)
			{
				yield return i;

				if (i.opcode == OpCodes.Ldstr && (i.operand as string).Equals("InProgress"))
					yield return new CodeInstruction(OpCodes.Call, InProgessStringInfo);
			}
		}

		public static string InProgessString(string inProgress)
		{
			return BlindResearch.CanSeeCurrent() ? inProgress : "";
		}
	}

	[HarmonyPatch(typeof(MainTabWindow_Research), "ListProjects")]
	class HideCurrentResearch_RightRect
	{
		//private void DrawRightRect(Rect rightOutRect)
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo ActiveResearchColorInfo = AccessTools.Field(typeof(TexUI), "ActiveResearchColor");
			FieldInfo ActiveProjectColorInfo = AccessTools.Field(typeof(MainTabWindow_Research), "ActiveProjectLabelColor");
			FieldInfo BorderResearchingColorInfo = AccessTools.Field(typeof(TexUI), "BorderResearchingColor");

			MethodInfo ProgressPercentInfo = AccessTools.PropertyGetter(typeof(ResearchProjectDef), nameof(ResearchProjectDef.ProgressPercent));


			MethodInfo ReplaceColorActiveResearchInfo = AccessTools.Method(typeof(HideCurrentResearch_RightRect), "ReplaceColorActiveResearch");
			MethodInfo ReplaceColorActiveProjectInfo = AccessTools.Method(typeof(HideCurrentResearch_RightRect), "ReplaceColorActiveProject");
			MethodInfo ReplaceColorBorderResearchingInfo = AccessTools.Method(typeof(HideCurrentResearch_RightRect), "ReplaceColorBorderResearching");

			MethodInfo ProgressPercentZeroerInfo = AccessTools.Method(typeof(HideCurrentResearch_RightRect), "ZeroProgress");

			foreach (CodeInstruction i in instructions)
			{
				yield return i;
				if (i.LoadsField(ActiveResearchColorInfo))
				{
					yield return new CodeInstruction(OpCodes.Call, ReplaceColorActiveResearchInfo);
				}
				if (i.LoadsField(ActiveProjectColorInfo))
				{
					yield return new CodeInstruction(OpCodes.Call, ReplaceColorActiveProjectInfo);
				}
				if (i.LoadsField(BorderResearchingColorInfo))
				{
					yield return new CodeInstruction(OpCodes.Call, ReplaceColorBorderResearchingInfo);
				}
				if (i.Calls(ProgressPercentInfo))
				{
					yield return new CodeInstruction(OpCodes.Call, ProgressPercentZeroerInfo);
				}
			}
		}

		public static Color ReplaceColorActiveResearch(Color activeTex)
		{
			return BlindResearch.CanSeeCurrent() ? activeTex : TexUI.AvailResearchColor;
		}

		public static Color ReplaceColorActiveProject(Color activeTex)
		{
			return BlindResearch.CanSeeCurrent() ? activeTex : Widgets.NormalOptionColor;
		}

		public static Color ReplaceColorBorderResearching(Color activeTex)
		{
			return BlindResearch.CanSeeCurrent() ? activeTex : TexUI.DefaultBorderResearchColor;
		}

		public static float ZeroProgress(float progress)
		{
			return BlindResearch.CanSeeProgress(progress) ? progress : 0;
		}
	}
}
