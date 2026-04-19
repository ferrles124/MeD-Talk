namespace MedTalk
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public string Provider { get; set; } = "Anthropic";
        public string ModelName { get; set; } = "claude-opus-4-5";
        public string ApiKey { get; set; } = "";
        public string ServerAddress { get; set; } = "";
        public string PromptFormat { get; set; } = "";
        public bool Debug { get; set; } = false;
    }
}
