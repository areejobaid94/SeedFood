using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class MenusEntity
    {
        public List<MenuDto> lstMenu { get; set; }
        public int TotalCount { get; set; }
    }
}
