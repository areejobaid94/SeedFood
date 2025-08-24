using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class UserTicketsModel
    {
         public long Id { get; set; }
        public int MaximumTickets { get; set; } = 0;
        public int TicketsOpened { get; set; }=0;

        public bool IsOpen { get; set; }=true;
    }
}
