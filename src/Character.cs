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
            return await Llm.Instance.GenerateDialogue($"You are {Name}. Say something natural.");
        }

        public async Task<string> CreateResponse(List<ConversationElement> conversation)
        {
            if (Llm.Instance == null) return "...";
            return await Llm.Instance.GenerateDialogue($"You are {Name}. Respond to the conversation.");
        }

        public async Task<string> CreateGiftResponse(StardewValley.Object gift, int taste)
        {
            if (Llm.Instance == null) return "...";
            return await Llm.Instance.GenerateDialogue($"You are {Name}. React to receiving {gift.Name}.");
        }

        public void ClearConversationHistory() { }
    }
}
