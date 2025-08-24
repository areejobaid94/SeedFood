using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHookStatusFun
{
    public class WebHookModel
    {
        public WhatsAppModel whatsApp { get; set; }
        public TenantModel tenant { get; set; }
        public CustomerModel customer { get; set; }
        public int TenantId { get; set; }
    }
}
