using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedTalk
{
    internal class LlmHuggingFace : Llm
    {
        public LlmHuggingFace(string apiKey, string modelName, string url = null)
        {
            _apiKey = apiKey;
            // Eğer model adı boşsa popüler ve güçlü bir ücretsiz model seçelim
            _modelName = string.IsNullOrEmpty(modelName) ? "meta-llama/Llama-3.1-8B-Instruct" : modelName;
            // Hugging Face standart API endpoint'i
            _url = $"https://api-inference.huggingface.co/models/{_modelName}";
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            Log.Info($"LlmHuggingFace.GenerateDialogue: {_modelName}");

            // Hugging Face Inference API genellikle basitleştirilmiş bir yapı bekler
            var payload = new
            {
                inputs = $"<|system|>\nYou are a Stardew Valley NPC. Respond naturally and in character.\n<|user|>\n{prompt}\n<|assistant|>",
                parameters = new
                {
                    max_new_tokens = 150,
                    return_full_text = false,
                    temperature = 0.7
                }
            };

            var inputString = JsonConvert.SerializeObject(payload);

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
                if (!string.IsNullOrEmpty(_apiKey))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var content = new StringContent(inputString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"Hugging Face Error: {response.StatusCode} - {responseString}");
                    return null;
                }

                // Hugging Face genellikle bir array içinde [{ "generated_text": "..." }] döner
                var responseJson = JArray.Parse(responseString);
                var text = responseJson[0]?["generated_text"]?.ToString();

                return string.IsNullOrEmpty(text) ? null : text.Trim();
            }
            catch (Exception ex)
            {
                Log.Error($"Hugging Face Exception: {ex.Message}");
                return null;
            }
        }
    }
}
