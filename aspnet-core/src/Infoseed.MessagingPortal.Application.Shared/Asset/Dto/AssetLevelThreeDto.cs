using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset.Dto
{
    public class AssetLevelThreeDto
    {
        public long Id { get; set; }
        public long LevelTwoId { get; set; }
        public string LevelThreeNameAr { get; set; }
        public string LevelThreeNameEn { get; set; }
        public int TenantId { get; set; }
    }
}
