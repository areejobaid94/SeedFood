using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public class HostTenantListDto
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string TenancyName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationTime { get; set; }
        public string InvoiceId { get; set; }
        public bool? Integration { get; set; }

        public string PhoneNumber { get; set; }

        public string DomainName { get; set; }
        public string CustomerName { get; set; }


    
    }
}
