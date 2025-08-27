

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class CreateContactDto
    {
        public string phoneNumber { get; set; }
        public string name { get; set; }
        public int tenantId { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}
