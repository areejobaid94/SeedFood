using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Booking.Dtos
{
    public class BookingContact
    {
        public long Id { get; set; }
        public long? BookingId { get; set; }
        public int TenantId { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsSent { get; set; }
        public string MessageId { get; set; }
        public decimal MessageRate { get; set; }
        public int TemplateTypeId { get; set; }
        public bool IsReminderSent { get; set; }
    }
}
