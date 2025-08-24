using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Currencies.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Currencies
{
    public interface ICurrenciesAppService : IApplicationService
    {
        List<CurrencyDto> GetAllCurrencies();
        CurrencyDto GetCurrencyByISOName(string ISOName);

        Task<PagedResultDto<GetCurrencyForViewDto>> GetAll(GetAllCurrenciesInput input);

        Task<GetCurrencyForViewDto> GetCurrencyForView(int id);

        Task<GetCurrencyForEditOutput> GetCurrencyForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditCurrencyDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetCurrenciesToExcel(GetAllCurrenciesForExcelInput input);

    }
}