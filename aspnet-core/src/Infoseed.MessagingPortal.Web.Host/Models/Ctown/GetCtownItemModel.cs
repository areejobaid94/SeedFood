using Infoseed.MessagingPortal.Items.Dtos;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Web.Models.Ctown
{
    public class GetCtownItemModel
    {

        public List<ItemDto> ItemDtos { get; set; }
        public int Total { get; set; }
    }
}
