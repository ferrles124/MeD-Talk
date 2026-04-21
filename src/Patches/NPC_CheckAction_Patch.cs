using HarmonyLib;
using StardewValley;

namespace MedTalk
{
    [HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
    public class NPC_CheckAction_Patch
    {
        private static int _lastCallTick = -999;
        private const int MIN_TICKS_BETWEEN_CALLS = 180;

        public static bool Prefix(ref NPC __instance, ref bool __result, Farmer who, GameLocation l)
        {
            var currentTick = (int)(System.DateTime.Now.Ticks / 166667);
            
            if (currentTick - _lastCallTick < MIN_TICKS_BETWEEN_CALLS)
            {
                Log.Info($"Cooldown active, skipping");
                return true;
            }

            if (!who.CanMove) return true;
            if (AsyncBuilder.Instance.AwaitingGeneration) return true;
            if (!DialogueBuilder.Instance.PatchNpc(__instance)) return true;

            _lastCallTick = currentTick;
            Log.Info($"Requesting dialogue for {__instance.Name}");
            DialogueBuilder.Instance.ClearContext();
            TextInputManager.RequestTextInput("", __instance);

            __result = false;
            return false;
        }
    }
}
