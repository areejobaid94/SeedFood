using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Menu
{
    public class GetInfoTenantModel
    {
        public string LogoImag { get; set; }
        public string BgImag { get; set; }

        public string Name { get; set; }

        public string NameEnglish { get; set; }
        public string CurrencyCode { get; set; }
    }
}
