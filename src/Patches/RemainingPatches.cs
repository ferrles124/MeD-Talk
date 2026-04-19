using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;

namespace MedTalk
{
    [HarmonyPatch(typeof(NPC), nameof(NPC.checkForNewCurrentDialogue))]
    public class NPC_CheckForNewCurrentDialogue_Patch
    {
        public static bool Prefix(NPC __instance, int heartLevel)
        {
            if (!DialogueBuilder.Instance.PatchNpc(__instance, 4, true)) return true;
            AsyncBuilder.Instance.RequestNpcBasic(__instance, $"heartLevel_{heartLevel}", "");
            return false;
        }
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.tryToRetrieveDialogue))]
    public class NPC_TryToRetrieveDialogue_Patch
    {
        public static bool Prefix(NPC __instance, ref Dialogue __result, string key)
        {
            if (!DialogueBuilder.Instance.PatchNpc(__instance)) return true;
            return true;
        }
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.tryToGetMarriageSpecificDialogue))]
    public class NPC_TryToGetMarriageSpecificDialogue_Patch
    {
        public static bool Prefix(NPC __instance, ref Dialogue __result, string key)
        {
            if (!DialogueBuilder.Instance.PatchNpc(__instance)) return true;
            return true;
        }
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.GetGiftReaction))]
    public class NPC_GetGiftReaction_Patch
    {
        public static bool Prefix(NPC __instance, ref string __result, StardewValley.Object item)
        {
            if (!DialogueBuilder.Instance.PatchNpc(__instance)) return true;
            AsyncBuilder.Instance.RequestNpcGiftResponse(__instance, item, 0);
            __result = "";
            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.GetLocationOverrideDialogue))]
    public class GameLocation_GetLocationOverrideDialogue_Patch
    {
        public static bool Prefix(GameLocation __instance, NPC n, ref Dialogue __result)
        {
            if (!DialogueBuilder.Instance.PatchNpc(n)) return true;
            return true;
        }
    }
}
