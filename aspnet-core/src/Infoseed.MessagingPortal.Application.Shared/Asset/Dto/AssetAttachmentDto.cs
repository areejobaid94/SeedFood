using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset.Dto
{
    public class AssetAttachmentDto
    {
        public long Id { get; set; }
        public string AttachmentType { get; set; }
        public string AttachmentName { get; set; }
        public bool InService { get; set; }
        public string AttachmentUrl { get; set; }
        public long AssetId { get; set; }
    }
}
