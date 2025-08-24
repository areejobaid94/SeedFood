using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SalesUserCreate.Dtos
{
   public class GetAllSalesUserCreateInput
    {
        public string Filter { get; set; }

        public DateTime? MaxCreationDateFilter { get; set; }
        public DateTime? MinCreationDateFilter { get; set; }
    }
}
