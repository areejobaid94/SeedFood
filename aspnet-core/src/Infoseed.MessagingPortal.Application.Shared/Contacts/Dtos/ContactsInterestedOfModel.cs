using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class ContactsInterestedOfModel
    {
        public long id { get; set; }
        public int contactId { get; set; }
        public int tenantId { get; set; }
        public string LevelOneNameAr { get; set; }
        public string LevelOneNameEn { get; set; }
        public string LevelTwoNameAr { get; set; }
        public string LevelTwoNameEn { get; set; }
        public string LevelThreeNameAr { get; set; }
        public string LevelThreeNameEn { get; set; }

    }
}
