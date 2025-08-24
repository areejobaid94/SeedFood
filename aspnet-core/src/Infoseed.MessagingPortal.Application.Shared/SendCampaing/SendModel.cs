using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infoseed.MessagingPortal.SendCampaing
{
    public class SendModel
    {
        public string ReciverPhoneNumber { get; set; }
        public string ReciverName { get; set; }

        // [MaxLength(1024)]
        public string MssageContent { get; set; }
        public string MssageContent2 { get; set; }
        public string MssageContent3 { get; set; }
        public string MssageContent4 { get; set; }
        public string MssageContent5 { get; set; }
        public string MssageContent16 { get; set; }


        public bool IsPDF { get; set; } = false;
        public string LinkPDF { get; set; } = "";

        public string FileNamePDF { get; set; } = "";

        public bool IsImage { get; set; } = false;
        public string LinkImage { get; set; } = "";

        public bool IsVideo { get; set; } = false;
        public string LinkVideo { get; set; } = "";



    }
}
