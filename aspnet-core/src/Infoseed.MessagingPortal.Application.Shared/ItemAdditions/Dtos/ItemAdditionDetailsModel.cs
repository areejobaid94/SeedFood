using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditions.Dtos
{
    public class ItemAdditionDetailsModel
    {

        public int Id { get; set; }
        public int CopiedFromId { get; set; }
        public int TenantId { get; set; }
        public int ItemId { get; set; }
        public int MenuType { get; set; }
        public int ItemAdditionId { get; set; }
        public bool IsInService { get; set; }


    }
}
