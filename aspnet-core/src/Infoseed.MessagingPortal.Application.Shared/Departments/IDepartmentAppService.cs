using Abp.Application.Services;
using Infoseed.MessagingPortal.Departments.Dto;

namespace Infoseed.MessagingPortal.Departments
{
    public interface IDepartmentAppService : IApplicationService
    {
        DepartmentEntity GetDepartments(int? tenantId = null, int pageNumber = 0, int pageSize = 50);
        bool UpdateDepartment(DepartmentModel department);
        DepartmentModel GetDepartmentById(long departmentId,bool isLiveChat,bool isRequest);
    }
}
