using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Zoho.Dto
{
    public class GenerateAccessTokenModel
    {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string api_domain { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
    }
}
