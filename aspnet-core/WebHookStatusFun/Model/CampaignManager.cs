using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHookStatusFun.Model
{
    public class CampaignManager
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string PhoneNumber { get; set; }
        public string MassageId { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string FaildDetails { get; set; }
        public string DetailsJosn { get; set; }
    }
}
