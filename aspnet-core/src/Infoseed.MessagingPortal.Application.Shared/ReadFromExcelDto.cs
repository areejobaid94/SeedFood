using Infoseed.MessagingPortal.InfoSeedParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class ReadFromExcelDto
    {
        public List<MembersList> List { get; set; }
        public long DuplicateCount { get; set; }
    }
}
