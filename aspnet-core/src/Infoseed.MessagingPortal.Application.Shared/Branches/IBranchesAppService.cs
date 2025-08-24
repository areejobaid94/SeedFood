using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Branches.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.Branches
{
    public interface IBranchesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBranchForViewDto>> GetAll(GetAllBranchesInput input);

        Task<GetBranchForViewDto> GetBranchForView(long id);

		Task<GetBranchForEditOutput> GetBranchForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditBranchDto input);

		Task Delete(EntityDto<long> input);

        void testinfosed();



    }
}