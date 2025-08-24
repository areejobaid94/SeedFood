using Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.WhatsAppDialog
{
    public class TenantD360Model
    {
        public int? TenantId { get; set; }
        public InfoSeedContainerItemTypes ItemType { get; set; }
        public string SmoochAppID { get; set; }

        public string SmoochSecretKey { get; set; }
        public string SmoochAPIKeyID { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }

        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public bool IsWorkActive { get; set; }
        //public string WorkText { get; set; }
        public WorkModel workModel { get; set; }

        public bool IsCancelOrder { get; set; }
        public int CancelTime { get; set; }


        public bool IsEvaluation { get; set; }

        public string EvaluationText { get; set; }

        public string PhoneNumber { get; set; }
        public int EvaluationTime { get; set; }


        public string D360Key { get; set; }

        public bool isOrderOffer { get; set; }
        

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }
    }

    public class WorkModel
    {

        public bool IsWorkActiveSun { get; set; }
        public string WorkTextSun { get; set; }
        public DateTime StartDateSun { get; set; }
        public DateTime EndDateSun { get; set; }



        public bool IsWorkActiveMon { get; set; }
        public string WorkTextMon { get; set; }
        public DateTime StartDateMon { get; set; }
        public DateTime EndDateMon { get; set; }


        public bool IsWorkActiveTues { get; set; }
        public string WorkTextTues { get; set; }
        public DateTime StartDateTues { get; set; }
        public DateTime EndDateTues { get; set; }



        public bool IsWorkActiveWed { get; set; }
        public string WorkTextWed { get; set; }
        public DateTime StartDateWed { get; set; }
        public DateTime EndDateWed { get; set; }


        public bool IsWorkActiveThurs { get; set; }
        public string WorkTextThurs { get; set; }
        public DateTime StartDateThurs { get; set; }
        public DateTime EndDateThurs { get; set; }


        public bool IsWorkActiveFri { get; set; }
        public string WorkTextFri { get; set; }
        public DateTime StartDateFri { get; set; }
        public DateTime EndDateFri { get; set; }



        public bool IsWorkActiveSat { get; set; }
        public string WorkTextSat { get; set; }
        public DateTime StartDateSat { get; set; }
        public DateTime EndDateSat { get; set; }


    }
}
