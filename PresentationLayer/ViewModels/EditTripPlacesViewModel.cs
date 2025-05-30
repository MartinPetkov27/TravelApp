using BusinessLayer;

namespace PresentationLayer.ViewModels
{
    public class EditTripPlacesViewModel
    {
        public Trip Trip { get; set; }
        public Country Country{ get; set; }
        public List<Place> Places{ get; set; }
    }
}
