using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{

    public class WhatsAppContactsDto
    {
        public int? Id { get; set; }

        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public long? Branch { get; set; }
        public string ContactName { get; set; }
        public string JoiningFrom { get; set; }
        public string JoiningTo { get; set; }
        public DateTime? OrderTimeFrom { get; set; }
        public DateTime? OrderTimeTo { get; set; }
        public int? TotalOrderMin { get; set; }
        public int? TotalOrderMax { get; set; }
        public int? TotalSessions { get; set; }

        public long? InterestedOfOne { get; set; }
        public long? InterestedOfTwo { get; set; }
        public long? InterestedOfThree { get; set; }
        public string IsOpt { get; set; }

        public TemplateVariables templateVariables { get; set; }

        public int? CustomerOPT { get; set; }


        public long? TemplateId { get; set; }
        public long? CampaignId { get; set; }
        public int? tenantId { get; set; }
        public int? pageNumber { get; set; } = 0;
        public int? pageSize { get; set; }

    }
    public class TemplateVariables
    {
        public string VarOne { get; set; }
        public string VarTwo { get; set; }
        public string VarThree { get; set; }
        public string VarFour { get; set; }
        public string VarFive { get; set; }
        public string VarSix { get; set; }
        public string VarSeven { get; set; }
        public string VarEight { get; set; }
        public string VarNine { get; set; }
        public string VarTen { get; set; }
        public string VarEleven { get; set; }
        public string VarTwelve { get; set; }
        public string VarThirteen { get; set; }
        public string VarFourteen { get; set; }
        public string VarFifteen { get; set; }
        public string VarSixteen { get; set; }

    }
    //public class HeaderVariablesTemplate
    //{
    //    public string VarOne { get; set; }
    //}
}
