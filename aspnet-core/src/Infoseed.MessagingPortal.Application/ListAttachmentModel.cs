using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class ListAttachmentModel
    {
        public string FilePath { get; set; }
        public byte[] content { get; set; }
        public string sh256 { get; set; }
        public string type { get; set; }
        public string mediaUrl { get; set; }
        public string fileName { get; set; }
        public string typeContent { get; set; }
        
    }
}
