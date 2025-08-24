//using Abp.Application.Services.Dto;
//using Abp.Domain.Repositories;
//using Abp.Linq.Extensions;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Territories.Dtos;
//using Infoseed.MessagingPortal.Territories.Exporting;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;

//namespace Infoseed.MessagingPortal.Territories
//{
//    //[AbpAuthorize(AppPermissions.Pages_Territories)]
//    public class TerritoriesAppService : MessagingPortalAppServiceBase, ITerritoriesAppService
//    {
//        private readonly IRepository<Territory> _territoryRepository;
//        private readonly ITerritoriesExcelExporter _territoriesExcelExporter;
//        private readonly IRepository<SalesUserCreate.SalesUserCreate> _salesUserCreateRepository;

//        public TerritoriesAppService(IRepository<Territory> territoryRepository, ITerritoriesExcelExporter territoriesExcelExporter, IRepository<SalesUserCreate.SalesUserCreate> salesUserCreateRepository)
//        {
//            _territoryRepository = territoryRepository;
//            _territoriesExcelExporter = territoriesExcelExporter;
//            _salesUserCreateRepository = salesUserCreateRepository;
//        }

//        public async Task<PagedResultDto<GetTerritoryForViewDto>> GetAll(GetAllTerritoriesInput input)
//        {

      


//            var filteredTerritories = _territoryRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.EnglishName.Contains(input.Filter) || e.ArabicName.Contains(input.Filter) || e.FacebookUri.Contains(input.Filter) || e.Phone.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.Age.Contains(input.Filter))
//                        //.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.EnglishNameFilter), e => e.EnglishName == input.EnglishNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.ArabicNameFilter), e => e.ArabicName == input.ArabicNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.FacebookUriFilter), e => e.FacebookUri == input.FacebookUriFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneFilter), e => e.Phone == input.PhoneFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email == input.EmailFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.AgeFilter), e => e.Age == input.AgeFilter)
//                        .WhereIf(input.MinCreationDateFilter != null, e => e.CreationDate >= input.MinCreationDateFilter)
//                        .WhereIf(input.MaxCreationDateFilter != null, e => e.CreationDate <= input.MaxCreationDateFilter);

//            var pagedAndFilteredTerritories = filteredTerritories
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var territories = from o in pagedAndFilteredTerritories
//                              select new
//                              {

//                                  o.UserName,
//                                  o.EnglishName,
//                                  o.ArabicName,
//                                  o.FacebookUri,
//                                  o.Phone,
//                                  o.Email,
//                                  o.Age,
//                                  o.CreationDate,
//                                  Id = o.Id
//                              };

//            var totalCount = await filteredTerritories.CountAsync();

//            var dbList = await territories.ToListAsync();
//            var results = new List<GetTerritoryForViewDto>();


//            var now = DateTime.UtcNow;


//            var list = _salesUserCreateRepository.GetAll().ToList();

//            var found = list.Where(x => x.UserName == input.UserNameFilter).FirstOrDefault();

//            if (found != null)
//            {
//                foreach (var o in dbList)
//                {
//                    var res = new GetTerritoryForViewDto()
//                    {
//                        Territory = new TerritoryDto
//                        {

//                            UserName = o.UserName,
//                            EnglishName = o.EnglishName,
//                            ArabicName = o.ArabicName,
//                            FacebookUri = o.FacebookUri,
//                            Phone = o.Phone,
//                            Email = o.Email,
//                            Age = (45 - (now.Date - o.CreationDate.Date).Days).ToString(),
//                            CreationDate = o.CreationDate,
//                            Id = o.Id,
//                        },
//                        IsActiveButtonCreate = found.IsActiveButton


//                    };

//                    results.Add(res);
//                }
//            }
//            else
//            {
//                foreach (var o in dbList)
//                {
//                    var res = new GetTerritoryForViewDto()
//                    {
//                        Territory = new TerritoryDto
//                        {

//                            UserName = o.UserName,
//                            EnglishName = o.EnglishName,
//                            ArabicName = o.ArabicName,
//                            FacebookUri = o.FacebookUri,
//                            Phone = o.Phone,
//                            Email = o.Email,
//                            Age = (45 - (now.Date - o.CreationDate.Date).Days).ToString(),
//                            CreationDate = o.CreationDate,
//                            Id = o.Id,
//                        },
//                        IsActiveButtonCreate =true,
    

//                };

//                    results.Add(res);
//                }
//            }

            

//            return new PagedResultDto<GetTerritoryForViewDto>(
//                totalCount,
//                results
//            );

//        }

//        public async Task<GetTerritoryForViewDto> GetTerritoryForView(int id)
//        {
//            var territory = await _territoryRepository.GetAsync(id);

//            var output = new GetTerritoryForViewDto { Territory = ObjectMapper.Map<TerritoryDto>(territory) };

//            return output;
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Territories_Edit)]
//        public async Task<GetTerritoryForEditOutput> GetTerritoryForEdit(EntityDto input)
//        {
//            var territory = await _territoryRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetTerritoryForEditOutput { Territory = ObjectMapper.Map<CreateOrEditTerritoryDto>(territory) };

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditTerritoryDto input)
//        {
//            if (input.Id == null)
//            {
//                await Create(input);
//            }
//            else
//            {
//                await Update(input);
//            }
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Territories_Create)]
//        protected virtual async Task Create(CreateOrEditTerritoryDto input)
//        {

