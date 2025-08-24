using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
   public  class UserModel
    {

        public string ErrorMsg { get; set; }
        public Guid? profilePictureUrl { get; set; }
        public int? TenantId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string userName { get; set; }
        public DateTime CreationTime { get; set; } 


        //public int ReadConversationCount { get; set; }
        //public int TotalOfConversation { get; set; }
        //public int NewConversationCount { get; set; }
        public int SendMessagesCount { get; set; }
        //public int ReceivedMessagesCount { get; set; }

        public int TotalOfClose { get; set; }
        public int TotalOfOrder { get; set; }
    }
}
