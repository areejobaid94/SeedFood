using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.OrderOffer.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infoseed.MessagingPortal.OrderOffer
{
    public class OrderOfferAppService : MessagingPortalAppServiceBase, IOrderOfferAppService
    {
        private readonly IRepository<OrderOffers.OrderOffer, long> _orderOfferRepository;
        public OrderOfferAppService(IRepository<OrderOffers.OrderOffer, long> orderOfferRepository)
		{
            _orderOfferRepository = orderOfferRepository;

        }

        public async Task<PagedResultDto<GetOrderOfferForViewDto>> GetAll(GetAllOrderOfferInput input)
        {

            var filteredorderoffer = _orderOfferRepository.GetAll()

                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Cities.Contains(input.Filter));

            var pagedAndFilteredorderoffer = filteredorderoffer
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var  orderoffer = from o in pagedAndFilteredorderoffer

                        select new GetOrderOfferForViewDto()
                        {
                             OrderOffer = new OrderOfferDto
                             {
                                Id = o.Id,
                                 isPersentageDiscount=o.isPersentageDiscount,
                                 Area=o.Area,
                                  Cities=o.Cities,
                                   FeesEnd=o.FeesEnd,
                                    FeesStart=o.FeesStart,
                                     isAvailable=o.isAvailable,
                                      OrderOfferEnd=o.OrderOfferEnd,
                                       OrderOfferStart=o.OrderOfferStart, 
                                        BranchesIds=o.BranchesIds,
                                         BranchesName=o.BranchesName,
                                          isBranchDiscount=o.isBranchDiscount
                            },

                        };

            var totalCount = await orderoffer.CountAsync();

            return new PagedResultDto<GetOrderOfferForViewDto>(
                totalCount,
               await orderoffer.ToListAsync()

            );
        }

        public async Task CreateOrEdit(CreateOrEditOrderOfferDto input)
        {

            if (input.Area==null)
            {
                input.Area="undefined";
            }

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        public async Task DeleteOrderOffer(EntityDto<long> input)
        {
            await _orderOfferRepository.DeleteAsync(input.Id);
        }



        public async Task<GetOrderOfferForEditOutput> GetOrderOfferForEdit(EntityDto<long> input)
        {
            var order = await _orderOfferRepository.FirstOrDefaultAsync(input.Id);

            GetOrderOfferForEditOutput getOrderOfferForEditOutput = new GetOrderOfferForEditOutput();

            getOrderOfferForEditOutput.OrderOffer.Area = order.Area;
            getOrderOfferForEditOutput.OrderOffer.isPersentageDiscount = order.isPersentageDiscount;

            getOrderOfferForEditOutput.OrderOffer.Cities = order.Cities;
            getOrderOfferForEditOutput.OrderOffer.FeesEnd  = order.FeesEnd;
            getOrderOfferForEditOutput.OrderOffer.FeesStart = order.FeesStart;

            getOrderOfferForEditOutput.OrderOffer.isAvailable = order.isAvailable;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferEnd = order.OrderOfferEnd;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferStart = order.OrderOfferStart;//.AddHours(AppSettingsModel.AddHour);

            getOrderOfferForEditOutput.OrderOffer.OrderOfferEndS = order.OrderOfferEndS;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferStartS = order.OrderOfferStartS;

            getOrderOfferForEditOutput.OrderOffer.Id = order.Id;

            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateEnd = order.OrderOfferDateEnd;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateStart = order.OrderOfferDateStart;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateEndS = order.OrderOfferDateEndS;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateStartS = order.OrderOfferDateStartS;


            getOrderOfferForEditOutput.OrderOffer.isBranchDiscount = order.isBranchDiscount;
            getOrderOfferForEditOutput.OrderOffer.BranchesName = order.BranchesName;
            getOrderOfferForEditOutput.OrderOffer.BranchesIds = order.BranchesIds;

            return getOrderOfferForEditOutput;
        }

        public async Task<GetOrderOfferForViewDto> GetOrderOfferForView(long id)
        {
            var order = await _orderOfferRepository.FirstOrDefaultAsync(id);

            GetOrderOfferForViewDto getOrderOfferForEditOutput = new GetOrderOfferForViewDto();

            getOrderOfferForEditOutput.OrderOffer.Area = order.Area;
            getOrderOfferForEditOutput.OrderOffer.Cities = order.Cities;
            getOrderOfferForEditOutput.OrderOffer.FeesEnd = order.FeesEnd;
            getOrderOfferForEditOutput.OrderOffer.FeesStart = order.FeesStart;

            getOrderOfferForEditOutput.OrderOffer.isPersentageDiscount = order.isPersentageDiscount;
            getOrderOfferForEditOutput.OrderOffer.isAvailable = order.isAvailable;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferEnd = order.OrderOfferEnd;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferStart = order.OrderOfferStart;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferEndS = order.OrderOfferEndS;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferStartS = order.OrderOfferStartS;
            getOrderOfferForEditOutput.OrderOffer.Id = order.Id;




            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateEnd = order.OrderOfferDateEnd;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateStart = order.OrderOfferDateStart;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateEndS = order.OrderOfferDateEndS;
            getOrderOfferForEditOutput.OrderOffer.OrderOfferDateStartS = order.OrderOfferDateStartS;


            getOrderOfferForEditOutput.OrderOffer.isBranchDiscount = order.isBranchDiscount;
            getOrderOfferForEditOutput.OrderOffer.BranchesName = order.BranchesName;
            getOrderOfferForEditOutput.OrderOffer.BranchesIds = order.BranchesIds;
            return getOrderOfferForEditOutput;
        }
        protected virtual async Task Create(CreateOrEditOrderOfferDto input)
        {


      
            //var order = ObjectMapper.Map<OrderOffers.OrderOffer>(input);
            OrderOffers.OrderOffer orderOffer = new OrderOffers.OrderOffer();
            if (input.isPersentageDiscount)
            {
                orderOffer = new OrderOffers.OrderOffer
                {
                    Area = "",

                    CreationTime = DateTime.UtcNow,
                    Cities = "",
                    isAvailable = true,
                    FeesEnd = input.FeesEnd,
                    FeesStart = input.FeesStart,
                    OrderOfferEndS = input.OrderOfferEndS,
                    OrderOfferStartS = input.OrderOfferStartS,
                    NewFees = input.NewFees,
                    OrderOfferEnd = input.OrderOfferEnd,//.AddHours(AppSettingsModel.AddHour),
                    OrderOfferStart = input.OrderOfferStart,//.AddHours(AppSettingsModel.AddHour),
                    TenantId = (int?)AbpSession.TenantId,
                    IsDeleted = false,
                    isPersentageDiscount = input.isPersentageDiscount,
                    OrderOfferDateStart = input.OrderOfferDateStart,
                    OrderOfferDateEnd = input.OrderOfferDateEnd,
                    OrderOfferDateEndS = input.OrderOfferDateEndS,
                    OrderOfferDateStartS = input.OrderOfferDateStartS,
                     isBranchDiscount = input.isBranchDiscount,
                      BranchesName = input.BranchesName,
                       BranchesIds = input.BranchesIds
                     


                };
            }
            else
            {
                orderOffer = new OrderOffers.OrderOffer
                {
                    Area = input.Area.Replace("undefined,", ""),

                    CreationTime = DateTime.UtcNow,
                    Cities = input.Cities,
                    isAvailable = true,
                    FeesEnd = input.FeesEnd,
                    FeesStart = input.FeesStart,
                    OrderOfferEndS = input.OrderOfferEndS,
                    OrderOfferStartS = input.OrderOfferStartS,
                    NewFees = input.NewFees,
                    OrderOfferEnd = input.OrderOfferEnd,//.AddHours(AppSettingsModel.AddHour),
                    OrderOfferStart = input.OrderOfferStart,//.AddHours(AppSettingsModel.AddHour),
                    TenantId = (int?)AbpSession.TenantId,
                    IsDeleted = false,
                    isPersentageDiscount = input.isPersentageDiscount,
                    OrderOfferDateStart = input.OrderOfferDateStart,
                    OrderOfferDateEnd = input.OrderOfferDateEnd,
                    OrderOfferDateEndS = input.OrderOfferDateEndS,
                    OrderOfferDateStartS = input.OrderOfferDateStartS,
                    isBranchDiscount = input.isBranchDiscount,
                    BranchesName = input.BranchesName,
                    BranchesIds = input.BranchesIds



                };
            }
            
         
      
            await _orderOfferRepository.InsertAsync(orderOffer);
        }

        protected virtual async Task Update(CreateOrEditOrderOfferDto input)
        {
            var order = await _orderOfferRepository.FirstOrDefaultAsync((long)input.Id);


            if (input.isPersentageDiscount)
            {
                order.Area = "";


                order.isPersentageDiscount = input.isPersentageDiscount;

                order.Cities = "";
                order.isAvailable = input.isAvailable;
                order.FeesEnd = input.FeesEnd;
                order.FeesStart = input.FeesStart;
                order.NewFees = input.NewFees;
                order.OrderOfferEnd = input.OrderOfferEnd;
                order.OrderOfferStart = input.OrderOfferStart;
                order.TenantId = (int?)AbpSession.TenantId;


                order.OrderOfferDateEnd = input.OrderOfferDateEnd;
                order.OrderOfferDateStart = input.OrderOfferDateStart;


                order.isBranchDiscount = input.isBranchDiscount;
                order.BranchesName = input.BranchesName;
                order.BranchesIds = input.BranchesIds;

            }
            else
            {
                order.Area = input.Area;


                order.isPersentageDiscount = input.isPersentageDiscount;

                order.Cities = input.Cities;
                order.isAvailable = input.isAvailable;
                order.FeesEnd = input.FeesEnd;
                order.FeesStart = input.FeesStart;
                order.NewFees = input.NewFees;
                order.OrderOfferEnd = input.OrderOfferEnd;
                order.OrderOfferStart = input.OrderOfferStart;
                order.TenantId = (int?)AbpSession.TenantId;


                order.OrderOfferDateEnd = input.OrderOfferDateEnd;
                order.OrderOfferDateStart = input.OrderOfferDateStart;

                order.isBranchDiscount = input.isBranchDiscount;
                order.BranchesName = input.BranchesName;
                order.BranchesIds = input.BranchesIds;
            }

               

            // order.IsDeleted = false;

            await _orderOfferRepository.UpdateAsync(order);
        }
       
    }
}
