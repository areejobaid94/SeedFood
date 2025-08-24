using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MgSystem
{
    public class CreateContactMg
    {

       
        public Property1[] properties { get; set; }
        

        public class Property1
        {
            public string property { get; set; }
            public string value { get; set; }
        }

    }
}
