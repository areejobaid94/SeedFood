using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public class SellingRequestDetailsDto
    {

        public long Id { get; set; }
        public int TenantId { get; set; }
        public int DocumentTypeId { get; set; }
        public long SellingRequestId { get; set; }
        public string DocumentURL { get; set; }


    }
}
