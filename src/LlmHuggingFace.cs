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
            _modelName = string.IsNullOrEmpty(modelName) ? "microsoft/DialoGPT-medium" : modelName;
            _url = $"https://api-inference.huggingface.co/models/{_modelName}";
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            var payload = new
            {
                inputs = prompt,
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
                    Log.Error($"HuggingFace API Error: {response.StatusCode} - {responseString}");
                    return "...";
                }

                var responseJson = JArray.Parse(responseString);
                var text = responseJson[0]?["generated_text"]?.ToString();
                return string.IsNullOrEmpty(text) ? "..." : text.Trim();
            }
            catch (Exception ex)
            {
                Log.Error($"HuggingFace error: {ex.Message}");
                return "...";
            }
        }
    }
}
