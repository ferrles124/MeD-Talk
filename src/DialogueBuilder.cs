namespace MedTalk
{
    public class DialogueBuilder
    {
        private static DialogueBuilder _instance;
        public static DialogueBuilder Instance => _instance ??= new DialogueBuilder();
        public ModConfig Config { get; set; }
    }
}
