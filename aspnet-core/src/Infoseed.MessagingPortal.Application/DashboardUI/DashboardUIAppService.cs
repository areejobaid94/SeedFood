using Framework.Data;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;

namespace Infoseed.MessagingPortal.DashboardUI
{
    public class DashboardUIAppService : MessagingPortalAppServiceBase, IDashboardUIAppService
    {
        public DashboardUIAppService() { }
        public DashboardNumbersEntity GetDashboardNumbers(int tenantId, string start, string end)
        {
            DateTime startDateTime = DateTime.ParseExact(start, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(end, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            return getDashboardNumbers(tenantId, startDateTime, endDateTime);
        }
        public long CreateDashboardNumber(DashboardNumbers numbers)
        {
            return createDashboardNumber(numbers);
        }














        #region Private Methods

        private DashboardNumbersEntity getDashboardNumbers(int tenantId, DateTime start, DateTime end)
        {
            try
            {
                DashboardNumbersEntity dashboardNumbersEntity = new DashboardNumbersEntity();
                var SP_Name = Constants.Dashboard.SP_DashboardNumbersGet;
                end = end.AddHours(12);
                var sqlParameters = new List<SqlParameter>
                {
                   new SqlParameter("@TenantId",tenantId),
                   new SqlParameter("@Start",start),
                   new SqlParameter("@End", end),
                };

                dashboardNumbersEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapDashboardNumbers, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return dashboardNumbersEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long createDashboardNumber(DashboardNumbers numbers)
        {
            try
            {
                if (numbers != null)
                {
                    var SP_Name = Constants.Dashboard.SP_DashboardNumberAdd;
                    var sqlParameters = new List<SqlParameter>
                    {
                         new SqlParameter("@TenantId",numbers.TenantId)
                        ,new SqlParameter("@CreationDateTime",numbers.CreatetionDateTime)
                        ,new SqlParameter("@TypeId",numbers.TypeId)
                        ,new SqlParameter("@TypeName",Enum.GetName(typeof(DashboardTypeEnum), numbers.TypeId))
                        ,new SqlParameter("@StatusId",numbers.StatusId)
                        ,new SqlParameter("@StatusName",numbers.StatusName)
                    };
                    var x = Enum.GetName(typeof(DashboardTypeEnum), 2);
                    var OutputParameter = new SqlParameter
                    {
                        SqlDbType = System.Data.SqlDbType.BigInt,
                        ParameterName = "@Id",
                        Direction = System.Data.ParameterDirection.Output
                    };


                    sqlParameters.Add(OutputParameter);
                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                    return (long)OutputParameter.Value;
                }
                else
                {
                    return 0;
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
