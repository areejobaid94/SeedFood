using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset.Dto
{
    public class AssetLevelFourDto
    {
        public long Id { get; set; }
        public long LevelThreeId { get; set; }
        public string LevelFourNameAr { get; set; }
        public string LevelFourNameEn { get; set; }
        public int TenantId { get; set; }
    }
}
