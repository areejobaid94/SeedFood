using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.TargetReach.Dto
{
    public class TargetReachModel
    {
        public string SenderPhoneNumber { get; set; }
        public string ReciverPhoneNumber { get; set; }
        public string ReciverName { get; set; }
        public string MssageContent { get; set; }
        public string TemplateName { get; set; }
    }
}
