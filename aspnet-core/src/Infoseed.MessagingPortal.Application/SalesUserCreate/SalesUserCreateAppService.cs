using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.SalesUserCreate.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.SalesUserCreate
{
    public class SalesUserCreateAppService : MessagingPortalAppServiceBase, ISalesUserCreateAppService
    {
        private readonly IRepository<SalesUserCreate> _salesUserCreateRepository;

        public SalesUserCreateAppService(IRepository<SalesUserCreate> salesUserCreateRepository)
        {
            _salesUserCreateRepository = salesUserCreateRepository;

        }

        public Task CreateOrEdit(CreateOrEditSalesUserCreateDto input)
        {
            throw new NotImplementedException();
        }

        public Task Delete(EntityDto input)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<GetSalesUserCreateForViewDto>> GetAll(GetAllSalesUserCreateInput input)
        {
            throw new NotImplementedException();
        }

        public Task<GetSalesUserCreateForEditOutput> GetSalesUserCreateForEdit(EntityDto input)
        {
            throw new NotImplementedException();
        }

        public Task<GetSalesUserCreateForViewDto> GetSalesUserCreateForView(int id)
        {
            throw new NotImplementedException();
        }
    }
}
