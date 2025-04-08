using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using BusinessLayer;
using static PresentationLayer.Models.GeoNamesCountryDTO;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;


namespace ServiceLayer
{
    public class GeoNamesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _username;

        public GeoNamesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            //_username = username;
            _username = "MartinPetkov27"; 
        }

        public async Task<Country> GetCountryInfoAsync(string alpha2Code)
        {
            string url = $"http://api.geonames.org/countryInfoJSON?country={alpha2Code}&username={_username}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GeoNamesResponse>(json);

            var country = result.Geonames.FirstOrDefault();
            if (country == null) return null;

            return new Country
            {
                AlphaCode = country.IsoAlpha3,
                Name = country.CountryName,
                Capital = country.Capital,
                Currency = country.CurrencyCode,
                Language = country.Languages?.Split(',')[0]
            };
        }

        //It can be called in the seeding method to seed the database with all countries
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            Console.WriteLine("GeoNamesService: Starting request...");
            string url = $"http://api.geonames.org/countryInfoJSON?username={_username}";
            Console.WriteLine("Request URL: " + url);

            var response = await _httpClient.GetAsync(url);

            // Print the HTTP status
            Console.WriteLine("Response Status Code: " + response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("GeoNames request failed. Reason: " + response.ReasonPhrase);
                return null;
            }


            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Raw JSON from GeoNames: " + json);
            var result = JsonConvert.DeserializeObject<GeoNamesResponse>(json);

            List<Country> countries = new List<Country>();
            foreach (var geoName in result.Geonames)
            {
                Country country = new Country
                {
                    AlphaCode = geoName.IsoAlpha3,
                    Name = geoName.CountryName,
                    Capital = geoName.Capital,
                    Currency = geoName.CurrencyCode,
                    Language = geoName.Languages?.Split(',')[0]
                };
                countries.Add(country);
            }
            //return result.Geonames.Select(c => new Country
            //{
            //    AlphaCode = c.IsoAlpha3,
            //    Name = c.CountryName,
            //    Capital = c.Capital,
            //    Currency = c.CurrencyCode,
            //    Language = c.Languages?.Split(',')[0]
            //}).ToList();
            return countries;
        }
    }
}
