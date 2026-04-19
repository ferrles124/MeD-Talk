using HarmonyLib;
using StardewValley;
using System;
using System.Linq;

namespace MedTalk
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.DrawDialogue), new Type[] { typeof(Dialogue) })]
    public class Game1_DrawDialogue_Patch
    {
        public static bool Prefix(Dialogue dialogue)
        {
            if (dialogue == null || dialogue.dialogues == null || dialogue.dialogues.Count == 0)
                return true;

            if (dialogue.dialogues.First().StartsWith("skip#"))
                return false;

            return true;
        }
    }
}
