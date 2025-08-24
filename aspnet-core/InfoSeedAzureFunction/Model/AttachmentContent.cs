using InfoSeedAzureFunction;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Infoseed.InfoSeedAzureFunction
{
    public class AttachmentContent
    {
        public AttachmentContent()
        {
            this.SubscriptionID = Constant.SubscriptionID; //ConfigurationManager.AppSettings["BlobStorageSubscription"];
        }

     
        public Guid AttachmentID { get; set; }
        public byte[] Content { get; set; }
        public Stream StreamContent { get; set; }
        public string MimeType { get; set; }
        public string Extension { get; set; }
        public string SubscriptionID { get; }
        public string AttacmentName { get; set; }


    }
}
