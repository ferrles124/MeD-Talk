using System;
using System.Threading.Tasks;

namespace MedTalk
{
    internal abstract class Llm
    {
        internal static Llm Instance { get; private set; }

        internal static void SetLlm(Type llmType, string url = "", string promptFormat = "", string apiKey = "", string modelName = null)
        {
            Instance = (Llm)Activator.CreateInstance(llmType, apiKey, modelName, url);
        }

        protected string _apiKey;
        protected string _modelName;
        protected string _url;

        public abstract Task<string> GenerateDialogue(string prompt);
    }
}
