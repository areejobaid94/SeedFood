using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
    public class TenantInfoForOrdaringSystemDto
    {

        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string LogoImag { get; set; }
        public string BgImag { get; set; }

        public string Name { get; set; }

        public string NameEnglish { get; set; }
        public string CurrencyCode { get; set; }

        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsApplyLoyalty { get; set; }


        public string ContactDisplayName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string OrderType { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


    }
}
