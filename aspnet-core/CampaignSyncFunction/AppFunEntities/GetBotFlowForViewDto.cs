using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSyncFunction.AppFunEntities
{
    public class GetBotFlowForViewDto
    {

        public int? id { get; set; }
        public bool? isRoot { get; set; }
        public string captionAr { get; set; } = "";
        public string captionEn { get; set; } = "";
        public string footerTextAr { get; set; } = "";
        public string footerTextEn { get; set; } = "";
        public int? childIndex { get; set; }
        public int[] parentIndex { get; set; }
        public int? childId { get; set; }
        public int[] parentId { get; set; }
        public long? top { get; set; }
        public long? bottom { get; set; }
        public long? left { get; set; }
        public long? rigth { get; set; }
        public string type { get; set; }
        public string parameter { get; set; }
        public string parameterType { get; set; }

        public ImageFlowModel urlImage { get; set; }
        public ImageFlowModel[] urlImageArray { get; set; }


        public int dilationTime { get; set; }
        public string listOfUsers { get; set; }
        public string actionBlock { get; set; }
        public string templateId { get; set; }


        public ContentInfo content { get; set; }

        public string title { get; set; }

        public int jumpId { get; set; }
        public RequestModel request { get; set; }
        public ConditionalModel conditional { get; set; }

        public class ConditionalModel
        {
            public List<ConditionList> conditionList { get; set; }
            public string orAnd { get; set; }
        }
        public class ConditionList
        {

            public string op1 { get; set; }

            public string operation { get; set; }

            public string op2 { get; set; }



        }
        public class RequestModel
        {
            public string httpMethod { get; set; }
            public string url { get; set; }
            public string body { get; set; }
            public string token { get; set; }
            public string contentType { get; set; }
            public string resposeType { get; set; }


        }
        public class ContentInfo
        {
            public string txt { get; set; }
            public Dtocontent[] dtoContent { get; set; }
        }

        public class Dtocontent
        {
            public int? childIndex { get; set; }
            public int[] parentIndex { get; set; }
            public int? childId { get; set; }
            public int[] parentId { get; set; }
            public string valueAr { get; set; } = "";
            public string valueEn { get; set; } = "";
            public int? key { get; set; }
            public string branchID { get; set; } = "";
        }

        public class ParameterValue
        {
            public string key { get; set; } = "";
            public string value { get; set; } = "";
        }
        public class ImageFlowModel
        {
            public string url { get; set; }
            public string name { get; set; }
        }


    }

}
