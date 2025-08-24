using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenantSyncJob
{
    public class TenantModel
    {
        public int? TenantId { get; set; }
        public ContainerItemTypes ItemType { get; set; }
        public string SmoochAppID { get; set; }
        public string SmoochSecretKey { get; set; }
        public string SmoochAPIKeyID { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }


        public bool IsCancelOrder { get; set; }
        public int CancelTime { get; set; }



        public bool IsWorkActive { get; set; }

        public WorkModel workModel { get; set; }


        public string D360Key { get; set; }

        public bool IsEvaluation { get; set; }

        public string EvaluationText { get; set; }
        public int EvaluationTime { get; set; }


        public bool isOrderOffer { get; set; }

        public bool IsLoyalityPoint { get; set; }
        public int Points { get; set; }

        public string PhoneNumber { get; set; }

        public string Image { get; set; }

        public string ImageBg { get; set; }
        public bool IsBundleActive { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }
    }
}
