using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MedTalk
{
    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static IModHelper SHelper { get; private set; }
        public static ModConfig Config;

        public static Dictionary<string, Type> LlmMap
        {
            get
            {
                if (_llmMap == null)
                {
                    _llmMap = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        {"Google", typeof(LlmGemini)},
                        {"Anthropic", typeof(LlmClaude)},
                        {"OpenAI", typeof(LlmOpenAi)},
                        {"OpenAiCompatible", typeof(LlmOAICompatible)},
                        {"Groq", typeof(LlmGroq)}
                        {"HuggingFace", typeof(LlmHuggingFace)},
                    };
                }
                return _llmMap;
            }
        }

        public static bool BlockModdedContent { get; private set; } = false;
        private static Dictionary<string, Type> _llmMap;

        public override void Entry(IModHelper helper)
        {
            SHelper = helper;
            SMonitor = Monitor;

            try
            {
                Config = Helper.ReadConfig<ModConfig>();
            }
            catch
            {
                Config = new ModConfig();
            }

            if (!Config.EnableMod)
            {
                Monitor.Log("MedTalk disabled in config.", LogLevel.Info);
                return;
            }

            Log.Initialize(Monitor);
            Log.Info("MedTalk starting...");

            if (!LlmMap.TryGetValue(Config.Provider, out var llmType))
            {
                Log.Error($"Invalid provider: {Config.Provider}. Use: Google, Anthropic, OpenAI, Groq");
                return;
            }

            Log.Info($"Using provider: {Config.Provider}, model: {Config.ModelName}");

            try
            {
                Llm.SetLlm(llmType, apiKey: Config.ApiKey, modelName: Config.ModelName, url: Config.ServerAddress);
                Log.Info($"LLM initialized: {Llm.Instance?.GetType().Name}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to initialize LLM: {ex.Message}");
                return;
            }

            DialogueBuilder.Instance.Config = Config;
            TextInputManager.Initialize();

            try
            {
                var harmony = new Harmony(ModManifest.UniqueID);
                harmony.PatchAll();
                Log.Info("Harmony patches applied");
            }
            catch (Exception ex)
            {
                Log.Error($"Harmony patch failed: {ex.Message}");
                return;
            }

            Log.Info("MedTalk loaded successfully!");
        }
    }
}
