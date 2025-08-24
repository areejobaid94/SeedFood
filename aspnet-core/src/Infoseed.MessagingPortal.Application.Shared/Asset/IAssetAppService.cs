using Abp.Application.Services;
using Infoseed.MessagingPortal.Asset.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset
{
    public interface IAssetAppService :IApplicationService
    {
        AssetEntity GetAsset(int pageNumber = 0, int pageSize = 50, int? tenantId = null);
        long AddAsset(AssetDto assetDto);
        void UpdateAsset(AssetDto assetDto);
        bool DeleteAsset(long assetId);
        AssetDto GetAssetById(long assetId, int? tenantId);
        LevelsEntity LoadLevels(int? tenantId);
        List<AssetLevelTwoDto> MgMotorsGetOfers(int? tenantId);
        LevelsEntity LoadDistinctLevels(int? tenantId);

        List<AssetDto> GetListOfAsset(int tenantId, long? levleOneId = null, long? levelTwoId = null, int? typeId = null, long? levelThreeId = null, long? levelFourId = null, bool isOffer = false);
        AssetEntity GetOfferAsset(int pageNumber = 0, int pageSize = 50, int? tenantId = null);
    }
}
