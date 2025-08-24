using Infoseed.MessagingPortal.Areas.Dtos;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.BotAPI.Models.FlwosRestorant
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
    }
    public class AreaResponseDto
    {
        public int Count { get; set; }
        public long Id { get; set; }
        public string AreaName { get; set; }
        public List<BranchDto> Areas { get; set; }
    }

}
