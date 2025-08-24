using Infoseed.MessagingPortal.WhatsApp.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class ContactsCampaignEntity
    {
        public List<ContactCampaignDto> contacts { get; set; }
        public int TotalCount { get; set; }


    }
}
