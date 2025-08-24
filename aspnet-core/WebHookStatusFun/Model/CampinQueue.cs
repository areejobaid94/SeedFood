using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class CampinQueue
    {
        public string userId { get; set; }
        public string msg { get; set; }
        public string type { get; set; }
        public string templateId { get; set; }
        public string campaignId { get; set; }
        public string phoneNumber { get; set; }
        public string displayName { get; set; }
        public int TenantId { get; set; }
        public string D360Key { get; set; }
        public string AccessToken { get; set; }
        public string postBody { get; set; }
        public string functionName { get; set; }
    }
}
