using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public enum CommunicationInitiated
    {
        business_initiated,
        user_initiated,
        referral_conversion
    }

    public enum MessageStatusWhatsApp
    {
        Delivered = 1,
        Read = 2
    }
}
