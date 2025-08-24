using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Menu
{
    public class GetMenuModel
    {
        public long Id { get; set; }
        public string MenuName { get; set; }

        public string MenuNameEnglish { get; set; }

        public int Priority { get; set; }
        public string ImageUri { get; set; }

        public List<GetCategorysModel> getCategorysModels { get; set; }
        public bool isInService { get; set; }
        public bool IsSelected { get; set; }
        

    }
}
