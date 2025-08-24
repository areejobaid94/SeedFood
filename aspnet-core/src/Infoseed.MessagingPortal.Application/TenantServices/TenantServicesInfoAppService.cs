using Framework.Data;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.TenantServicesInfo
{
    public class TenantServicesInfoAppService : MessagingPortalAppServiceBase, ITenantServicesInfoAppService
    {

         
        public TenantInfoForOrdaringSystemDto GetTenantsById(int tenantId,int contactId)
        {
            return  getTenantsById(tenantId, contactId);
        }

        public ContactDto GetContactbyId(int id)
        {
            return getContactbyId(id);
        }

        #region  Privet method 

        private TenantInfoForOrdaringSystemDto getTenantsById(int tenantId, int contactId)
        {
            TenantInfoForOrdaringSystemDto tenantInfoDto = new TenantInfoForOrdaringSystemDto();



            try
            {
                var SP_Name = Constants.Tenant.SP_TenantsByIdGet;

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@Id",tenantId),
                 new SqlParameter("@ContactId",contactId)

            };


                tenantInfoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                    DataReaderMapper.ConvertTenantInformationDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return tenantInfoDto;

            }
            catch (Exception ex)
            {
                return tenantInfoDto;
                //throw ex;
            }






        }
        private ContactDto getContactbyId(int id)
        {
            try
            {
                ContactDto contactDto = new ContactDto();
                var SP_Name = Constants.Contacts.SP_ContactbyIdGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
               new System.Data.SqlClient.SqlParameter("@Id",id)
                 };

                contactDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapContact, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return contactDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion




    }
}
