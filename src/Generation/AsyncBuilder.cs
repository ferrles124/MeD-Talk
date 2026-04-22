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

            var npc = _speakingNpc;
            _speakingNpc = null;

            if (_pendingTask.IsFaulted)
            {
                Log.Error($"Task faulted: {_pendingTask.Exception?.GetBaseException().Message}");
                _pendingTask = null;
                return;
            }

            var text = _pendingTask.Result;
            _pendingTask = null;

            Log.Info($"Response received: '{text}'");

            if (string.IsNullOrWhiteSpace(text) || text == "...")
            {
                Log.Info("Empty or default response, skipping");
                return;
            }

            if (npc == null)
            {
                Log.Info("NPC is null, skipping");
                return;
            }

            try
            {
                npc.setNewDialogue(text, true, false);
                Game1.drawDialogue(npc);
                Log.Info($"Dialogue shown for {npc.Name}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error showing dialogue: {ex.Message}");
            }
        }

        internal void RequestNpcBasic(NPC npc, string key, string original)
        {
            if (AwaitingGeneration)
            {
                Log.Info("Already awaiting, skipping");
                return;
            }

            if (Llm.Instance == null)
            {
                Log.Error("Llm.Instance is null! Check config.");
                return;
            }

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
            if (Llm.Instance == null) { Log.Error("Llm.Instance is null!"); return; }
            _speakingNpc = npc;
            AwaitingGeneration = true;
            _taskStarted = true;
            var character = DialogueBuilder.Instance.GetCharacter(npc);
            _pendingTask = character.CreateResponse(conversation);
        }

        internal void RequestNpcGiftResponse(NPC npc, StardewValley.Object gift, int taste)
        {
            if (AwaitingGeneration) return;
            if (Llm.Instance == null) { Log.Error("Llm.Instance is null!"); return; }
            _speakingNpc = npc;
            AwaitingGeneration = true;
            _taskStarted = true;
            var character = DialogueBuilder.Instance.GetCharacter(npc);
            _pendingTask = character.CreateGiftResponse(gift, taste);
        }
    }
}
