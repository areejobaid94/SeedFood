using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Authorization.Accounts.Dto
{
    public class LoginFaceBookInfo
    {

        
            public string verified_name { get; set; }
            public string code_verification_status { get; set; }
            public string display_phone_number { get; set; }
            public string quality_rating { get; set; }
            public string platform_type { get; set; }
            public Throughput throughput { get; set; }
            public DateTime last_onboarded_time { get; set; }
            public Webhook_Configuration webhook_configuration { get; set; }
            public string id { get; set; }
        

        public class Throughput
        {
            public string level { get; set; }
        }

        public class Webhook_Configuration
        {
            public string application { get; set; }
        }

    }
}
