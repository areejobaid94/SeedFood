using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Customers.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Infoseed.MessagingPortal.Customers
{
	public interface ICustomersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerForViewDto>> GetAll(GetAllCustomersInput input);

        Task<GetCustomerForViewDto> GetCustomerForView(long id);

		Task<GetCustomerForEditOutput> GetCustomerForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditCustomerDto input);

		Task Delete(EntityDto<long> input);

		
		Task<List<CustomerGenderLookupTableDto>> GetAllGenderForTableDropdown();
		
		Task<List<CustomerCityLookupTableDto>> GetAllCityForTableDropdown();
		
    }
}