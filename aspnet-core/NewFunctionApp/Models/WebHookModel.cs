using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFunctionApp.Models
{
    public class WebHookModel
    {
        public WhatsAppModel whatsApp { get; set; }
        public TenantModel tenant { get; set; }
    }
}
