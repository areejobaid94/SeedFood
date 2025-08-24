using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class GetMeassagesInfoOutput
    {

        public string ErrorMsg { get; set; }
        public int SendMessagesCount { get; set; }
        public int CloseCount { get; set; }


        public int OrderCount { get; set; }

    }
}
