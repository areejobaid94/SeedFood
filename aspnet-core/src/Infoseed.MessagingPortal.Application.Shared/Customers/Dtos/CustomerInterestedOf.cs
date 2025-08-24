using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Customers.Dtos
{
    public class CustomerInterestedOf
    {
        public int? TenantID { get; set; }
        public int ContactId { get; set; }
        public int levleOneId { get; set; }
        public int? levelTwoId { get; set; }
        public int? levelThreeId { get; set; }
    }
}
