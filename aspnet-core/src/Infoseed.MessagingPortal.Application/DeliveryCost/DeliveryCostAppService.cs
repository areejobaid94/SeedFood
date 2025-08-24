using Framework.Data;
using Infoseed.MessagingPortal.DeliveryCost.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryCost
{
    public class DeliveryCostAppService : MessagingPortalAppServiceBase, IDeliveryCostAppService
    {

        public  DeliveryCostAppService(){

            }
        public long AddDeliveryCost(DeliveryCostDto deliveryCostDto)
        {
            return addDeliveryCost(deliveryCostDto);
        }

        public bool DeleteDeliveryCost(long deliveryCostId)
        {
           return deleteDeliveryCost(deliveryCostId);
        }

        public DeliveryCostEntity GetDeliveryCost(int? tenantId = null, int pageNumber = 0, int pageSize = 50)
        {

            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;

            return getDeliveryCost(tenantID.Value,pageNumber,pageSize);
        }

        public DeliveryCostDto GetDeliveryCostById(long deliveryCostId, int? tenantId)
        {
            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;


            return getDeliveryCostById(deliveryCostId, tenantID.Value);
        }
          
        public DeliveryCostDto GetDeliveryCostByAreaId(int tenantId, long areaId)
        {
            return getDeliveryCostByAreaId( tenantId, areaId);
        }

        public void UpdateDeliveryCost(DeliveryCostDto deliveryCostDto)
        {
            updateDeliveryCost(deliveryCostDto);
        }

       public LocationAddressDto GetDeliveryCostPerArea(int tenantId, float latitude, float longitude, string city, string area, string distric)
        {
          return  getDeliveryCostPerArea(tenantId, longitude, latitude, city, area, distric);
        }






        #region private Method
        private long  addDeliveryCost(DeliveryCostDto deliveryCostDto)
        {

            try
            {

             var SP_Name = Constants.DeliveryCost.SP_DeliveryCostAdd;

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@TenantId",(int?)AbpSession.TenantId.Value)
                ,new SqlParameter("@AreaIds",deliveryCostDto.AreaIds)
                ,new SqlParameter("@AboveValue",deliveryCostDto.AboveValue)       
                ,new SqlParameter("@CreatedBy",deliveryCostDto.CreatedBy)
                ,new SqlParameter("@CreatedOn",DateTime.UtcNow)
                ,new SqlParameter("@DeliveryCostJson",JsonConvert.SerializeObject(deliveryCostDto.lstDeliveryCostDetailsDto))
            };


                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@DeliveryCostId";
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
        private bool deleteDeliveryCost(long deliveryCostId)
        {
            try
            {

                var SP_Name = Constants.DeliveryCost.SP_DeliveryCostDelete;

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@Id",deliveryCostId) 
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        private DeliveryCostEntity getDeliveryCost(int tenantId, int pageNumber, int pageSize)
        {
            try
            {
                List<DeliveryCostDto> lstDeliveryCostDto = new List<DeliveryCostDto>();
                var SP_Name = Constants.DeliveryCost.SP_DeliveryCostGet;

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@TenantId",tenantId)
                ,new SqlParameter("@PageNumber",pageNumber)
                ,new SqlParameter("@PageSize",pageSize)




            };


                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;


                sqlParameters.Add(OutputParameter);


                lstDeliveryCostDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToDeliveryCostDto, AppSettingsModel.ConnectionStrings).ToList();

                DeliveryCostEntity deliveryCostEntity = new DeliveryCostEntity();
                deliveryCostEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                deliveryCostEntity.lstDeliveryCostDto = lstDeliveryCostDto;
                return deliveryCostEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DeliveryCostDto getDeliveryCostById(long id, int tenantId)
        {
            try
            {
                DeliveryCostDto deliveryCostDto = new DeliveryCostDto();
                var SP_Name = Constants.DeliveryCost.SP_DeliveryCostByIDGet;

                var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@TenantId",tenantId)
               , new SqlParameter("@Id",id)
            };

                deliveryCostDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToDeliveryCostDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return deliveryCostDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        private DeliveryCostDto getDeliveryCostByAreaId(int tenantId,long areaId)
        {
            try
            {
                DeliveryCostDto deliveryCostDto = new DeliveryCostDto();
                var SP_Name = Constants.DeliveryCost.SP_DeliveryCostByAreaIDGet;

                var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@TenantId",tenantId)
               , new SqlParameter("@AreaId",areaId)
            };

                deliveryCostDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToDeliveryCostDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return deliveryCostDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void updateDeliveryCost(DeliveryCostDto deliveryCostDto)
        {
            try
            {


                var SP_Name = Constants.DeliveryCost.SP_DeliveryCostUpdate;

              

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@Id",deliveryCostDto.Id)
                ,new SqlParameter("@AreaIds",deliveryCostDto.AreaIds)
                ,new SqlParameter("@AboveValue",deliveryCostDto.AboveValue)
                ,new SqlParameter("@ModifiedBy",deliveryCostDto.ModifiedBy)
                ,new SqlParameter("@ModifiedOn",DateTime.UtcNow)
                ,new SqlParameter("@DeliveryCostJson",JsonConvert.SerializeObject(deliveryCostDto.lstDeliveryCostDetailsDto))
            };





                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private LocationAddressDto getDeliveryCostPerArea(int tenantId, float longitude, float latitude, string city, string area, string distric)
        {
            try
            {
      
                LocationAddressDto LocationAddressDto = new LocationAddressDto();
                var SP_Name = Constants.DeliveryCost.SP_DeliveryCostPerAreaGet;

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@TenantId",tenantId)
                ,new SqlParameter("@Longitude",longitude)
                ,new SqlParameter("@Latitude",latitude)
                ,new SqlParameter("@City",city)
                ,new SqlParameter("@Area",area)
                ,new SqlParameter("@Distric",distric)




            };

                LocationAddressDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToLocationAddressDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return LocationAddressDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion












    }
}
