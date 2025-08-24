using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public enum CommunicationInitiated
    {
        business_initiated,
        service,
        referral_conversion,
        utility,
        marketing,
        authentication
    }

    public enum MessageStatusWhatsApp
    {
        Delivered = 1,
        Read = 2,
        Failed = 3,
        Deleted = 4,
        Warning = 5,
        Else = 6,

    }
  
}
