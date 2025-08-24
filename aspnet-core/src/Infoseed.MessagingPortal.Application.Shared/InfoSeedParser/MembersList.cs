using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.InfoSeedParser
{
    public class MembersList
    {
        public long Id { get; set; }
        public string phoneNumber { get; set; }
        public string displayName { get; set; }
        public bool isFailed { get; set; }
        public Dictionary<string, string> variables { get; set; }
        public int customerOPT {get; set;}
    
    }
}
