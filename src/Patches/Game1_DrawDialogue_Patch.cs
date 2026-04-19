using HarmonyLib;
using StardewValley;
using System;

namespace MedTalk
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.DrawDialogue), new Type[] { typeof(Dialogue) })]
    public class Game1_DrawDialogue_Patch
    {
        public static bool Prefix(Dialogue dialogue)
        {
            if (dialogue == null || dialogue.dialogues == null || dialogue.dialogues.Count == 0)
                return true;

            var first = dialogue.dialogues.Peek();
            if (first != null && first.ToString().StartsWith("skip#"))
                return false;

            return true;
        }
    }
}
