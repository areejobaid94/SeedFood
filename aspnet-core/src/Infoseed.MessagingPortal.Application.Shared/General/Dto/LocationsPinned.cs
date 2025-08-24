using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.General.Dto
{
    public class LocationsPinned
    {
        public long Id { get; set; }
        public double  Latitude { get; set; }
        public double  Longitude  { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public int TenantId { get; set; }
        public string URL { get; set; }
    }
}
