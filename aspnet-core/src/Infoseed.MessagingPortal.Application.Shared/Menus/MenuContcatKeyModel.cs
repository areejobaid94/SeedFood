using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus
{
    public class MenuContcatKeyModel
    {

        public long Id { get; set; }
        public string KeyMenu { get; set; }
        public string Value { get; set; }

        //public DateTime CreationTime { get; set; }
        public int ContactID { get; set; }
    }
}
