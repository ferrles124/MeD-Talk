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

        public bool AwaitingGeneration { get; private set; } = false;
        public NPC SpeakingNpc => _speakingNpc;

        private NPC _speakingNpc = null;
        private Task<string> _pendingTask = null;
        private bool _taskStarted = false;

        private AsyncBuilder()
        {
            ModEntry.SHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!_taskStarted || _pendingTask == null) return;

            if (!_pendingTask.IsCompleted) return;

            _taskStarted = false;
            AwaitingGeneration = false;

            if (_pendingTask.IsFaulted)
            {
                Log.Error($"Task failed: {_pendingTask.Exception?.Message}");
                _pendingTask = null;
                _speakingNpc = null;
                return;
            }

            var text = _pendingTask.Result;
            _pendingTask = null;

            if (string.IsNullOrEmpty(text) || text == "...") 
            {
                Log.Info("Empty response received");
                _speakingNpc = null;
                return;
            }

            Log.Info($"Got response: {text}");

            var npc = _speakingNpc;
            _speakingNpc = null;

            if (npc == null) return;

            npc.setNewDialogue(text, true, false);
            Game1.drawDialogue(npc);
        }

        internal void RequestNpcBasic(NPC npc, string key, string original)
        {
            if (AwaitingGeneration) return;

            _speakingNpc = npc;
            AwaitingGeneration = true;
            _taskStarted = true;

            var character = DialogueBuilder.Instance.GetCharacter(npc);
            _pendingTask = character.CreateDialogue(original);

            Log.Info($"Task started for {npc.Name}");
        }

        internal void RequestNpcResponse(NPC npc, List<ConversationElement> conversation)
        {
            if (AwaitingGeneration) return;
            _speakingNpc = npc;
            AwaitingGeneration = true;
            _taskStarted = true;
            var character = DialogueBuilder.Instance.GetCharacter(npc);
            _pendingTask = character.CreateResponse(conversation);
        }

        internal void RequestNpcGiftResponse(NPC npc, StardewValley.Object gift, int taste)
        {
            if (AwaitingGeneration) return;
            _speakingNpc = npc;
            AwaitingGeneration = true;
            _taskStarted = true;
            var character = DialogueBuilder.Instance.GetCharacter(npc);
            _pendingTask = character.CreateGiftResponse(gift, taste);
        }
    }
}
