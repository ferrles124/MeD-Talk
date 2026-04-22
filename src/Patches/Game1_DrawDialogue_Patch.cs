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
            if (dialogue == null ||
