using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public class BookingRequestModel
    {
        public int NumberOfBranches { get; set; }
        public int TotalNumberOfDepartmentsOrDoctor  { get; set; }
        public bool IsIncludedCustomerInquiry { get; set; }
    }
}
