using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MedTalk
{
    public class Character
    {
        public string Name { get; }
        private NPC _npc;

        public Character(string name, NPC npc)
        {
            Name = name;
            _npc = npc;
        }

        public async Task<string> CreateDialogue(string originalLine)
        {
            if (Llm.Instance == null) return originalLine;
            var prompt = BuildContextPrompt();
            return await Llm.Instance.GenerateDialogue(prompt);
        }

        public async Task<string> CreateResponse(List<ConversationElement> conversation)
        {
            if (Llm.Instance == null) return "...";
            var sb = new StringBuilder();
            sb.AppendLine(BuildContextPrompt());
            sb.AppendLine("\nConversation so far:");
            foreach (var element in conversation)
            {
                var speaker = element.IsPlayerLine ? Game1.player.Name : Name;
                sb.AppendLine($"{speaker}: {element.Text}");
            }
            sb.AppendLine($"\n{Name}:");
            return await Llm.Instance.GenerateDialogue(sb.ToString());
        }

        public async Task<string> CreateGiftResponse(StardewValley.Object gift, int taste)
        {
            if (Llm.Instance == null) return "...";
            var sb = new StringBuilder();
            sb.AppendLine(BuildContextPrompt());
            sb.AppendLine($"\nThe farmer just gave you {gift.DisplayName} as a gift.");
            var tasteWord = taste switch
            {
                0 => "love",
                2 => "like",
                4 => "dislike",
                6 => "hate",
                _ => "feel neutral about"
            };
            sb.AppendLine($"You {tasteWord} this gift. React naturally in character.");
            return await Llm.Instance.GenerateDialogue(sb.ToString());
        }

        private string BuildContextPrompt()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"You are {Name}, a character in Stardew Valley. Respond in character, naturally and briefly (1-2 sentences).");
            sb.AppendLine($"Respond in the same language the farmer uses. Default to Turkish.");

            try
            {
                var farmer = Game1.player;
                var location = _npc?.currentLocation?.Name ?? Game1.currentLocation?.Name ?? "unknown";
                var season = Game1.currentSeason ?? "spring";
                var day = Game1.dayOfMonth;
                var year = Game1.year;
                var time = Game1.timeOfDay;
                var timeStr = $"{(time / 100) % 24}:{time % 100:00}";

                var hearts = farmer.friendshipData.ContainsKey(Name)
                    ? farmer.friendshipData[Name].Points / 250
                    : 0;

                var isMarried = farmer.friendshipData.ContainsKey(Name) &&
                    farmer.friendshipData[Name].IsMarried();

                sb.AppendLine($"\n=== Current Context ===");
                sb.AppendLine($"Location: {location}");
                sb.AppendLine($"Season: {season}, Day: {day}, Year: {year}");
                sb.AppendLine($"Time: {timeStr}");
                sb.AppendLine($"Friendship hearts with farmer: {hearts}");
                sb.AppendLine($"Married to farmer: {isMarried}");
                sb.AppendLine($"Farmer name: {farmer.Name}");

                var weather = new List<string>();
                if (Game1.IsRainingHere()) weather.Add("raining");
                if (Game1.IsSnowingHere()) weather.Add("snowing");
                if (Game1.IsLightningHere()) weather.Add("lightning");
                if (weather.Count > 0)
                    sb.AppendLine($"Weather: {string.Join(", ", weather)}");
                else
                    sb.AppendLine("Weather: sunny");
            }
            catch { }

            sb.AppendLine($"\nRespond as {Name} would naturally speak. Keep it short and in character.");
            return sb.ToString();
        }

        public void ClearConversationHistory() { }

        internal void AddDialogue(IEnumerable<StardewValley.DialogueLine> dialogues, int year, StardewValley.Season season, int dayOfMonth, int timeOfDay) { }
        internal void AddEventDialogue(List<StardewValley.DialogueLine> dialogues, IEnumerable<NPC> actors, string festivalName, int year, StardewValley.Season season, int dayOfMonth, int timeOfDay) { }
        internal void AddOverheardDialogue(NPC speaker, List<StardewValley.DialogueLine> dialogues, int year, StardewValley.Season season, int dayOfMonth, int timeOfDay) { }
        internal void AddConversation(List<ConversationElement> chatHistory, int year, StardewValley.Season season, int dayOfMonth, int timeOfDay) { }
        internal bool MatchLastDialogue(List<StardewValley.DialogueLine> dialogues) => false;
        internal bool SpokeJustNow() => false;
    }
}
