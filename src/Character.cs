using System;
using System.Collections.Generic;
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
            var prompt = BuildPrompt(originalLine);
            return await Llm.Instance.GenerateDialogue(prompt);
        }

        public async Task<string> CreateResponse(List<ConversationElement> conversation)
        {
            if (Llm.Instance == null) return "...";
            var prompt = BuildConversationPrompt(conversation);
            return await Llm.Instance.GenerateDialogue(prompt);
        }

        public async Task<string> CreateGiftResponse(StardewValley.Object gift, int taste)
        {
            if (Llm.Instance == null) return "...";
            var prompt = $"You are {Name} from Stardew Valley. React to receiving {gift.DisplayName} as a gift. Keep it short and in character.";
            return await Llm.Instance.GenerateDialogue(prompt);
        }

        private string BuildPrompt(string context)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"You are {Name} from Stardew Valley.");
            sb.AppendLine("Respond naturally and in character. Keep your response short (1-2 sentences).");
            if (!string.IsNullOrEmpty(context))
                sb.AppendLine($"Context: {context}");
            sb.AppendLine("Say something to the farmer.");
            return sb.ToString();
        }

        private string BuildConversationPrompt(List<ConversationElement> conversation)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"You are {Name} from Stardew Valley.");
            sb.AppendLine("Respond naturally and in character. Keep your response short (1-2 sentences).");
            sb.AppendLine("Conversation so far:");
            foreach (var element in conversation)
            {
                var speaker = element.IsPlayerLine ? "Farmer" : Name;
                sb.AppendLine($"{speaker}: {element.Text}");
            }
            sb.AppendLine($"{Name}:");
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
