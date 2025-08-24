using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.LiveChat.Dto
{
    public class LiveChatEnums
    {
        public enum LiveChatStatusEnum
        {
            NoN = 0,
            Pending = 1,
            Open = 2,
            Close = 3
        }
        public enum TypeEnum
        {
            NoN = 0,
            Ticket = 1,
            Request = 2,
        }
    }
}
