
using BusinessLayer;

namespace PresentationLayer.ViewModels
{
    public class CreatePlaceFromTripViewModel
    {
        public Place Place { get; set; }
        public string TripName { get; set; }
        public int CountryIndex { get; set; }
    }
}
