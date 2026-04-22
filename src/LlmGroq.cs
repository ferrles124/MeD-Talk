using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedTalk
{
    internal class LlmGroq : Llm
    {
        public LlmGroq(string apiKey, string modelName, string url)
        {
            _apiKey = apiKey;
            _modelName = string.IsNullOrEmpty(modelName) ? "llama-3.3-70b-versatile" : modelName;
            _url = "https://api.groq.com/openai/v1/chat/completions";
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            var inputString = JsonConvert.SerializeObject(new
            {
                model = _modelName,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 150
            });

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
                var content = new StringContent(inputString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"Groq API Error: {response.StatusCode} - {responseString}");
                    return "...";
                }
                
                var responseJson = JObject.Parse(responseString);
                var text = responseJson["choices"]?[0]?["message"]?["content"]?.ToString();
                return string.IsNullOrEmpty(text) ? "..." : text;
            }
            catch (Exception ex)
            {
                Log.Error($"Groq error: {ex.Message}");
                return "...";
            }
        }
    }
}
