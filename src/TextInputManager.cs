using StardewValley;

namespace MedTalk
{
    public static class TextInputManager
    {
        private static bool _isProcessing = false;

        public static void Initialize() { }

        public static void RequestTextInput(string prompt, NPC npc)
        {
            if (_isProcessing) return;
            if (AsyncBuilder.Instance.AwaitingGeneration) return;
            
            _isProcessing = true;
            AsyncBuilder.Instance.RequestNpcBasic(npc, "touch_input", prompt);
            
            ModEntry.SHelper.Events.GameLoop.UpdateTicked += ResetProcessing;
        }

        private static void ResetProcessing(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            _isProcessing = false;
            ModEntry.SHelper.Events.GameLoop.UpdateTicked -= ResetProcessing;
        }
    }
}
