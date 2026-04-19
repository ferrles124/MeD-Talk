using System;
using System.Threading.Tasks;

namespace MedTalk
{
    public abstract class Llm
    {
        private static Llm _instance;

        public static void SetLlm(Type llmType, string modelName, string apiKey, string url, string promptFormat)
        {
            _instance = (Llm)Activator.CreateInstance(llmType);
            _instance.ModelName = modelName;
            _instance.ApiKey = apiKey;
            _instance.Url = url;
            _instance.PromptFormat = promptFormat;
        }

        public static Llm Instance => _instance;

        public string ModelName { get; set; }
        public string ApiKey { get; set; }
        public string Url { get; set; }
        public string PromptFormat { get; set; }

        public abstract Task<string> GenerateDialogue(string prompt);
    }

    public class LlmGemini : Llm
    {
        public override Task<string> GenerateDialogue(string prompt) => Task.FromResult("");
    }

    public class LlmClaude : Llm
    {
        public override Task<string> GenerateDialogue(string prompt) => Task.FromResult("");
    }

    public class LlmOpenAi : Llm
    {
        public override Task<string> GenerateDialogue(string prompt) => Task.FromResult("");
    }

    public class LlmOAICompatible : Llm
    {
        public override Task<string> GenerateDialogue(string prompt) => Task.FromResult("");
    }
}
