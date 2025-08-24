using System;

namespace Infoseed.MessagingPortal.Teams.Dto
{
    public class TeamsFilter
    {
        public int pageNumber { get; set; } = 0;
        public int pageSize { get; set; } = 10;
        public string contactName { get; set; } = "";
        public string countryCode { get; set; } = "";
        public DateTime? joiningFrom { get; set; } = null;
        public DateTime? joiningTo { get; set; } = null;
        public string isOpt { get; set; } = "";
    }
}
