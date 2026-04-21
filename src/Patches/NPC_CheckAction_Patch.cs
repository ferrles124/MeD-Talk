using HarmonyLib;
using StardewValley;

namespace MedTalk
{
    [HarmonyPatch(typeof(NPC), nameof(NPC.checkAction))]
    public class NPC_CheckAction_Patch
    {
        private static bool _isOnCooldown = false;

        public static bool Prefix(ref NPC __instance, ref bool __result, Farmer who, GameLocation l)
        {
            if (_isOnCooldown)
            {
                Log.Info("Cooldown active, skipping");
                return true;
            }

            if (!who.CanMove) return true;
            if (AsyncBuilder.Instance.AwaitingGeneration) return true;
            if (!DialogueBuilder.Instance.PatchNpc(__instance)) return true;

            _isOnCooldown = true;
            ModEntry.SHelper.Events.GameLoop.UpdateTicked += ResetCooldown;

            Log.Info($"Requesting dialogue for {__instance.Name}");
            DialogueBuilder.Instance.ClearContext();
            TextInputManager.RequestTextInput("", __instance);

            __result = false;
            return false;
        }

        private static int _tickCount = 0;
        private static void ResetCooldown(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            _tickCount++;
            if (_tickCount >= 120)
            {
                _tickCount = 0;
                _isOnCooldown = false;
                ModEntry.SHelper.Events.GameLoop.UpdateTicked -= ResetCooldown;
                Log.Info("Cooldown reset");
            }
        }
    }
}
