using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                        {"OpenAiCompatible", typeof(LlmOAICompatible)}
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
            Config = Helper.ReadConfig<ModConfig>();

            if (!Config.EnableMod) return;

            Log.Initialize(Monitor);

            if (!LlmMap.TryGetValue(Config.Provider, out var llmType))
            {
                Log.Error($"Invalid LLM type: {Config.Provider}");
                return;
            }

            Llm.SetLlm(llmType, modelName: Config.ModelName, apiKey: Config.ApiKey, url: Config.ServerAddress, promptFormat: Config.PromptFormat);

            DialogueBuilder.Instance.Config = Config;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            Log.Debug($"[{DateTime.Now}] MedTalk loaded");
        }
    }
}
