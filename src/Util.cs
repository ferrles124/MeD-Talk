using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MedTalk
{
    public class Util
    {
        public static IEnumerable<NPC> GetNearbyNpcs(NPC npc)
        {
            var speakerLocation = npc.Tile;
            var speakerName = npc.Name;
            var npcs = Game1.currentLocation.characters.Where(x => x.CanReceiveGifts() && x.Name != speakerName);
            var nearbyNpcs = new List<NPC>();
            foreach (var otherNpc in npcs)
            {
                var npcLocation = otherNpc.Tile;
                var dx = speakerLocation.X - npcLocation.X;
                var dy = speakerLocation.Y - npcLocation.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance < 4.5)
                    nearbyNpcs.Add(otherNpc);
            }
            return nearbyNpcs;
        }

        internal static string ConcatAnd(List<string> strings)
        {
            if (strings.Count == 0) return string.Empty;
            if (strings.Count == 1) return strings[0];
            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < strings.Count; i++)
            {
                if (i == strings.Count - 1) builder.Append(" and ");
                else if (i > 0) builder.Append(", ");
                builder.Append(strings[i]);
            }
            return builder.ToString();
        }

        internal static string GetString(Character npc, string key, object tokens = null, bool returnNull = false)
        {
            return GetString(key, tokens, returnNull);
        }

        internal static string GetString(string key, object tokens = null, bool returnNull = false)
        {
            if (returnNull) return null;
            return key;
        }
    }
}
