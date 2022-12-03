using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using HugsLib.Settings;
using RimWorld;
using Verse;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;

namespace HugsLib.Patches {
	[HarmonyPatch(typeof(Dialog_Options))]
	[HarmonyPatch(nameof(Dialog_Options.PostOpen))]
	internal class DialogOptions_PostOpen_Patch {
		[HarmonyPostfix]
		public static void InjectHugsLibEntries(Dialog_Options __instance) {
			OptionsDialogExtensions.InjectHugsLibModEntries(__instance);
		}
	}

	[HarmonyPatch(typeof(Dialog_Options), nameof (Dialog_Options.DoModOptions), new[] { typeof(Listing_Standard) })]
	internal class DialogOptions_DoModOptions_Patch
	{
		public static bool Prefix(Dialog_Options __instance, Listing_Standard listing)
		{
			Rect rect = listing.GetRect(30f, 1f);
			__instance.quickSearchWidget.OnGUI(rect, null);
			__instance.modFilter = __instance.quickSearchWidget.filter.Text;
			listing.Gap(12f);
			Rect rect2 = default(Rect);
			int num = 0;
			foreach (Mod mod in __instance.cachedModsWithSettings)
			{
				if (mod.SettingsCategory().ToLower().Contains(__instance.modFilter.ToLower()) || (mod.Content != null && mod.Content.Name.ToLower().Contains(__instance.modFilter.ToLower())))
				{
					if (num % 2 == 0)
					{
						rect2 = listing.GetRect(34f, 1f);
					}
					if (Widgets.ButtonText(((num % 2 == 0) ? rect2.LeftHalf() : rect2.RightHalf()).ContractedBy(2f), "  " + mod.SettingsCategory(), true, true, true, new TextAnchor?(TextAnchor.MiddleLeft)))
					{
						Find.WindowStack.Add(OptionsDialogExtensions.GetModSettingsWindow(mod));
					}
					num++;
				}
			}
			return false;
		}
	}
}