namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class GetContactForViewDto
    {
        public ContactDto Contact { get; set; }

        public string ChatStatuseChatStatusName { get; set; }

        public string ContactStatuseContactStatusName { get; set; }
        public int OrderCount { get; set; }
        public int DeliveryOrder { get; set; }
        public int TakeAwayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int loyalityPoint { get; set; }

    }
}