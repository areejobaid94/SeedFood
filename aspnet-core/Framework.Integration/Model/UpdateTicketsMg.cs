using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Integration.Model
{
    public class UpdateTicketsMg
    {


     
        public Input[] inputs { get; set; }
      

        public class Input
        {
            public From from { get; set; }
            public To to { get; set; }
            public string type { get; set; }
        }

        public class From
        {
            public string id { get; set; }
        }

        public class To
        {
            public string id { get; set; }
        }

    }
}
