using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class TripCriteria
    {
        public string Continent { get; set; }
        public string TargetGroup { get; set; }
        public int Duration { get; set; }
        public string Criteria { get; set; }
    }
    public class TripPlanCountry
    {
        public string Country { get; set; }
        public List<string> Places { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TripGeneratorController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public TripGeneratorController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTrip([FromBody] TripCriteria criteria)
        {
            var prompt = $@"
You are a travel planner. Based on the following inputs, generate a trip plan.

Continent: {criteria.Continent}
Target Group: {criteria.TargetGroup}
Duration: {criteria.Duration} days
Criteria: {criteria.Criteria}

Return JSON in this format:
[
  {{ ""Country"": ""France"", ""Places"": [""Paris"", ""Versailles""] }},
  ...
]
";

            var apiKey = _config["OpenAI:ApiKey"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4o", // Use gpt-4, gpt-4o, or gpt-3.5-turbo based on your plan
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var aiContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var tripPlan = JsonSerializer.Deserialize<List<TripPlanCountry>>(aiContent);

            return Ok(tripPlan); // Return the clean structured result
        }
    }
}
