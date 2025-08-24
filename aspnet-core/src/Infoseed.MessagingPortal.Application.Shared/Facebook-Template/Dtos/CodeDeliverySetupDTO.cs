using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class CodeDeliverySetupDTO
    {
        public CodeDeliverySetup CodeDeliverySetupOption { get; set; } // Default selection
        public bool ZeroTapAgreement { get; set; }
    }

   public enum CodeDeliverySetup
    {
        ZeroTap,
        OneTap,
        CopyCode
    }
}
