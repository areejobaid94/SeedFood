using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class MediaIDModel2
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
