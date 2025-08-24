using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SalesUserCreate.Dtos
{
    public class SalesUserCreateDto
    {
        public int? TenantId { get; set; }
        public  int UserId { get; set; }
        public  string UserName { get; set; }

        public  int TotalCreate { get; set; }

        public  bool IsActiveButton { get; set; }
    }
}
