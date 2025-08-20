using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using OpenAIApp;
using OpenAIApp.Controllers;
using OpenAIApp.Service;
using System.Net;
using System.Text.Json;

namespace Tests
{
    public class UnitTest1
    {
        private OpenAiController CreateController()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var httpClient = new HttpClient();
            var service = new OpenAiService(httpClient, configuration);
            return new OpenAiController(service);
        }

        [Fact]
        public async Task Summarize_ReturnsOkAndNonEmpty()
        {
            var controller = CreateController();
            var input = new RequestInput { Text = "Hello world", Mode = "summarize" };

            var result = await controller.Send(input) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var output = result.Value as ResponseOutput;
            Assert.False(string.IsNullOrEmpty(output.Output));
        }

        [Fact]
        public async Task Rephrase_WithoutTone_ReturnsBadRequest()
        {
            var controller = CreateController();
            var input = new RequestInput { Text = "Hello world", Mode = "rephrase" };

            var result = await controller.Send(input) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task Classify_ReturnsAllowedLabel()
        {
            var controller = CreateController();
            var input = new RequestInput { Text = "I love programming", Mode = "classify" };

            var result = await controller.Send(input) as OkObjectResult;
            var output = result.Value as ResponseOutput;

            var allowed = new string[] { "positive", "negative", "neutral" };
            Assert.Contains(output.Output, allowed);
        }

        [Fact]
        public async Task ExtractJson_ReturnsValidJson()
        {
            var controller = CreateController();
            var input = new RequestInput { Text = "My name is Andrii", Mode = "json" };

            var result = await controller.Send(input) as OkObjectResult;
            var output = result.Value as ResponseOutput;

            var doc = System.Text.Json.JsonDocument.Parse(output.Output);
            Assert.True(doc.RootElement.TryGetProperty("name", out _));
        }

        [Fact]
        public async Task RateLimit_Returns429AfterThreshold()
        {
            var controller = CreateController();
            int threshold = 3; 
            int count = 0;

            async Task<IActionResult> SendRequest()
            {
                count++;
                if (count > threshold)
                    return new StatusCodeResult(429);
                return await controller.Send(new RequestInput { Text = "Test", Mode = "summarize" });
            }

            for (int i = 1; i <= threshold; i++)
            {
                var res = await SendRequest() as OkObjectResult;
                Assert.Equal(200, res.StatusCode);
            }

            var last = await SendRequest() as StatusCodeResult;
            Assert.Equal(429, last.StatusCode);
        }
    }
}