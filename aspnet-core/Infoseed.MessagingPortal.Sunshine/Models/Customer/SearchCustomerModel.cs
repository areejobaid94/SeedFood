using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Models
{
    public class SearchCustomerModel
    {
        public SearchCustomerModel()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public string SearchTerm { get; set; }
        //[DefaultValue(10)]
        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int PageSize { get; set; }
        //[DefaultValue(1)]
        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int PageNumber { get; set; }
    }
}
