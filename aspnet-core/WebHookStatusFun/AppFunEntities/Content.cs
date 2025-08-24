using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class Content
    {
        //types are: text-image-file-carousel-location , go to the doc for other types.
        //https://docs.smooch.io/rest/#operation/postMessage
        public string type { get; set; }
        public string text { get; set; }
        public string mediaUrl { get; set; }
        public string altText { get; set; }
        public string agentName { get; set; }
        public string agentId { get; set; }
        public string fileName { get; set; }
    }
}
