using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class MenuEntity 
    {
        public long Id { get; set; }
        public string MenuName { get; set; }

        public string MenuNameEnglish { get; set; }

        public int Priority { get; set; }
        public string ImageUri { get; set; }
        public string ItemCategories { get; set; }
        public List<CategoryEntity> CategoryEntity { get; set; }
        public bool isInService { get; set; }
        public string SettingJson { get; set; }
        public int MenuTypeId { get; set; }

    }

    public class CategoryEntity
    {
        public long Id { get; set; }
        public int? TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public int MenuType { get; set; }
        public int LanguageBotId { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public int Priority { get; set; }
        public long MenuId { get; set; }
        public string bgImag { get; set; }
        public string logoImag { get; set; }
        public long? CopiedFromId { get; set; }
       // public string ItemSubCategories { get; set; }
        public List<SubCategoryEntity> ItemSubCategories { get; set; }


    
    } 
    
    public class SubCategoryEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public int Priority { get; set; }
        public int ItemCategoryId { get; set; }
        public int LanguageBotId { get; set; }

        public string bgImag { get; set; }
        public string logoImag { get; set; }
        public bool IsDeleted { get; set; }

        public int MenuType { get; set; }
        public int? TenantId { get; set; }
        public long? CopiedFromId { get; set; }
        public long? MenuId { get; set; }
        public decimal Price { get; set; }
        public bool IsNew { get; set; }



    }

    
}
