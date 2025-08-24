using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.UTracOrder.Dto;
using System;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppFunModel
    {

        public WhatsAppContactsDto whatsAppContactsDto { get; set; }
        public BookingModel BookingModel { get; set; }
        public long templateId { get; set; }
        public long campaignId { get; set; }
        public int TenantId { get; set; }
        public Guid GuidId { get; set; }
        public string UserId { get; set; }
        public bool IsContact { get; set; }
        public string Parameters { get; set; }
    }
}
