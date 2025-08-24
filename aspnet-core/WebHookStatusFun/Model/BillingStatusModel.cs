using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class BillingStatusModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Status { get; set; }
        public string Massage { get; set; }
        public bool IsActive { get; set; }
    }
}
