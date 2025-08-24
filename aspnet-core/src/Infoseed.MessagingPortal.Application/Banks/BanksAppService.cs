using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Banks.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.Banks
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Banks)]
    public class BanksAppService : MessagingPortalAppServiceBase, IBanksAppService
    {
        private readonly IRepository<Bank> _bankRepository;

        public BanksAppService(IRepository<Bank> bankRepository)
        {
            _bankRepository = bankRepository;

        }

        public async Task<PagedResultDto<GetBankForViewDto>> GetAll(GetAllBanksInput input)
        {
            try
            {
                var filteredBanks = _bankRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.BankName.Contains(input.Filter));

                var pagedAndFilteredBanks = filteredBanks
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var banks = from o in pagedAndFilteredBanks
                            select new GetBankForViewDto()
                            {
                                Bank = new BankDto
                                {
                                    BankName = o.BankName,
                                    Id = o.Id
                                }
                            };

                var totalCount = await filteredBanks.CountAsync();

                return new PagedResultDto<GetBankForViewDto>(
                    totalCount,
                    await banks.ToListAsync()
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<GetBankForViewDto> GetBankForView(int id)
        {
            var bank = await _bankRepository.GetAsync(id);

            var output = new GetBankForViewDto { Bank = ObjectMapper.Map<BankDto>(bank) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Banks_Edit)]
        public async Task<GetBankForEditOutput> GetBankForEdit(EntityDto input)
        {
            try
            {
                var bank = await _bankRepository.FirstOrDefaultAsync(input.Id);

                var output = new GetBankForEditOutput { Bank = ObjectMapper.Map<CreateOrEditBankDto>(bank) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task CreateOrEdit(CreateOrEditBankDto input)
        {
            try
            {
                if (input.Id == null)
                {
                    await Create(input);
                }
                else
                {
                    await Update(input);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Banks_Create)]
        protected virtual async Task Create(CreateOrEditBankDto input)
        {
            try
            {
                var bank = ObjectMapper.Map<Bank>(input);

                await _bankRepository.InsertAsync(bank);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Banks_Edit)]
        protected virtual async Task Update(CreateOrEditBankDto input)
        {
            try
            {
                var bank = await _bankRepository.FirstOrDefaultAsync((int)input.Id);
                ObjectMapper.Map(input, bank);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Banks_Delete)]
        public async Task Delete(EntityDto input)
        {
            try
            {
                await _bankRepository.DeleteAsync(input.Id);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}