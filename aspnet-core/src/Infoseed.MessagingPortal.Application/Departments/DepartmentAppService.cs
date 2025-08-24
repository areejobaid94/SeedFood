using Framework.Data;
using Infoseed.MessagingPortal.Departments.Dto;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Infoseed.MessagingPortal.Departments
{
    public class DepartmentAppService : MessagingPortalAppServiceBase, IDepartmentAppService
    {
        public DepartmentAppService()
        {

        }
        public DepartmentEntity GetDepartments(int? tenantId = null,int pageNumber = 0, int pageSize = 50)
        {
            return getDepartments(tenantId, pageNumber,pageSize);
        }

        public bool UpdateDepartment(DepartmentModel department)
        {
            return updateDepartment(department); 
        }

        public DepartmentModel GetDepartmentById(long departmentId, bool isLiveChat, bool isRequest)
        {
            return getDepartmentById(departmentId,isLiveChat, isRequest);
        }









        #region Private Methods
        private DepartmentEntity getDepartments(int? tenantId = null, int pageNumber = 0, int pageSize = 50)
        {
            try
            {
                DepartmentEntity departments = new DepartmentEntity();

                var SP_Name = Constants.Depatrment.SP_DepartmentsGet;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@TenantId",tenantId),
                    new SqlParameter("@PageNumber",pageNumber),
                    new SqlParameter("@PageSize",pageSize)

                };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.Int;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);

                departments.lstDepartments = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.MapDepartment, AppSettingsModel.ConnectionStrings).ToList();
                departments.TotalCount = (int)OutputParameter.Value;
                return departments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool updateDepartment(DepartmentModel department)
        {
            try
            {
                var SP_Name = Constants.Depatrment.SP_DepartmentUpdate;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@DepartmentId",department.Id),
                    new SqlParameter("@TenantId",department.TenantId),
                    new SqlParameter("@UserIds",department.UserIds),
                    new SqlParameter("@ModifiedBy",AbpSession.UserId.Value),
                    new SqlParameter("@ModifiedDate",DateTime.UtcNow)
                };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),AppSettingsModel.ConnectionStrings);

                return (bool)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DepartmentModel getDepartmentById(long departmentId, bool isLiveChat, bool isRequest)
        {
            try
            {
                DepartmentModel department = new DepartmentModel();

                var SP_Name = Constants.Depatrment.SP_DepartmentByDepartmentIdGet;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@DepartmentId",departmentId),
                    new SqlParameter("@IsLiveChat",isLiveChat),
                    new SqlParameter("@IsRequest",isRequest),

                };


                department = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapDepartment, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return department;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
