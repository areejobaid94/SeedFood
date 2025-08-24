using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset.Dto
{
    public class AssetEntity
    {
        public List<AssetDto> lstAssetDto { get; set; }
        public int  TotalCount { get; set; }
    }
}
