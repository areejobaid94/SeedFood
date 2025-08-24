using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset.Dto
{
    public class AssetLevelTwoDto
    {
        public long Id { get; set; }
        public long LevelOneId { get; set; }
        public string LevelTwoNameAr { get; set; }
        public string LevelTwoNameEn { get; set; }
        public int TenantId { get; set; }
    }
}
