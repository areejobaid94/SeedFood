using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.BotModel
{
    public class GetListPDFModel
    {
        public  int TenantID { get; set; }
        public string phoneNumber { get; set; }
        public string AttachmentUrl { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentType { get; set; }
         public string AssetDescriptionAr { get; set; }
        public string AssetDescriptionEn { get; set; }

        public string AssetNameAr { get; set; }
        public string AssetNameEn { get; set; }
    }
}
