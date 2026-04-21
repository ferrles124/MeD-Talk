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

        private Dialogue _pendingDialogue = null;
        private bool _dialogueReady = false;

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

            if (_dialogueReady && _pendingDialogue != null)
            {
                _dialogueReady = false;
                var dialogue = _pendingDialogue;
                _pendingDialogue = null;
                try
                {
                    Game1.drawDialogue(dialogue.speaker);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error showing dialogue: {ex.Message}");
                }
            }
        }

        private async Task PerformGeneration()
        {
            try
            {
            Log.Info($"PerformGeneration started for type: {_awaitedType}, NPC: {_speakingNpc?.Name}");
                
                var npc = _speakingNpc;
                Dialogue newDialogue = null;

                switch (_awaitedType)
                {
                    case GenerationType.Basic:
                        newDialogue = await DialogueBuilder.Instance.Generate(npc, _currentDialogueKey, _originalLine);
                        break;
                    case GenerationType.Conversation:
                        var response = await DialogueBuilder.Instance.GenerateResponse(npc, _currentConversation, true);
                        newDialogue = new Dialogue(npc, _currentDialogueKey, response);
                        break;
                    case GenerationType.Gift:
                        newDialogue = await DialogueBuilder.Instance.GenerateGift(npc, _currentGift, _currentTaste);
                        break;
                }

                if (newDialogue != null && npc != null)
                {
                    npc.setNewDialogue(newDialogue.dialogues[0].Text, true, false);
                    _pendingDialogue = new Dialogue(npc, "medtalk", newDialogue.dialogues[0].Text);
                    _dialogueReady = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error generating dialogue: {ex.Message}");
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
