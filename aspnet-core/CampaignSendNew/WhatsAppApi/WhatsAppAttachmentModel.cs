using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSendNew.WhatsAppApi
{
    public class WhatsAppAttachmentModel
    {

        public byte[] contentByte { get; set; }
        public string contentType { get; set; }


        public string messaging_product { get; set; }
        public string url { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string file_size { get; set; }
        public string id { get; set; }
    }



}
