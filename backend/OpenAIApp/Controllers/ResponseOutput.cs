namespace OpenAIApp.Controllers
{
    public class ResponseOutput
    {
        public string Output {  get; set; } = string.Empty;
        public Usage Usage { get; set; } = new Usage();
    }

    public class Usage
    {
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
