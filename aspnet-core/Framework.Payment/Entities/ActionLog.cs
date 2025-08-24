using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Payment
{
    public class ActionLog
    {
        public long ID { get; set; }

        public string RequestorIPAddress { get; set; }

        public string RequestUri { get; set; }

        public string RequestMethod { get; set; }

        public DateTime? RequestTimestamp { get; set; }

        public string RequestContentType { get; set; }

        public string RequestHeaders { get; set; }

        public string RequestContent { get; set; }

        public string RequestRawData { get; set; }

        public int? ResponseStatusCode { get; set; }

        public DateTime? ResponseTimestamp { get; set; }

        public string ResponseContentType { get; set; }

        public string ResponseHeaders { get; set; }

        public string ResponseContent { get; set; }

        public string ResponseRawData { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
    }
}
