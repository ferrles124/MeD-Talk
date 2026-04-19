using StardewValley;

namespace MedTalk
{
    public static class TextInputManager
    {
        public static void Initialize() { }

        public static void RequestTextInput(string prompt, NPC npc)
        {
            // Mobil için dokunmatik input — doğrudan AI response tetikle
            AsyncBuilder.Instance.RequestNpcBasic(npc, "touch_input", prompt);
        }
    }
}
