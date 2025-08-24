using System;

namespace NewFunctionApp
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
    
}
