using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class ListContactToCampin
    {
        public int Id { get; set; }
        public int CustomerOPT { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public TemplateVariablles templateVariables { get; set; }

    }
    public class TemplateVariablles
    {
        public string VarOne { get; set; }
        public string VarTwo { get; set; }
        public string VarThree { get; set; }
        public string VarFour { get; set; }
        public string VarFive { get; set; }
    }
}
