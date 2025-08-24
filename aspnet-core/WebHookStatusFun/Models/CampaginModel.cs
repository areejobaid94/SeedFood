using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHookStatusFun.Models
{
    public class CampaginModel
    {
        public long rowId { get; set; }
        public long campaignId { get; set; }
        public string campaignName { get; set; }
        public string templateName { get; set; }
        public long templateId { get; set; }
        public bool IsExternal { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TenantId { get; set; }
        public long UserId { get; set; }
        public string JopName { get; set; }
        public string TemplateJson { get; set; }
        public List<ListContactToCampin> contacts { get; set; }

        public MessageTemplateModel model { get; set; }
        public TemplateVariablles templateVariablles { get; set; }
    }
}
