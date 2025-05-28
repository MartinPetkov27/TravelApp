using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceLayer
{
    public class TripManager
    {
        private readonly TripContext tripContext;
        //private readonly IHttpClientFactory httpClientFactory;
        //private readonly IConfiguration config;

        //Methods for managing the tripContext for CRUD operations in Trip
        public TripManager(TripContext tripContext /*IHttpClientFactory httpClientFactory, IConfiguration config*/)
        {
            this.tripContext = tripContext;
            //this.httpClientFactory = httpClientFactory;
            //this.config = config;
        }

        public async Task CreateAsync(Trip item)
        {
            await tripContext.CreateAsync(item);
        }
        public async Task<Trip> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await tripContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }
        public async Task<Trip> GetTripByNameAsync(string title)
        {
            ICollection<Trip> trips = await tripContext.ReadAllAsync(true, true);
            return trips.FirstOrDefault(t => t.Title == title);
        }
        public async Task<ICollection<Trip>> ReadAllAsync(bool useNAvigationalProperties = false, bool isReadOnly = true)
        {
            return await tripContext.ReadAllAsync(useNAvigationalProperties, isReadOnly);
        }
        public async Task UpdateAsync(Trip item, bool useNaigationalProperties = false)
        {
            await tripContext.UpdateAsync(item, useNaigationalProperties);
        }
        public async Task DeleteAsync(int key)
        {
            await tripContext.DeleteAsync(key);
        }
        //public class TripCriteria
        //{
        //    public string Continent { get; set; }
        //    public string TargetGroup { get; set; }
        //    public int Duration { get; set; }
        //    public string Criteria { get; set; }
        //}
        public class TripPlanCountry
        {
            public string Country { get; set; }
            public List<string> Places { get; set; }
        }

//        public async Task<List<TripPlanCountry>> GenerateTripPlanAsync(string Continent, string TargetGroup, int Duration, string Criteria)
//        {
//            var prompt = $@"
//You are a travel planner. Based on the following inputs, generate a trip plan.

//Continent: {Continent}
//Target Group: {TargetGroup}
//Duration: {Duration} days
//Criteria: {Criteria}

//Return JSON in this format:
//[
//  {{ ""Country"": ""France"", ""Places"": [""Paris"", ""Versailles""] }},
//  ...
//]
//";

//            var apiKey = config["OpenAI:ApiKey"];
//            var client = httpClientFactory.;
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

//            var requestBody = new
//            {
//                model = "gpt-4o",
//                messages = new[]
//                {
//            new { role = "user", content = prompt }
//        },
//                temperature = 0.7
//            };

//            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
//            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"OpenAI API call failed: {await response.Content.ReadAsStringAsync()}");
//            }

//            var responseString = await response.Content.ReadAsStringAsync();

//            using var doc = JsonDocument.Parse(responseString);
//            var aiContent = doc.RootElement
//                .GetProperty("choices")[0]
//                .GetProperty("message")
//                .GetProperty("content")
//                .GetString();

//            return JsonSerializer.Deserialize<List<TripPlanCountry>>(aiContent);
//        }
    }
}
