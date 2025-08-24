using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class AttachmentContent
    {
        public AttachmentContent()
        {
            this.SubscriptionID = ConfigurationManager.AppSettings["BlobStorageSubscription"];
        }

     
        public Guid AttachmentID { get; set; }
        public byte[] Content { get; set; }
        public Stream StreamContent { get; set; }
        public string MimeType { get; set; }
        public string Extension { get; set; }
        public string SubscriptionID { get; }
        public string fileName { get; set; }
        public string AttacmentName { get; set; }
    }
}
