using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.WhatsAppApi
{
    public enum WhatsContentTypeEnum
    {

        video,
        audio,
        document,
        text,
        image,
        file,
        location,
        interactive,
    }

    public enum WhatsAppCampaignStatusEnum
    {
        Draft = 0,
        Sent = 1,
        Scheduled = 2,
        Active = 3,
        InActive = 4,
    }
    public enum WhatsAppTemplateStatusEnum
    {
        PENDING = 1,
        APPROVED = 2,
        REJECTED = 3

    }
}
