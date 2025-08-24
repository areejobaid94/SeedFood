using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Currencies.Exporting;
using Infoseed.MessagingPortal.Currencies.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Framework.Data;

namespace Infoseed.MessagingPortal.Currencies
{
    //[AbpAuthorize(AppPermissions.Pages_Administration_Currencies)]
    public class CurrenciesAppService : MessagingPortalAppServiceBase, ICurrenciesAppService
    {
        private readonly IRepository<Currency> _currencyRepository;
        private readonly ICurrenciesExcelExporter _currenciesExcelExporter;

        public CurrenciesAppService(IRepository<Currency> currencyRepository, ICurrenciesExcelExporter currenciesExcelExporter)
        {
            _currencyRepository = currencyRepository;
            _currenciesExcelExporter = currenciesExcelExporter;

        }
        public List<CurrencyDto> GetAllCurrencies()
        {
            return getAllCurrencies();
        }
        public CurrencyDto GetCurrencyByISOName(string ISOName)
        {
            return getCurrencyByISOName(ISOName);
        }

        public async Task<PagedResultDto<GetCurrencyForViewDto>> GetAll(GetAllCurrenciesInput input)
        {

            var filteredCurrencies = _currencyRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.CurrencyName.Contains(input.Filter) || e.ISOName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyNameFilter), e => e.CurrencyName == input.CurrencyNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ISONameFilter), e => e.ISOName == input.ISONameFilter);

            var pagedAndFilteredCurrencies = filteredCurrencies
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var currencies = from o in pagedAndFilteredCurrencies
                             select new GetCurrencyForViewDto()
                             {
                                 Currency = new CurrencyDto
                                 {
                                     CurrencyName = o.CurrencyName,
                                     ISOName = o.ISOName,
                                     Id = o.Id
                                 }
                             };

            var totalCount = await filteredCurrencies.CountAsync();

            return new PagedResultDto<GetCurrencyForViewDto>(
                totalCount,
                await currencies.ToListAsync()
            );
        }

        public async Task<GetCurrencyForViewDto> GetCurrencyForView(int id)
        {
            var currency = await _currencyRepository.GetAsync(id);

            var output = new GetCurrencyForViewDto { Currency = ObjectMapper.Map<CurrencyDto>(currency) };

            return output;
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_Currencies_Edit)]
        public async Task<GetCurrencyForEditOutput> GetCurrencyForEdit(EntityDto input)
        {
            var currency = await _currencyRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCurrencyForEditOutput { Currency = ObjectMapper.Map<CreateOrEditCurrencyDto>(currency) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCurrencyDto input)
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

        //[AbpAuthorize(AppPermissions.Pages_Administration_Currencies_Create)]
        protected virtual async Task Create(CreateOrEditCurrencyDto input)
        {
            var currency = ObjectMapper.Map<Currency>(input);

            await _currencyRepository.InsertAsync(currency);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_Currencies_Edit)]
        protected virtual async Task Update(CreateOrEditCurrencyDto input)
        {
            var currency = await _currencyRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, currency);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_Currencies_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _currencyRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetCurrenciesToExcel(GetAllCurrenciesForExcelInput input)
        {

            var filteredCurrencies = _currencyRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.CurrencyName.Contains(input.Filter) || e.ISOName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyNameFilter), e => e.CurrencyName == input.CurrencyNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ISONameFilter), e => e.ISOName == input.ISONameFilter);

            var query = (from o in filteredCurrencies
                         select new GetCurrencyForViewDto()
                         {
                             Currency = new CurrencyDto
                             {
                                 CurrencyName = o.CurrencyName,
                                 ISOName = o.ISOName,
                                 Id = o.Id
                             }
                         });

            var currencyListDtos = await query.ToListAsync();

            return _currenciesExcelExporter.ExportToFile(currencyListDtos);
        }
        #region Private Methods
        private List<CurrencyDto> getAllCurrencies()
        {
            try
            {
                List<CurrencyDto> currency = new List<CurrencyDto>();
                var SP_Name = Constants.Currencies.SP_CurrenciesGet;

                var sqlParameters = new List<SqlParameter> {};


                currency = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.MapCurrency, AppSettingsModel.ConnectionStrings).ToList();


                return currency;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private CurrencyDto getCurrencyByISOName(string ISOName)
        {
            try
            {
                CurrencyDto model = new CurrencyDto();

                var SP_Name = Constants.Currencies.SP_CurrencyGetByISOName;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@ISOName",ISOName)
                };

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapCurrency, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}