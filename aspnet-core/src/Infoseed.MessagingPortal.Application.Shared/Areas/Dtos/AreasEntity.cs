using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Areas.Dtos
{
    public class AreasEntity
    {
        public List<AreaDto> lstAreas { get; set; }
        public int TotalCount { get; set; }
    }
}
