using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus
{
    public class ListMenu
    {
        public List<ListItem> listItems  { get; set; }
        public long Id { get; set; }
        public string ImageUri { get; set; }
        public  string ImageBgUri { get; set; }
        public  string MenuName { get; set; }

        public  string MenuDescription { get; set; }

        public  string MenuNameEnglish { get; set; }

        public  string MenuDescriptionEnglish { get; set; }
        public int Priority { get; set; }

        public RestaurantsTypeEunm RestaurantsType { get; set; }

    }
}
