using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Careem_Express
{
    public class SendLocationUserModel
    {
        public string query { get; set; }
        public int tenantID { get; set; }
        public bool isOrderOffer { get; set; }
        public decimal OrderTotal { get; set; }
        public int menu { get; set; }
        public string local { get; set; }
        public bool isChangeLocation { get; set; }
        public string address { get; set; }
    }
}
