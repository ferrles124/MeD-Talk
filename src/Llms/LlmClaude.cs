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
    internal class LlmClaude : Llm, IGetModelNames
    {
        private readonly string _claudeApiKey;
        private readonly string _claudeModelName;

        public LlmClaude(string apiKey, string modelName, string url)
        {
            _claudeApiKey = apiKey;
            _claudeModelName = string.IsNullOrEmpty(modelName) ? "claude-3-5-haiku-latest" : modelName;
            _url = "https://api.anthropic.com/v1/messages";
        }

        public override async Task<string> GenerateDialogue(string prompt)
        {
            var inputString = JsonConvert.SerializeObject(new
            {
                model = _claudeModelName,
                max_tokens = 1024,
                system = new[] { new { type = "text", text = "You are a Stardew Valley NPC. Respond naturally and in character." } },
                messages = new[] { new { role = "user", content = prompt } }
            });

            int retry = 3;
            while (retry > 0)
            {
                try
                {
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                    var request = new HttpRequestMessage(HttpMethod.Post, _url);
                    request.Headers.Add("x-api-key", _claudeApiKey);
                    request.Headers.Add("anthropic-version", "2023-06-01");
                    request.Content = new StringContent(inputString, Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseJson = JObject.Parse(responseString);

                    var content = responseJson["content"] as JArray;
                    if (content != null && content.HasValues)
                    {
                        var text = content.FirstOrDefault()?["text"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(text))
                            return text;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
                retry--;
                Thread.Sleep(100);
            }
            return "...";
        }

        public string[] GetModelNames()
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.anthropic.com/v1/models");
                request.Headers.Add("x-api-key", _claudeApiKey);
                request.Headers.Add("anthropic-version", "2023-06-01");
                var response = client.SendAsync(request).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                var responseJson = JObject.Parse(responseString);
                var models = responseJson["data"] as JArray;
                var modelNames = new List<string>();
                if (models != null)
                    foreach (var model in models)
                        modelNames.Add(model["id"].ToString());
                return modelNames.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new string[] { };
            }
        }
    }

    internal interface IGetModelNames
    {
        string[] GetModelNames();
    }
}
