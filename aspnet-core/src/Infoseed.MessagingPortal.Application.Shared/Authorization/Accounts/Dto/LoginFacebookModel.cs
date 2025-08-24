using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Authorization.Accounts.Dto
{
    public class LoginFacebookModel
    {
        public string phone_number_id { get; set; }
        public string waba_id { get; set; }
        public string code { get; set; }
    }
}
