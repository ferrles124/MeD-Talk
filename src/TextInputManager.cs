using StardewValley;
using StardewModdingAPI.Events;

namespace MedTalk
{
    public static class TextInputManager
    {
        private static bool _isProcessing = false;
        private static int _cooldownTicks = 0;
        private const int COOLDOWN = 120;

        public static void Initialize()
        {
            ModEntry.SHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (_cooldownTicks > 0) _cooldownTicks--;
            if (_cooldownTicks == 0) _isProcessing = false;
        }

        public static void RequestTextInput(string prompt, NPC npc)
        {
            if (_isProcessing) return;
            if (AsyncBuilder.Instance.AwaitingGeneration) return;

            _isProcessing = true;
            _cooldownTicks = COOLDOWN;
            AsyncBuilder.Instance.RequestNpcBasic(npc, "touch_input", "");
        }
    }
}
