using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Menu
{
    public class MenuCategoryWithSubDto
    {
        
              public int Id { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }


        public string logoImag { get; set; }
        public string bgImag { get; set; }

        public bool IsDeleted { get; set; }

        public int Priority { get; set; }

        public int MenuType { get; set; }
        public int LanguageBotId { get; set; }
        public bool isSubCategory { get; set; }

        public List<MenuSubCategoryDto> menuSubCategoryDtos { get; set; }
    }
}
