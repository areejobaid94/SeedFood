using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class ContactEntity
    {
        public List<ContactDto> lstContacts { get; set; }
        public int TotalCount { get; set; }
    }
}
