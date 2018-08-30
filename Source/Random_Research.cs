using System.Reflection;
using Verse;
using UnityEngine;
using Harmony;

namespace Random_Research
{
	public class Mod : Verse.Mod
	{
		HarmonyInstance harmony;
		public Mod(ModContentPack content) : base(content)
		{
			// initialize settings
			// GetSettings<Settings>();
#if DEBUG
			HarmonyInstance.DEBUG = true;
#endif
			harmony = HarmonyInstance.Create("Uuugggg.rimworld.Random_Research.main");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
		public static HarmonyInstance Harmony()
		{
			return LoadedModManager.GetMod<Random_Research.Mod>().harmony;
		}

//		public override void DoSettingsWindowContents(Rect inRect)
//		{
//			base.DoSettingsWindowContents(inRect);
//			GetSettings<Settings>().DoWindowContents(inRect);
//		}
//
//		public override string SettingsCategory()
//		{
//			return "TD.RandomResearch".Translate();
//		}
	}
}