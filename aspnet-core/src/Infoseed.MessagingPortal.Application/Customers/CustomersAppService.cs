using Infoseed.MessagingPortal.Genders;
using System.Collections.Generic;
using Infoseed.MessagingPortal.Cities;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Customers.Dtos;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.Customers
{
	[AbpAuthorize(AppPermissions.Pages_Customers)]
    public class CustomersAppService : MessagingPortalAppServiceBase, ICustomersAppService
    {
		 private readonly IRepository<Customer, long> _customerRepository;
		 private readonly IRepository<Gender,long> _lookup_genderRepository;
		 private readonly IRepository<City,long> _lookup_cityRepository;
		 

		  public CustomersAppService(IRepository<Customer, long> customerRepository , IRepository<Gender, long> lookup_genderRepository, IRepository<City, long> lookup_cityRepository) 
		  {
			_customerRepository = customerRepository;
			_lookup_genderRepository = lookup_genderRepository;
			_lookup_cityRepository = lookup_cityRepository;
		
		  }

		 public async Task<PagedResultDto<GetCustomerForViewDto>> GetAll(GetAllCustomersInput input)
         {
			
			var filteredCustomers = _customerRepository.GetAll()
						.Include( e => e.GenderFk)
						.Include( e => e.CityFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.CustomerName.Contains(input.Filter) || e.PhoneNumber.Contains(input.Filter) || e.CustomerAddress.Contains(input.Filter) || e.EmailAddress.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter),  e => e.CustomerName == input.CustomerNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.PhoneNumberFilter),  e => e.PhoneNumber == input.PhoneNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerAddressFilter),  e => e.CustomerAddress == input.CustomerAddressFilter)
						.WhereIf(input.MinCreationTimeFilter != null, e => e.CreationTime >= input.MinCreationTimeFilter)
						.WhereIf(input.MaxCreationTimeFilter != null, e => e.CreationTime <= input.MaxCreationTimeFilter)
						.WhereIf(input.MinDeletionTimeFilter != null, e => e.DeletionTime >= input.MinDeletionTimeFilter)
						.WhereIf(input.MaxDeletionTimeFilter != null, e => e.DeletionTime <= input.MaxDeletionTimeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EmailAddressFilter),  e => e.EmailAddress == input.EmailAddressFilter)
						.WhereIf(input.IsActiveFilter.HasValue && input.IsActiveFilter > -1,  e => (input.IsActiveFilter == 1 && e.IsActive) || (input.IsActiveFilter == 0 && !e.IsActive) )
						.WhereIf(input.IsDeletedFilter.HasValue && input.IsDeletedFilter > -1,  e => (input.IsDeletedFilter == 1 && e.IsDeleted) || (input.IsDeletedFilter == 0 && !e.IsDeleted) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.GenderNameFilter), e => e.GenderFk != null && e.GenderFk.Name == input.GenderNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CityNameFilter), e => e.CityFk != null && e.CityFk.Name == input.CityNameFilter);

			var pagedAndFilteredCustomers = filteredCustomers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var customers = from o in pagedAndFilteredCustomers
                         join o1 in _lookup_genderRepository.GetAll() on o.GenderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_cityRepository.GetAll() on o.CityId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetCustomerForViewDto() {
							Customer = new CustomerDto
							{
                                CustomerName = o.CustomerName,
                                PhoneNumber = o.PhoneNumber,
                                CustomerAddress = o.CustomerAddress,
                                CreationTime = o.CreationTime,
                                DeletionTime = o.DeletionTime,
                                EmailAddress = o.EmailAddress,
                                IsActive = o.IsActive,
                                IsDeleted = o.IsDeleted,
                                Id = o.Id
							},
                         	GenderName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                         	CityName = s2 == null || s2.Name == null ? "" : s2.Name.ToString()
						};

            var totalCount = await filteredCustomers.CountAsync();

            return new PagedResultDto<GetCustomerForViewDto>(
                totalCount,
                await customers.ToListAsync()
            );
         }
		 
		 public async Task<GetCustomerForViewDto> GetCustomerForView(long id)
         {
            var customer = await _customerRepository.GetAsync(id);

            var output = new GetCustomerForViewDto { Customer = ObjectMapper.Map<CustomerDto>(customer) };

		    if (output.Customer.GenderId != null)
            {
                var _lookupGender = await _lookup_genderRepository.FirstOrDefaultAsync((long)output.Customer.GenderId);
                output.GenderName = _lookupGender?.Name?.ToString();
            }

		    if (output.Customer.CityId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((long)output.Customer.CityId);
                output.CityName = _lookupCity?.Name?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Customers_Edit)]
		 public async Task<GetCustomerForEditOutput> GetCustomerForEdit(EntityDto<long> input)
         {
            var customer = await _customerRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCustomerForEditOutput {Customer = ObjectMapper.Map<CreateOrEditCustomerDto>(customer)};

		    if (output.Customer.GenderId != null)
            {
                var _lookupGender = await _lookup_genderRepository.FirstOrDefaultAsync((long)output.Customer.GenderId);
                output.GenderName = _lookupGender?.Name?.ToString();
            }

		    if (output.Customer.CityId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((long)output.Customer.CityId);
                output.CityName = _lookupCity?.Name?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCustomerDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Customers_Create)]
		 protected virtual async Task Create(CreateOrEditCustomerDto input)
         {
            var customer = ObjectMapper.Map<Customer>(input);

			
			if (AbpSession.TenantId != null)
			{
				customer.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _customerRepository.InsertAsync(customer);
         }

		 [AbpAuthorize(AppPermissions.Pages_Customers_Edit)]
		 protected virtual async Task Update(CreateOrEditCustomerDto input)
         {
            var customer = await _customerRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, customer);
         }

		 [AbpAuthorize(AppPermissions.Pages_Customers_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _customerRepository.DeleteAsync(input.Id);
         } 
			[AbpAuthorize(AppPermissions.Pages_Customers)]
			public async Task<List<CustomerGenderLookupTableDto>> GetAllGenderForTableDropdown()
			{
				return await _lookup_genderRepository.GetAll()
					.Select(gender => new CustomerGenderLookupTableDto
					{
						Id = gender.Id,
						DisplayName = gender == null || gender.Name == null ? "" : gender.Name.ToString()
					}).ToListAsync();
			}
							
			[AbpAuthorize(AppPermissions.Pages_Customers)]
			public async Task<List<CustomerCityLookupTableDto>> GetAllCityForTableDropdown()
			{
				return await _lookup_cityRepository.GetAll()
					.Select(city => new CustomerCityLookupTableDto
					{
						Id = city.Id,
						DisplayName = city == null || city.Name == null ? "" : city.Name.ToString()
					}).ToListAsync();
			}
							
    }
}