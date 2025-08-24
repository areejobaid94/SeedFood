using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewFunctionApp
{
    public class MediaIDModel
    {
      
            public Medium[] media { get; set; }
            public Meta meta { get; set; }
        

        public class Meta
        {
            public string api_status { get; set; }
            public string version { get; set; }
        }

        public class Medium
        {
            public string id { get; set; }
        }

    }
}
