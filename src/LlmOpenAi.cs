using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedTalk
{
    internal class LlmOpenAi : Llm
    {
        public LlmOpenAi(string apiKey, string modelName, string url)
        {
            _apiKey = apiKey;
            _modelName = string.IsNullOrEmpty(modelName) ? "gpt-3.5-turbo" : modelName;
            _url = "https://api.openai.com/v1/chat/completions";
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
                var request = new HttpRequestMessage(HttpMethod.Post, _url);
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");
                request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"OpenAI API Error: {response.StatusCode} - {responseString}");
                    return "...";
                }
                
                var responseJson = JObject.Parse(responseString);
                var text = responseJson["choices"]?[0]?["message"]?["content"]?.ToString();
                return string.IsNullOrEmpty(text) ? "..." : text;
            }
            catch (Exception ex)
            {
                Log.Error($"OpenAI error: {ex.Message}");
                return "...";
            }
        }
    }
}
