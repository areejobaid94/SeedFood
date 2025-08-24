using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Menu
{
    public class MenuSubCategoryDto
    {
        public int Id { get; set; }
        public int ItemCategoryId { get; set; }
        public int Priority { get; set; }
        public int LanguageBotId { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }


        
        public int TenantId { get; set; }
        public int MenuType { get; set; }

    }
}
