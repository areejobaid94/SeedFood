using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace Infoseed.MessagingPortal.Sunshine.Models
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
        public string SubscriptionID { get;  } 


    }
}
