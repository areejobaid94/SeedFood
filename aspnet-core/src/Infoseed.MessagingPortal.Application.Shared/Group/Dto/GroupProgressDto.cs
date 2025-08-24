using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Group.Dto
{
    public class GroupProgressDto
    {
        public int Total { get; set; }
        public int Inserted { get; set; }
        public int Remaining { get; set; }
        public double ProgressPercent { get; set; }
    }
}
