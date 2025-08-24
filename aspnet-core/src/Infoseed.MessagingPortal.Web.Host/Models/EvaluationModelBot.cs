using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models
{
    public class EvaluationModelBot
    {
        public string OrderNumber { get; set; }
        public string EvaluationText { get; set; }
        public int TenantId { get; set; }
    }
}
