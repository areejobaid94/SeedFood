using DocumentFormat.OpenXml.ExtendedProperties;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.BotAPI.Models.AlSarhTravel
{
    public class AirportResponse
    {
        public List<AirportHit> Hits { get; set; }
        public string Query { get; set; }
        public int ProcessingTimeMs { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int EstimatedTotalHits { get; set; }
    }

    public class AirportHit
    {
        public string Code { get; set; }
        public string IATA_CODE { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public Country1 Country { get; set; }
        public City1 City { get; set; }
        public string Permalink { get; set; } 
        public State State { get; set; }       
        public Hotels Hotels { get; set; }     
    }
    public class State
    {
        public string Code { get; set; }
    }

    public class Hotels
    {
        public int TotalCount { get; set; }
    }
    public class MultilingualText
    {
        public string Ar { get; set; }
        public string En { get; set; }
        public string Es { get; set; }
    }

    public class Country1
    {
        public string Code { get; set; }
        public Dictionary<string, string> Name { get; set; }
    }

    public class City1
    {
        public string Code { get; set; }
        public Dictionary<string, string> Name { get; set; } 
    }

}
