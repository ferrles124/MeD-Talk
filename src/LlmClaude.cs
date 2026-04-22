using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedTalk
{
    internal class LlmClaude : Llm
    {
        public LlmClaude(string apiKey, string modelName, string url)
        {
            _apiKey = apiKey;
            _modelName = string.IsNullOrEmpty(modelName) ? "claude-3-haiku-20240307" : modelName;
            _url = "https://api.anthropic.com/v1/messages";
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            var inputString = JsonConvert.SerializeObject(new
            {
                model = _modelName,
                max_tokens = 150,
                system = "You are a Stardew Valley NPC. Respond naturally and in character. Keep responses short (1-2 sentences).",
                messages = new[] { new { role = "user", content = prompt } }
            });

            int retry = 2;
            while (retry >= 0)
            {
                try
                {
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
                    var request = new HttpRequestMessage(HttpMethod.Post, _url);
                    request.Headers.Add("x-api-key", _apiKey);
                    request.Headers.Add("anthropic-version", "2023-06-01");
                    request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);
                    var responseString = await response.Content.ReadAsStringAsync();
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        Log.Error($"Claude API Error: {response.StatusCode} - {responseString}");
                        retry--;
                        if (retry >= 0) await Task.Delay(100);
                        continue;
                    }
                    
                    var responseJson = JObject.Parse(responseString);
                    var content = responseJson["content"] as JArray;
                    if (content != null && content.HasValues)
                    {
                        var text = content.FirstOrDefault()?["text"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(text))
                            return text;
                    }
                    return "...";
                }
                catch (Exception ex)
                {
                    Log.Error($"Claude error: {ex.Message}");
                    retry--;
                    if (retry >= 0) await Task.Delay(100);
                }
            }
            return "...";
        }
    }
}
