﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace Random_Research.ResearchPal
{
	//public static void DrawQueue( Rect canvas, bool interactible )
	//[HarmonyPatch(typeof(Queue), "DrawQueue")]
	[StaticConstructorOnStartup]
	public static class Cancel
	{
		static Cancel()
		{
			try
			{
				Patch();
			}
			catch (Exception) { }
		}

		public static void Patch()
		{
			Harmony harmony = new Harmony("Uuugggg.rimworld.Random_Research.main");
			MethodInfo patchDrawQueue = AccessTools.Method(AccessTools.TypeByName("FluffyResearchTree.Queue"), "DrawQueue");
			if (patchDrawQueue == null) patchDrawQueue = AccessTools.Method(AccessTools.TypeByName("ResearchPal.Queue"), "DrawQueue");
			if (patchDrawQueue != null)
				harmony.Patch(patchDrawQueue, postfix: new HarmonyMethod(typeof(Cancel), "Postfix"));
		}

		public static void Postfix(Rect canvas, bool interactible)
		{
			if (BlindResearch.Active()
				&& BlindResearch.CanSeeCurrent())
			{
				Rect iconRect = canvas.ContractedBy(4);
				iconRect.width = iconRect.height;
				if (Widgets.ButtonImage(iconRect, ContentFinder<Texture2D>.Get("UI/Designators/Cancel")) && interactible)
				{
					Find.ResearchManager.currentProj = null;
				}
			}
		}
	}
}
