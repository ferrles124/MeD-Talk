namespace MedTalk
{
    public class ValleyTalkInterface
    {
        public bool IsGenerating => AsyncBuilder.Instance.AwaitingGeneration;
    }
}
