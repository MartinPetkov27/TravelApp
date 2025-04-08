using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PresentationLayer.Models
{
    public class GeoNamesCountryDTO
    {
        public class GeoNamesResponse
        {
            [JsonProperty("geonames")]
            public List<GeoNamesCountry> Geonames { get; set; }
        }

        public class GeoNamesCountry
        {
            [JsonProperty("countryCode")]
            public string CountryCode { get; set; } // Alpha-2

            [JsonProperty("countryName")]
            public string CountryName { get; set; }

            [JsonProperty("capital")]
            public string Capital { get; set; }

            [JsonProperty("currencyCode")]
            public string CurrencyCode { get; set; }

            [JsonProperty("languages")]
            public string Languages { get; set; }

            [JsonProperty("isoAlpha3")]
            public string IsoAlpha3 { get; set; } // Store this in your Country model
        }
    }
}
