using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class AttachmentMessageModel
    {
        public bool IsHasAttachment { get; set; }
        public string ID { get; set; }
        public string URLID { get; set; }
        public string AttachmentId { get; set; }
        public string AttachmentMimeType { get; set; }
        public string FcToken { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }
        public string CustomerModel { get; set; }
    }
}
