using HarmonyLib;
using StardewValley;

namespace MedTalk
{
    [HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
    public class NPC_CheckAction_Patch
    {
        public static bool Prefix(ref NPC __instance, ref bool __result, Farmer who, GameLocation l)
        {
            Log.Info($"NPC_CheckAction_Patch called for {__instance.Name}");

            if (!who.CanMove)
            {
                Log.Info("Farmer cannot move, skipping");
                return true;
            }

            if (!DialogueBuilder.Instance.PatchNpc(__instance))
            {
                Log.Info("PatchNpc returned false, skipping");
                return true;
            }

            Log.Info($"Requesting dialogue for {__instance.Name}");
            DialogueBuilder.Instance.ClearContext();
            TextInputManager.RequestTextInput("", __instance);

            __result = false;
            return false;
        }
    }
}
