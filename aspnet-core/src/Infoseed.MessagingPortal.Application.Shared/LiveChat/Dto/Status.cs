using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.LiveChat.Dto
{
    public class Status
    {
        public int TenantId { get; set; }
        public int LiveChatStatus { get; set; }
        public int ContactId { get; set; }
        public string phoneNumber { get; set; }
        public string CategoryType { get; set; }

    }
}
