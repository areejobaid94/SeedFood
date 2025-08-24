using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public class SellingRequestEntity
    {
        public List<SellingRequestDto> lstSellingRequestDto { get; set; }
        public int TotalCount { get; set; }

    }
}
