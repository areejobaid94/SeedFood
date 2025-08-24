
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.MenuCategories.Dtos
{
    public class MenuCategoryDto : EntityDto<long>
    {

        public  string Name { get; set; }
        public  string NameEnglish { get; set; }


        public string logoImag { get; set; }
        public string bgImag { get; set; }

        public bool IsDeleted { get; set; }

        public int Priority { get; set; }

        public int MenuType { get; set; }
        public int LanguageBotId { get; set; }
        public long MenuId { get; set; }
    }
}