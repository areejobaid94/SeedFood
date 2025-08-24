using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Items.Dtos
{
    public class ItemImagesModel
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public long ItemId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMainImage { get; set; }
    }
}
