using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class _360DialogModel
    {
        public WebHookD360Model messages { get; set; }
        public _360DialogStatus statuses { get; set; }
        public object errors { get; set; }
    }

    public class _360DialogStatus
    {
        public int id { get; set; }
        public _360Dialogconversation conversation { get; set; }
    }
    public class _360Dialogconversation
    {
        public string id { get; set; }
        public _360DialogOrgin origin { get; set; }
        public object expiration_timestamp { get; set; }
    }

    public class _360DialogOrgin
    {
        public string type { get; set; }

        /*Indicates where a conversation has started. This can also be referred to as a conversation entry point. Currently, the available options are:

business_initiated: indicates that the conversation started by a business sending the first message to a user. This applies any time it has been more than 24 hours since the last user message.
user_initiated: indicates that the conversation started by a business replying to a user message. This applies only when the business reply is within 24 hours of the last user message.
referral_conversion: indicates that the conversation originated from a free entry point. These conversations are always user-initiated.*/
    }
}
