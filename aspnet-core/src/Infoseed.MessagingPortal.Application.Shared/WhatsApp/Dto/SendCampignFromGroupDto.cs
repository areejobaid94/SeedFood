using System.Collections.Generic;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class SendCampignFromGroupDto
    {
        public long groupId { get; set; }
        public long campaignId { get; set; }
        public long templateId { get; set; }
        public string language { get; set; }
        public string sendTime { get; set; }
        public bool isExternal { get; set; }
        public int campaignStatus { get; set; }
        public List<ListContact> listContact { get; set; }
    }
    public class ListContact
    {
        public string contactName { get; set; }
        public string phoneNumber { get; set; } 
        public string templateVariables { get; set; }
    }

}
