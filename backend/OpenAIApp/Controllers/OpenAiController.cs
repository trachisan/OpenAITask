using Microsoft.AspNetCore.Mvc;
using OpenAIApp.Service;

namespace OpenAIApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenAiController : ControllerBase
    {
        private readonly OpenAiService _service;

        public OpenAiController(OpenAiService service)
        {
            _service = service;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] RequestInput request) 
        {
            if (string.IsNullOrWhiteSpace(request.Text) || request.Text.Length > 5000)
                return BadRequest(new { error = "Text must be between 1 and 5000 characters." });

            if (request.Mode == "rephrase" && string.IsNullOrWhiteSpace(request.Tone))
                return BadRequest(new { error = "Tone is required for rephrase mode." });

            try
            {
                var output = await _service.SendRequest(request, request.Mode);
                return Ok(output);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
