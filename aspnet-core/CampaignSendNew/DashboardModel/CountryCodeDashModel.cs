using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSendNew.Model
{
    public class CountryCodeDashModel
    {
        public long Id { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string CountryCallingCode { get; set; }
        public string Currency { get; set; }
        public double MarketingPrice { get; set; }
        public double UtilityPrice { get; set; }
        public double AuthenticationPrice { get; set; }
        public double ServicePrice { get; set; }
        public string ISOCountryCodes { get; set; }
    }
}
