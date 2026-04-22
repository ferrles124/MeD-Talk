using StardewModdingAPI;
using StardewValley;

namespace MedTalk
{
    public static class TextInputManager
    {
        private static IModHelper _helper;
        
        public static void Initialize(IModHelper helper)
        {
            _helper = helper;
        }
        
        // Mobil için: doğrudan AI yanıtı başlat
        public static void RequestTextInput(string prompt, NPC npc)
        {
            AsyncBuilder.Instance.RequestNpcBasic(npc, "mobile_input", prompt);
        }
    }
}
