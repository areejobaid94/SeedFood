using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto
{
   public  class GetMeassagesInfoHostOutput
    {

        public int TotalOfConvsersations { get; set; }
        public int TotalNumberOfCustomers { get; set; }

        public int SendMessagesCount { get; set; }
        public int ReceivedMessagesCount { get; set; }

        //public int NewConversationCount { get; set; }


        //public int ConversationsExpiryCount { get; set; }
        //public int ToltalOpenConversations { get; set; }
        //public int ToltalCloseConversations { get; set; }

    }
}
