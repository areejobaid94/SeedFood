using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class ContactListToQueue
    {
        public int Id { get; set; }
        public int CustomerOPT { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        //public string userId { get; set; }
        //public string displayName { get; set; }
        public TemplateVariablless templateVariables { get; set; }
    }
    public class TemplateVariablless
    {
        public string VarOne { get; set; }
        public string VarTwo { get; set; }
        public string VarThree { get; set; }
        public string VarFour { get; set; }
        public string VarFive { get; set; }
    }
}
