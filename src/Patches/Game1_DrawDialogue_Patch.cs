using HarmonyLib;
using StardewValley;
using System;

namespace MedTalk
{
    [HarmonyPatch(typeof(Game1), "drawDialogue", new Type[] { typeof(Dialogue) })]
    public class Game1_DrawDialogue_Patch
    {
        public static bool Prefix(Dialogue dialogue)
        {
            if (dialogue == null) return true;
            
            if (dialogue is MedTalkDialogue medTalkDlg)
            {
                if (medTalkDlg.dialogues == null || medTalkDlg.dialogues.Count == 0)
                    return true;
                    
                var first = medTalkDlg.dialogues[0];
                if (first != null && first.Text != null && first.Text.StartsWith("skip#"))
                    return false;
            }
            
            return true;
        }
    }
}
