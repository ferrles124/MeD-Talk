using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley;

namespace MedTalk
{
    public class AsyncBuilder
    {
        private static AsyncBuilder _instance = new AsyncBuilder();
        public static AsyncBuilder Instance => _instance;

        private bool _awaitingGeneration = false;
        private GenerationType _awaitedType = GenerationType.None;
        private NPC _speakingNpc = null;
        private string _currentDialogueKey = "";
        private string _originalLine = null;
        private List<ConversationElement> _currentConversation = null;
        private StardewValley.Object _currentGift = null;
        private int _currentTaste = 0;

        public bool AwaitingGeneration => _awaitingGeneration;
        public NPC SpeakingNpc => _speakingNpc;

        private AsyncBuilder()
        {
            ModEntry.SHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (_awaitingGeneration && Game1.activeClickableMenu == null)
            {
                _awaitingGeneration = false;
                _ = PerformGeneration();
            }
        }

        private async Task PerformGeneration()
        {
            try
            {
                var npc = _speakingNpc;
                Dialogue newDialogue = null;

                switch (_awaitedType)
                {
                    case GenerationType.Basic:
                        newDialogue = await DialogueBuilder.Instance.Generate(npc, _currentDialogueKey, _originalLine);
                        break;
                    case GenerationType.Conversation:
                        var response = await DialogueBuilder.Instance.GenerateResponse(npc, _currentConversation, true);
                        newDialogue = new Dialogue(response ?? "...", npc);
                        break;
                    case GenerationType.Gift:
                        newDialogue = await DialogueBuilder.Instance.GenerateGift(npc, _currentGift, _currentTaste);
                        break;
                }

                if (newDialogue != null)
                {
                    Game1.DrawDialogue(newDialogue);
                }
            }
            catch (Exception ex)
            {
                ModEntry.SMonitor?.Log($"AsyncBuilder error: {ex.Message}", StardewModdingAPI.LogLevel.Error);
                if (_speakingNpc != null)
                {
                    var fallbackDialogue = new Dialogue("...", _speakingNpc);
                    Game1.DrawDialogue(fallbackDialogue);
                }
            }
            finally
            {
                Reset();
            }
        }

        private void Reset()
        {
            _awaitingGeneration = false;
            _speakingNpc = null;
            _currentDialogueKey = "";
            _originalLine = null;
            _currentConversation = null;
            _currentGift = null;
            _currentTaste = 0;
            _awaitedType = GenerationType.None;
        }

        internal void RequestNpcBasic(NPC npc, string key, string original)
        {
            if (_awaitingGeneration) return;
            _speakingNpc = npc;
            _currentDialogueKey = key;
            _originalLine = original;
            _awaitedType = GenerationType.Basic;
            _awaitingGeneration = true;
        }

        internal void RequestNpcResponse(NPC npc, List<ConversationElement> conversation)
        {
            if (_awaitingGeneration) return;
            _speakingNpc = npc;
            _currentConversation = conversation;
            _awaitedType = GenerationType.Conversation;
            _awaitingGeneration = true;
        }

        internal void RequestNpcGiftResponse(NPC npc, StardewValley.Object gift, int taste)
        {
            if (_awaitingGeneration) return;
            _speakingNpc = npc;
            _currentGift = gift;
            _currentTaste = taste;
            _awaitedType = GenerationType.Gift;
            _awaitingGeneration = true;
        }
    }

    internal enum GenerationType
    {
        None, Basic, Conversation, Gift
    }
}
