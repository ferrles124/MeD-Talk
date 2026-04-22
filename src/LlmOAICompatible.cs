using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedTalk
{
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
                if (!string.IsNullOrEmpty(_apiKey))
                    request.Headers.Add("Authorization", $"Bearer {_apiKey}");
                request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"OAI Compatible API Error: {response.StatusCode} - {responseString}");
                    return "...";
                }
                
                var responseJson = JObject.Parse(responseString);
                var text = responseJson["choices"]?[0]?["message"]?["content"]?.ToString();
                return string.IsNullOrEmpty(text) ? "..." : text;
            }
            catch (Exception ex)
            {
                Log.Error($"OAI Compatible error: {ex.Message}");
                return "...";
            }
        }
    }
}
