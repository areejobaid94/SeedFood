using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset.Dto
{
    public class LevelsEntity
    {
        public List<AssetLevelOneDto> lstAssetLevelOneDto { get; set; }
        public List<AssetLevelTwoDto> lstAssetLevelTwoDto { get; set; }
        public List<AssetLevelThreeDto> lstAssetLevelThreeDto { get; set; }
        public List<AssetLevelFourDto> lstAssetLevelFourDto { get; set; }

    }
}
