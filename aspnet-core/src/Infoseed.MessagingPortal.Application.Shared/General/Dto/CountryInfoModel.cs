using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.General.Dto
{
    public class CountryInfoModel
    {

        public string Country { get; set; }
        public string Pfx { get; set; }
        public string Iso { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }

        public CountryInfoModel(string country, string currency, string pfx, string iso,  decimal rate)
        {
            Country = country;
            Currency = currency;
            Pfx = pfx;
            Iso = iso;
            Rate = rate;
        }

    }
}
