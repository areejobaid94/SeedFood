using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.LiveChat.Dto
{
    public class LiveChatEntity
    {
        public List<CustomerLiveChatModel> lstLiveChat { get; set; }
        public int TotalCount { get; set; }
        public int TotalPending { get; set; }
        public int TotalOpen { get; set; }
        public int TotalClosed { get; set; }
        public int TotalExpired { get; set; }
        public decimal TotalResolutionTime { get; set; }


    }
}
