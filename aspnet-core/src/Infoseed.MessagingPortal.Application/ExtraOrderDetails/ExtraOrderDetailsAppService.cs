using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.OrderDetails;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ExtraOrderDetails
{
    //[AbpAuthorize(AppPermissions.Pages_MenuCategories)]
    public class ExtraOrderDetailsAppService : MessagingPortalAppServiceBase, IExtraOrderDetailsAppService
    {
        private readonly IRepository<OrderDetail, long> _orderDetailRepository;
        private readonly IRepository<ExtraOrderDetail, long> _extraOrderDetailRepository;


        public ExtraOrderDetailsAppService(IRepository<OrderDetail, long> orderDetailRepository, IRepository<ExtraOrderDetail, long> extraOrderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _extraOrderDetailRepository = extraOrderDetailRepository;

        }
        public async Task<PagedResultDto<GetExtraOrderDetailsForViewDto>> GetAll(GetAllExtraOrderDetailsInput input)
        {
            var filteredExtraOrderDetails = _extraOrderDetailRepository.GetAll()
                     .Include(e => e.OrderDetailFk)
                     .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter);


            var extraOrderDetails = from o in filteredExtraOrderDetails
                                    select new GetExtraOrderDetailsForViewDto()                            
                                    {
                                  
                                        extraOrderDetailsDto = new   ExtraOrderDetailsDto {
                                      
                                            Id = o.Id,                                     
                                             Name = o.Name,   
                                              NameEnglish=o.NameEnglish,
                                             OrderDetailId = o.OrderDetailId,                                       
                                             Quantity = o.Quantity,
                                              TenantId=o.TenantId,
                                               Total=o.Total,
                                                UnitPrice=o.UnitPrice
                                        }                              
                                    };


            var totalCount = filteredExtraOrderDetails.Count();
            var list = filteredExtraOrderDetails.ToList();

            return new PagedResultDto<GetExtraOrderDetailsForViewDto>(
                totalCount,
                 extraOrderDetails.ToList()
            );
        }
        public async Task<long> CreateOrEdit(CreateOrEditExtraOrderDetailsDto input)
        {
            if (input.Id == null)
            {
                return await Create(input);
            }
            else
            {
                await Update(input);
            }

            return 0;
        }

        protected virtual async Task<long> Create(CreateOrEditExtraOrderDetailsDto input)
        {

            var ExtraOrderDetail = new ExtraOrderDetail();

            ExtraOrderDetail.OrderDetailId  = input.OrderDetailId;
            ExtraOrderDetail.Name = input.Name;
            ExtraOrderDetail.Quantity  = input.Quantity;
            ExtraOrderDetail.UnitPrice = input.UnitPrice;
            ExtraOrderDetail.Total = input.Total;
            ExtraOrderDetail.TenantId = (int?)AbpSession.TenantId;
            

            var entityId = await _extraOrderDetailRepository.InsertAndGetIdAsync(ExtraOrderDetail);

            return entityId;

        }

        protected virtual async Task Update(CreateOrEditExtraOrderDetailsDto input)
        {
            var ExtraOrderDetail = await _extraOrderDetailRepository.FirstOrDefaultAsync((long)input.Id);

            ExtraOrderDetail.OrderDetailId = input.OrderDetailId;
            ExtraOrderDetail.Name = input.Name;
            ExtraOrderDetail.Quantity = input.Quantity;
            ExtraOrderDetail.UnitPrice = input.UnitPrice;
            ExtraOrderDetail.Total = input.Total;
            ExtraOrderDetail.TenantId = (int?)AbpSession.TenantId;


            await _extraOrderDetailRepository.UpdateAsync(ExtraOrderDetail);
        }

        public async Task Delete(EntityDto<long> input)
        {
            await _extraOrderDetailRepository.DeleteAsync(input.Id);
        }

      

        public List<ExtraOrderDetailsDto> GetAllWithTenantID(int? TenantID)
        {
            throw new NotImplementedException();
        }

    
        public Task<GetExtraOrderDetailsForViewDto> GetExtraOrderDetailsForView(long id)
        {
            throw new NotImplementedException();
        }

        public Task<GetExtraOrderDetailsForEditOutput> GetExtraOrderDetailsForEdit(EntityDto<long> input)
        {
            throw new NotImplementedException();
        }
    }
}