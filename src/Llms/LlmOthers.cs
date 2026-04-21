using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedTalk
{
    internal class LlmGemini : Llm
{
    public LlmGemini(string apiKey, string modelName, string url)
    {
        _apiKey = apiKey;
        _modelName = string.IsNullOrEmpty(modelName) ? "gemini-2.5-flash" : modelName;
        _url = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:generateContent";
    }

    public override async Task<string> GenerateDialogue(string prompt)
    {
        var inputString = JsonConvert.SerializeObject(new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { maxOutputTokens = 150 }
        });

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var requestUrl = $"{_url}?key={_apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            Log.Info($"Gemini response: {responseString.Substring(0, Math.Min(200, responseString.Length))}");
            var responseJson = JObject.Parse(responseString);
            return responseJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "...";
        }
        catch (Exception ex)
        {
            Log.Error($"Gemini error: {ex.Message}");
            return "...";
        }
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
            var inputString = JsonConvert.SerializeObject(new
            {
                model = _modelName,
                messages = new[] { new { role = "user", content = prompt } }
            });

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                var request = new HttpRequestMessage(HttpMethod.Post, _url);
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");
                request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseString);
                return responseJson["choices"]?[0]?["message"]?["content"]?.ToString() ?? "...";
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "...";
            }
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
            var inputString = JsonConvert.SerializeObject(new
            {
                model = _modelName,
                messages = new[] { new { role = "user", content = prompt } }
            });

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                var request = new HttpRequestMessage(HttpMethod.Post, _url);
                if (!string.IsNullOrEmpty(_apiKey))
                    request.Headers.Add("Authorization", $"Bearer {_apiKey}");
                request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseString);
                return responseJson["choices"]?[0]?["message"]?["content"]?.ToString() ?? "...";
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "...";
            }
        }
    }

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
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                var request = new HttpRequestMessage(HttpMethod.Post, _url);
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");
                request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseString);
                return responseJson["choices"]?[0]?["message"]?["content"]?.ToString() ?? "...";
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "...";
            }
        }
    }
}
