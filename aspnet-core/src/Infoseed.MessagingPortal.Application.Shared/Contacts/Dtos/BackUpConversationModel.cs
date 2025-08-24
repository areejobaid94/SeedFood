using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
   public class BackUpConversationModel
    {
        public DateTime TextDate { get; set; }

        public string PhoneNumber { get; set; }
        public string UserName { get; set; }

        public string Text { get; set; }
        public string MediaUrl { get; set; }

    }
}
