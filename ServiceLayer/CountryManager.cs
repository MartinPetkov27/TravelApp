using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;

namespace ServiceLayer
{
    public class CountryManager
    {
        private readonly CountryContext countryContext;

        //Methods for managing the countryContext for CRUD operaatin in Country
        public CountryManager(CountryContext countryContext)
        { 
            this.countryContext = countryContext;
        }

        public async Task CreateAsync(Country item)
        {
            await countryContext.CreateAsync(item);
        }

        public async Task<Country> ReadAsync(string key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await countryContext.ReadAsync(key, useNavigationalProperties, isReadOnly);
        }

        public async Task<ICollection<Country>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            return await countryContext.ReadAllAsync(useNavigationalProperties, isReadOnly);
        }
        public async Task<Country> GetCountryByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var normalizedInput = name.Trim().ToLowerInvariant();

            var countries = await ReadAllAsync(); // Fetch from DB or in-memory list

            // Step 1: Try exact match (case-insensitive)
            var exactMatch = countries.FirstOrDefault(c =>
                c.Name.Trim().Equals(normalizedInput, StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
                return exactMatch;

            // Step 2: Try contains (Google may return "United States" instead of "United States of America")
            var partialMatch = countries.FirstOrDefault(c =>
                c.Name.Trim().ToLowerInvariant().Contains(normalizedInput) ||
                normalizedInput.Contains(c.Name.Trim().ToLowerInvariant()));

            if (partialMatch != null)
                return partialMatch;

            // Step 3 (optional): Use Levenshtein distance or similar fuzzy match library
            // Can implement this or use a NuGet package like FuzzySharp

            return null;
        }

        public async Task<Country> GetCountryByAlphaCode(string alphaCode)
        {
            var countries = await countryContext.ReadAllAsync(false, true);
            return countries.FirstOrDefault(c => c.AlphaCode.Equals(alphaCode, StringComparison.OrdinalIgnoreCase));
        }

        public async Task UpdateAsync(Country item, bool useNavigationalProperties = false)
        {
            await countryContext.UpdateAsync(item, useNavigationalProperties);
        }

        public async Task DeleteAsync(string key)
        {
            await countryContext.DeleteAsync(key);
        }

        public async Task<string> GetCountryNameFromCoordinates(double lat, double lng)
        {
            var httpClient = new HttpClient();
            var apiKey = "AIzaSyBVCMcaodMWF5rbNUVo7YdZaAIHPC3draE"; // Use configuration for this
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lng}&key={apiKey}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var geocodeData = JsonDocument.Parse(json);

            var results = geocodeData.RootElement.GetProperty("results");

            foreach (var result in results.EnumerateArray())
            {
                foreach (var component in result.GetProperty("address_components").EnumerateArray())
                {
                    if (component.GetProperty("types").EnumerateArray().Any(t => t.GetString() == "country"))
                    {
                        return component.GetProperty("long_name").GetString(); // Full country name
                    }
                }
            }

            return null;
        }
    }
}
