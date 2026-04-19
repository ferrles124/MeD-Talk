using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedTalk
{
    internal abstract class Llm
    {
        internal static Llm Instance { get; private set; }

        internal static void SetLlm(Type llmType, string url = "", string promptFormat = "", string apiKey = "", string modelName = null)
        {
            var constructor = llmType.GetConstructors()[0];
            Instance = (Llm)Activator.CreateInstance(llmType, apiKey, modelName, url);
        }

        protected string _apiKey;
        protected string _modelName;
        protected string _url;

        public abstract Task<string> GenerateDialogue(string prompt);
    }

    internal class LlmGemini : Llm
    {
        public LlmGemini(string apiKey, string modelName, string url)
        {
            _apiKey = apiKey;
            _modelName = string.IsNullOrEmpty(modelName) ? "gemini-2.0-flash" : modelName;
            _url = url;
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            await Task.Delay(0);
            return "...";
        }
    }

    internal class LlmClaude : Llm
    {
        public LlmClaude(string apiKey, string modelName, string url)
        {
            _apiKey = apiKey;
            _modelName = string.IsNullOrEmpty(modelName) ? "claude-opus-4-5" : modelName;
            _url = "https://api.anthropic.com/v1/messages";
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            await Task.Delay(0);
            return "...";
        }
    }

    internal class LlmOpenAi : Llm
    {
        public LlmOpenAi(string apiKey, string modelName, string url)
        {
            _apiKey = apiKey;
            _modelName = string.IsNullOrEmpty(modelName) ? "gpt-4o" : modelName;
            _url = "https://api.openai.com/v1/chat/completions";
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            await Task.Delay(0);
            return "...";
        }
    }

    internal class LlmOAICompatible : Llm
    {
        public LlmOAICompatible(string apiKey, string modelName, string url)
        {
            _apiKey = apiKey;
            _modelName = modelName;
            _url = url;
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            await Task.Delay(0);
            return "...";
        }
    }
}
