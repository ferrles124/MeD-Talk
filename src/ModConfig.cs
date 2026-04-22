namespace MedTalk
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public string Provider { get; set; } = "Google";
        public string ModelName { get; set; } = "gemini-1.5-flash";
        public string ApiKey { get; set; } = "";
        public string ServerAddress { get; set; } = "";
        public string PromptFormat { get; set; } = "";
        public bool Debug { get; set; } = false;
        public bool SuppressConnectionCheck { get; set; } = true;
        public int QueryTimeout { get; set; } = 30;
        public System.Collections.Generic.List<string> DisabledCharactersList { get; set; } = new System.Collections.Generic.List<string>();
    }
}
