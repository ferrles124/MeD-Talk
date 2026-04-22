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
        
        public static void RequestTextInput(string prompt, NPC npc)
        {
            // Mobil için: doğrudan AI yanıtı başlat
            // Not: PC'de oynayanlar için metin girişi istenebilir,
            // ancak bu mod mobil öncelikli olduğu için direkt AI çağrılıyor.
            AsyncBuilder.Instance.RequestNpcBasic(npc, "touch_input", prompt);
        }
        
        // PC kullanıcıları için alternatif (isteğe bağlı)
        public static void RequestTextInputPC(string prompt, NPC npc)
        {
            _helper.Input.ShowTextInput(prompt, (success, text) =>
            {
                if (success && !string.IsNullOrEmpty(text))
                    AsyncBuilder.Instance.RequestNpcBasic(npc, "pc_input", text);
            });
        }
    }
}
