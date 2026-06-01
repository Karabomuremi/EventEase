using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Please select an event")]
        [Display(Name = "Event")]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required(ErrorMessage = "Please select a venue")]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Confirmed";
    }
}
