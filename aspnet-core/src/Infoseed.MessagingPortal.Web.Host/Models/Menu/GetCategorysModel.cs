using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Menu
{
    public class GetCategorysModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public int Priority { get; set; }
        public long MenuId { get; set; }

        public string bgImag { get; set; }
        public string logoImag { get; set; }
      

        public List<GetSubCategorysModel> getSubCategorysModels { get; set; }
    }
}
