using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Booking.Dtos
{
    public class BookingEntity
    {
        public List<BookingModel> lstBookingModel { get; set; }
        public int TotalCount { get; set; }
        public int TotalPending { get; set; }
        public int TotalConfirmed { get; set; }
        public int TotalBooked { get; set; }
        public int TotalCanceled { get; set; }
        public int TotalDelete { get; set; }
    }
}
