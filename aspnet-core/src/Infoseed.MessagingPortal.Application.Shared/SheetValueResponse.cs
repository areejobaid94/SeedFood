using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class SheetValueResponse
    {
        [JsonProperty("range")]
        public string Range { get; set; }

        [JsonProperty("majorDimension")]
        public string MajorDimension { get; set; }

        [JsonProperty("values")]
        public List<List<object>> Values { get; set; }
    }
}
