using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Event Type")]
        public int? EventTypeId { get; set; }
        public EventType? EventType { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
