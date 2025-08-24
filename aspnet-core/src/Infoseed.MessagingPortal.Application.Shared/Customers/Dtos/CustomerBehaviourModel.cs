using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Customers.Dtos
{
    public class CustomerBehaviourModel
    {
        public int? TenantID { get; set; }
        public int ContactId { get; set; }
        public bool Stop { get; set; }
        public bool Start { get; set; }
        public int CustomerOPt { get; set; } = 0;
    }
}
