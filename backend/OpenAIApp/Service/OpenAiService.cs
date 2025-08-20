using OpenAIApp.Controllers;
using Sprache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OpenAIApp.Service
{
    public class OpenAiService
    {
        private readonly string secretKey;
        private readonly HttpClient httpClient;
        private readonly string url = "https://api.openai.com/v1/responses";
        private readonly Dictionary<string, string> promptsId;
        private readonly List<string> tones = new List<string>() { "casual", "professional", "friendly" };



        public OpenAiService(HttpClient httpClient, IConfiguration configuration)
        {
            secretKey = configuration["API_KEY"] ?? throw new ArgumentNullException("API is not valid!");
            promptsId = new Dictionary<string, string>
            {
                { "summarize", configuration["Prompt-Summarize"] ?? string.Empty },
                { "classify", configuration["Prompt-Classify"] ?? string.Empty },
                { "json", configuration["Prompt-Json"] ?? string.Empty },
                { "rephrase", configuration["Prompt-Rephrase"] ?? string.Empty }
            };
            this.httpClient = httpClient;
        }

        public async Task<ResponseOutput> SendRequest(RequestInput requestText, string prompt)
        {
            if(requestText.Mode == "rephrase")
            {
                if (string.IsNullOrEmpty(requestText.Tone) || !tones.Contains(requestText.Tone))
                    throw new ArgumentException("Tone is required or incorrect for rephrase mode.");
                else
                    requestText.Text += $"\n Rephrase in a {requestText.Tone.ToLower()} tone:";
            }

            var requestBody = new
            {
                model = "gpt-5",
                prompt = new
                {
                    id = promptsId[prompt]
        },
                input = requestText.Text
            };

            var json = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(request);
            string output = await response.Content.ReadAsStringAsync();

            return CreateResponse(output);
        }

        private ResponseOutput CreateResponse(string output)
        {
            JsonDocument doc = JsonDocument.Parse(output);
            var result = doc.RootElement
                .GetProperty("output")[1]
                .GetProperty("content")[0]
                .GetProperty("text").GetString();
            int inputTokens = doc.RootElement.GetProperty("usage").GetProperty("input_tokens").GetInt32();
            int outputTokens = doc.RootElement.GetProperty("usage").GetProperty("output_tokens").GetInt32();
            int totalTokens = doc.RootElement.GetProperty("usage").GetProperty("total_tokens").GetInt32();

            return new ResponseOutput() { Output = result, Usage = new Usage() { InputTokens = inputTokens, OutputTokens = outputTokens, TotalTokens = totalTokens } };
        }
    }
}
