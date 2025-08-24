using Abp.Authorization;
using Framework.Data;
using Infoseed.MessagingPortal.Asset.Dto;
using Infoseed.MessagingPortal.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Infoseed.MessagingPortal.Asset
{

    //[AbpAuthorize(AppPermissions.Pages_Assets)]
    public class AssetAppService : MessagingPortalAppServiceBase, IAssetAppService
    {

        public AssetAppService(){
        }

        public long AddAsset(AssetDto assetDto)
        {
            return addAsset(assetDto);
        }

        public bool DeleteAsset(long assetId)
        {
           return deleteAsset(assetId);
        }

        public AssetEntity GetAsset(int pageNumber = 0, int pageSize = 50, int? tenantId= null)
        {

            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;


            return getAsset(tenantID.Value, pageNumber, pageSize);
        }

        public AssetEntity GetOfferAsset(int pageNumber = 0, int pageSize = 50, int? tenantId = null)
        {

            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;


            return getOfferAsset(tenantID.Value, pageNumber, pageSize);
        }
        public AssetDto GetAssetById(long assetId, int? tenantId)
        {
            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;

            return getAssetById(assetId, tenantID.Value);
        }

        public void UpdateAsset(AssetDto assetDto)
        {
             updateAsset(assetDto);
        }
        public LevelsEntity LoadLevels(int? tenantId)
        {
            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;

            return loadLevels(tenantID.Value);
        }
        public List<AssetLevelTwoDto> MgMotorsGetOfers(int? tenantId)
        {
            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;

            return mgMotorsGetOfers(tenantID.Value);
        }
        public LevelsEntity LoadDistinctLevels(int? tenantId)
        {
            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;

            return loadDistinctLevels(tenantID.Value);
        }
        public List<AssetDto> GetListOfAsset(int tenantId, long? levleOneId = null, long? levelTwoId = null, int? typeId = null, long? levelThreeId = null, long? levelFourId = null, bool isOffer = false)

        {
            return getListOfAsset(tenantId,levleOneId,levelTwoId,typeId,levelThreeId,levelFourId,isOffer);
        }




        #region PrivetMethod

        private long addAsset(AssetDto assetDto)
        {
            try
            {
                assetDto.CreatedOn = DateTime.Now;
               
                var SP_Name = Constants.Asset.SP_AssetAdd;

                var sqlParameters = new List<SqlParameter> 
                {
                new SqlParameter("@AssetNameAr",assetDto.AssetNameAr)
               ,new SqlParameter("@AssetNameEn",assetDto.AssetNameEn)
               ,new SqlParameter("@AssetDescriptionAr",assetDto.AssetDescriptionAr)
               ,new SqlParameter("@AssetDescriptionEn",assetDto.AssetDescriptionEn)
               ,new SqlParameter("@TenantId",(int?)AbpSession.TenantId.Value)
               ,new SqlParameter("@AssetTypeId",assetDto.AssetTypeId)
               ,new SqlParameter("@AssetLevelOneId",assetDto.AssetLevelOneId)
               ,new SqlParameter("@AssetLevelTwoId",assetDto.AssetLevelTwoId)
               ,new SqlParameter("@AssetLevelThreeId",assetDto.AssetLevelThreeId)
               ,new SqlParameter("@AssetLevelFourId",assetDto.AssetLevelFourId)
               ,new SqlParameter("@AssetStatusId",assetDto.AssetStatusId)
               ,new SqlParameter("@CreatedOn",assetDto.CreatedOn)
               ,new SqlParameter("@CreatedBy",assetDto.CreatedBy)
               ,new SqlParameter("@IsOffer",assetDto.IsOffer)
               ,new SqlParameter("@AssetAttachmentJson",JsonConvert.SerializeObject( assetDto.lstAssetAttachmentDto))
               
            };


                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@AssetId";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool deleteAsset(long assetId)
        {
            try
            {

                var SP_Name = Constants.Asset.SP_AssetDelete;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@AssetId",assetId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private AssetEntity getAsset(int tenantId, int pageNumber = 0, int pageSize = 50)
        {
            try
            {
                AssetEntity assetEntity= new AssetEntity();
                List<AssetDto> lstAssetDto = new List<AssetDto>();
                var SP_Name = Constants.Asset.SP_AssetGet;

                var sqlParameters = new List<SqlParameter>
                {
                new SqlParameter("@PageSize",pageSize)
               ,new SqlParameter("@PageNumber",pageNumber)
               ,new SqlParameter("@TenantId",tenantId)
              
               
            };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                lstAssetDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToAssetDto, AppSettingsModel.ConnectionStrings).ToList();


                assetEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                assetEntity.lstAssetDto = lstAssetDto;
                return assetEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private AssetEntity getOfferAsset(int tenantId, int pageNumber = 0, int pageSize = 50)
        {
            try
            {
                AssetEntity assetEntity = new AssetEntity();
                List<AssetDto> lstAssetDto = new List<AssetDto>();
                var SP_Name = Constants.Asset.SP_AssetOfferGet;

                var sqlParameters = new List<SqlParameter>
                {
                new SqlParameter("@PageSize",pageSize)
               ,new SqlParameter("@PageNumber",pageNumber)
               ,new SqlParameter("@TenantId",tenantId)


            };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                lstAssetDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToAssetDto, AppSettingsModel.ConnectionStrings).ToList();


                assetEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                assetEntity.lstAssetDto = lstAssetDto;
                return assetEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private AssetDto getAssetById(long assetId, int? tenantId)
        {
            try
            {
              
                AssetDto objAssetDto = new AssetDto();
                var SP_Name = Constants.Asset.SP_AssetByIDGet;

                var sqlParameters = new List<SqlParameter>
                {
             
                new SqlParameter("@AssetId",assetId)
               ,new SqlParameter("@TenantId",tenantId)


            };

                objAssetDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToAssetDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                return objAssetDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void updateAsset(AssetDto assetDto)
        {
            try
            {
                assetDto.ModifiedOn = DateTime.Now;
              //  assetDto.ModifiedBy = assetDto.ModifiedBy;
                var SP_Name = Constants.Asset.SP_AssetUpdate;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@AssetId",assetDto.Id)
                ,new SqlParameter("@AssetNameAr",assetDto.AssetNameAr)
               ,new SqlParameter("@AssetNameEn",assetDto.AssetNameEn)
               ,new SqlParameter("@AssetDescriptionAr",assetDto.AssetDescriptionAr)
               ,new SqlParameter("@AssetDescriptionEn",assetDto.AssetDescriptionEn)
               ,new SqlParameter("@TenantId",assetDto.TenantId)
               ,new SqlParameter("@AssetTypeId",assetDto.AssetTypeId)
               ,new SqlParameter("@AssetLevelOneId",assetDto.AssetLevelOneId)
               ,new SqlParameter("@AssetLevelTwoId",assetDto.AssetLevelTwoId)               
               ,new SqlParameter("@AssetLevelThreeId",assetDto.AssetLevelThreeId)
               ,new SqlParameter("@AssetLevelFourId",assetDto.AssetLevelFourId)
               ,new SqlParameter("@AssetStatusId",assetDto.AssetStatusId)
               ,new SqlParameter("@ModifiedBy",assetDto.ModifiedBy)
               ,new SqlParameter("@ModifiedOn",assetDto.ModifiedOn)
               ,new SqlParameter("@IsOffer",assetDto.IsOffer)
               ,new SqlParameter("@AssetAttachmentJson",JsonConvert.SerializeObject( assetDto.lstAssetAttachmentDto))


            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private LevelsEntity loadLevels(int tenantId)
        {
            try
            {
                LevelsEntity levelsEntity = new LevelsEntity();
                var SP_Name = Constants.Asset.SP_LevelAssetGet;

                var sqlParameters = new List<SqlParameter>
                {
             
               new SqlParameter("@TenantId",tenantId)


            };

                levelsEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToLevelsEntity, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return levelsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<AssetLevelTwoDto> mgMotorsGetOfers(int tenantId)
        {
            try
            {
                List<AssetLevelTwoDto> AssetLevelTwoDto = new List<AssetLevelTwoDto>();
                var SP_Name = Constants.Asset.SP_MgMotorsGetAsset;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@TenantId",tenantId)
                };

                AssetLevelTwoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.ConvertMgMotorsGetAsset, AppSettingsModel.ConnectionStrings).ToList();

                return AssetLevelTwoDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private LevelsEntity loadDistinctLevels(int tenantId)
        {
            try
            {
                LevelsEntity levelsEntity = new LevelsEntity();
                var SP_Name = Constants.Asset.SP_LevelAssetDistinctGet;

                var sqlParameters = new List<SqlParameter>
                {

               new SqlParameter("@TenantId",tenantId)


            };

                levelsEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToLevelsEntity, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return levelsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<AssetDto> getListOfAsset(int tenantId, long? levleOneId, long? levelTwoId, int? typeId,long? levelThreeId = null, long? levelFourId = null,bool isOffer=false)
        {
            try
            {

                if (!isOffer)
                {
                    List<AssetDto> lstObjAssetDto = new List<AssetDto>();
                    var SP_Name = Constants.Asset.SP_AssetByLevelsAndTypeGet;

                    var sqlParameters = new List<SqlParameter>
                    {

                         new SqlParameter("@LevleOneId",levleOneId)
                        ,new SqlParameter("@LevelTwoId",levelTwoId)
                        ,new SqlParameter("@LevelThreeId",levelThreeId)
                        ,new SqlParameter("@LevelFourId",levelFourId)
                        ,new SqlParameter("@TenantId",tenantId)
                        ,new SqlParameter("@TypeId",typeId)

                   };

                    lstObjAssetDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                     DataReaderMapper.ConvertToAssetDto, AppSettingsModel.ConnectionStrings).ToList();


                    return lstObjAssetDto;

                }
                else
                {

                    List<AssetDto> lstObjAssetDto = new List<AssetDto>();
                    var SP_Name = Constants.Asset.SP_AssetGetOffer;

                    var sqlParameters = new List<SqlParameter>
                    {
                           new SqlParameter("@TenantId",tenantId)
                           ,new SqlParameter("@IsOffer",isOffer)
                       
                    };

                    lstObjAssetDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                     DataReaderMapper.ConvertToAssetDto, AppSettingsModel.ConnectionStrings).ToList();


                    return lstObjAssetDto;
                }


            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