//            var list = _salesUserCreateRepository.GetAll().ToList();

//            var found = list.Where(x => x.UserName == input.UserName).FirstOrDefault();

//            if (found != null)
//            {

//                if (found.TotalCreate == 0)
//                {
//                    SalesUserCreate.SalesUserCreate salesUserCreate = new SalesUserCreate.SalesUserCreate
//                    {
//                        Id = found.Id,
//                        IsActiveButton = false,
//                        TenantId = AbpSession.TenantId,
//                        TotalCreate = 0,
//                        UserId = 0,
//                        UserName = input.UserName
//                    };
//                    UpdateSalesUserCreate(salesUserCreate);
//                    //var x = _salesUserCreateRepository.Update(salesUserCreate);
//                }
//                else
//                {
//                    SalesUserCreate.SalesUserCreate salesUserCreate = new SalesUserCreate.SalesUserCreate
//                    {
//                        Id = found.Id,
//                        IsActiveButton = true,
//                        TenantId = AbpSession.TenantId,
//                        TotalCreate = found.TotalCreate - 1,
//                        UserId = 0,
//                        UserName = input.UserName
//                    };

//                    UpdateSalesUserCreate(salesUserCreate);
//                    //var x= _salesUserCreateRepository.Update(salesUserCreate);

//                }

                

               
//            }
//            else
//            {
//                SalesUserCreate.SalesUserCreate salesUserCreate = new SalesUserCreate.SalesUserCreate
//                {
//                     IsActiveSubmitButton=true,
//                    IsActiveButton = true,
//                    TenantId = AbpSession.TenantId,
//                    TotalCreate = 49,
//                    UserId = 0,
//                    UserName = input.UserName
//                };

//                await _salesUserCreateRepository.InsertAsync(salesUserCreate);

//            }

            

//            input.CreationDate = DateTime.Now;


//            var territory = ObjectMapper.Map<Territory>(input);

//            await _territoryRepository.InsertAsync(territory);

//        }

//        //[AbpAuthorize(AppPermissions.Pages_Territories_Edit)]
//        protected virtual async Task Update(CreateOrEditTerritoryDto input)
//        {
//            var territory = await _territoryRepository.FirstOrDefaultAsync((int)input.Id);
//            ObjectMapper.Map(input, territory);

//        }

//        //[AbpAuthorize(AppPermissions.Pages_Territories_Delete)]
//        public async Task Delete(EntityDto input)
//        {
//            await _territoryRepository.DeleteAsync(input.Id);
//        }

//        public async Task<FileDto> GetTerritoriesToExcel(GetAllTerritoriesForExcelInput input)
//        {

//            var filteredTerritories = _territoryRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.EnglishName.Contains(input.Filter) || e.ArabicName.Contains(input.Filter) || e.FacebookUri.Contains(input.Filter) || e.Phone.Contains(input.Filter) || e.Email.Contains(input.Filter) || e.Age.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.EnglishNameFilter), e => e.EnglishName == input.EnglishNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.ArabicNameFilter), e => e.ArabicName == input.ArabicNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.FacebookUriFilter), e => e.FacebookUri == input.FacebookUriFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneFilter), e => e.Phone == input.PhoneFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email == input.EmailFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.AgeFilter), e => e.Age == input.AgeFilter)
//                        .WhereIf(input.MinCreationDateFilter != null, e => e.CreationDate >= input.MinCreationDateFilter)
//                        .WhereIf(input.MaxCreationDateFilter != null, e => e.CreationDate <= input.MaxCreationDateFilter);

//            var query = (from o in filteredTerritories
//                         select new GetTerritoryForViewDto()
//                         {
//                             Territory = new TerritoryDto
//                             {
//                                 UserName = o.UserName,
//                                 EnglishName = o.EnglishName,
//                                 ArabicName = o.ArabicName,
//                                 FacebookUri = o.FacebookUri,
//                                 Phone = o.Phone,
//                                 Email = o.Email,
//                                 Age = o.Age,
//                                 CreationDate = o.CreationDate,
//                                 Id = o.Id
//                             }
//                         });

//            var territoryListDtos = await query.ToListAsync();

//            return _territoriesExcelExporter.ExportToFile(territoryListDtos);
//        }

//        private void UpdateSalesUserCreate(SalesUserCreate.SalesUserCreate  salesUserCreate)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//                try
//                {

//                    using (SqlCommand command = connection.CreateCommand())
//                    {

//                        command.CommandText = "UPDATE SalesUserCreate SET TenantId = @TenantId ,UserId =@UserId , UserName = @UserName , TotalCreate = @TotalCreate ,IsActiveButton = @IsActiveButton  Where Id = @Id";

//                        command.Parameters.AddWithValue("@Id", salesUserCreate.Id);
//                        command.Parameters.AddWithValue("@TenantId", salesUserCreate.TenantId);
//                        command.Parameters.AddWithValue("@UserId", salesUserCreate.UserId);
//                        command.Parameters.AddWithValue("@UserName", salesUserCreate.UserName);
//                        command.Parameters.AddWithValue("@TotalCreate", salesUserCreate.TotalCreate);
//                        command.Parameters.AddWithValue("@IsActiveButton", salesUserCreate.IsActiveButton);

//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }
//                }
//                catch (Exception)
//                {


//                }

//        }

//    }
//}