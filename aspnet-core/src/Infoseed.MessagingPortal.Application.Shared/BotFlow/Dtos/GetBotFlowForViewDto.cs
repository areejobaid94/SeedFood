using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

namespace Infoseed.MessagingPortal.BotFlow.Dtos
{
    public class GetBotFlowForViewDto
    {

        public int? id { get; set; }
        public bool? isRoot { get; set; } = false;
        public bool? isNodeRoot { get; set; } = false;
        public string captionAr { get; set; } = "";
        public string captionEn { get; set; } = "";
        public string footerTextAr { get; set; } = "";
        public string footerTextEn { get; set; } = "";
        
        public string InputHint { get; set; } = "Please check";

        public int? counterNode { get; set; } = 0;

        public int? childIndex { get; set; }
        public int[] parentIndex { get; set; }
        public int? childId { get; set; }
        public int[] parentId { get; set; }
        public long? top { get; set; }
        public long? bottom { get; set; }
        public long? left { get; set; }
        public long? rigth { get; set; }
        public string type { get; set; }
         public string  parameter { get; set; }
        public string parameterType { get; set; }

        public ImageFlowModel urlImage { get; set; }
        public ImageFlowModel[] urlImageArray { get; set; }


        public int  dilationTime { get; set; }
        public string listOfUsers  { get; set; }
        public string listOfTeams { get; set; }
        public string actionBlock { get; set; }
        public string templateId { get; set; }

        public bool isOneTimeQuestion { get; set; }
         
        public bool isAdvance { get; set; }= false;
        public ContentInfo content { get; set; }



        public string title { get; set; }

        public int jumpId { get; set; }
        public RequestModel request { get; set; }
        public ConditionalModel conditional { get; set; }
        public List<ParameterSet> parameterList { get; set; }



        public ScheduleModel schedule { get; set; }
        public GoogleSheetIntegrationModel googleSheetIntegration { get; set; }
        public CatalogTemplateDto catalogTemplateDto { get; set; }

        public class CatalogTemplateDto
        {
            public TemplateHeader Header { get; set; }
            public TemplateBody Body { get; set; }
            public TemplateFooter Footer { get; set; }
            public Cacatalog Catalog { get; set; }
        }

        public class TemplateHeader
        {
            public string Type { get; set; }
            public string Text { get; set; }
        }

        public class TemplateBody
        {
            public string Text { get; set; }
        }

        public class TemplateFooter
        {
            public string Text { get; set; }
        }

        public class Cacatalog
        {
            public string CatalogId { get; set; }
            public string CatalogName { get; set; }
            public string SectionTitle { get; set; }
            public List<ProductDto> Products { get; set; }
        }

        public class ProductDto
        {
            public string ProductId { get; set; }
            public string Name { get; set; }
            public string retailer_Id { get; set; }
            public string Description { get; set; }
            public string availability { get; set; }
            public string Price { get; set; }
            public string Currency { get; set; }
            public string ImageUrl { get; set; }
        }
        //public Cacatalog cacatalog { get; set; }


        public class GoogleSheetIntegrationModel
        {
            public string IntegrationType { get; set; }
            public string GoogleSheetAction { get; set; }
            public string SpreadSheetId { get; set; }
            public string SpreadSheetName { get; set; }
            public string WorkSheet { get; set; }
            public string LookupColumn { get; set; }
            public string LookupValue { get; set; }
            public List<string> Parameters { get; set; }
            public List<string> WorksheetColumns { get; set; }
            public int TenantId { get; set; }


        }
        public class ScheduleModel
        {
            public bool isData { get; set; }

            public bool isNow{ get; set; }
            public int numberButton { get; set; }
            public string unavailableDate { get; set; }

            public DateTime? startDate { get; set; }
            public DateTime? endDate { get; set; }


        }

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

        public class ParameterSet
        {

            public string par { get; set; }

            public string val { get; set; }

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

    }
}
