namespace MedTalk
{
    internal class LlmResponse
    {
        public string Text { get; }
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public int StatusCode { get; }

        public LlmResponse(string text)
        {
            Text = text;
            IsSuccess = true;
        }

        public LlmResponse(string errorMessage, int statusCode)
        {
            Text = "";
            IsSuccess = false;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }
    }
}
