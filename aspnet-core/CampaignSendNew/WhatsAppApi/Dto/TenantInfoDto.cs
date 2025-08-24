using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.WhatsAppApi.Dto
{
    public class TenantInfoDto
    {
        public int TenantId { get; set; }
        public string AccessToken { get; set; }
        public string D360Key { get; set; }
    }
}
