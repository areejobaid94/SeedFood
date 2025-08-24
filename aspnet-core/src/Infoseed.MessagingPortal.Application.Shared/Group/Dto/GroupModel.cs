using Infoseed.MessagingPortal.Group.Dto;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Groups.Dto
{
    public class GroupModel
    {
        public List<GroupDtoModel> groupDtoModel { get; set; }
        public long total { get; set; }
        public int state { get; set; }
        public string message { get; set; }
    }
}
