using StardewValley;

namespace MedTalk
{
    public class MedTalkDialogue : Dialogue
    {
        public MedTalkDialogue(string dialogueText) : base(dialogueText)
        {
        }
        
        public MedTalkDialogue(string dialogueText, NPC speaker) : base(dialogueText, speaker)
        {
        }
    }
}
