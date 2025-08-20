namespace OpenAIApp.Controllers
{
    public class RequestInput
    {
        public string Mode { get; set; } = string.Empty;
        public string? Tone { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
