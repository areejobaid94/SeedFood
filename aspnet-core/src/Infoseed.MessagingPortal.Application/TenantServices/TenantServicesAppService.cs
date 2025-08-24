using Infoseed.MessagingPortal.InfoSeedServices;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.TenantServices.Exporting;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Framework.Data;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infoseed.MessagingPortal.TenantServices
{
    [AbpAuthorize(AppPermissions.Pages_Administration_TenantServices)]
    public class TenantServicesAppService : MessagingPortalAppServiceBase, ITenantServicesAppService
    {
 
        private readonly IRepository<TenantService> _tenantServiceRepository;
        private readonly ITenantServicesExcelExporter _tenantServicesExcelExporter;
        private readonly IRepository<InfoSeedService, int> _lookup_infoSeedServiceRepository;
        private IConfiguration configuration;


        public TenantServicesAppService(IRepository<TenantService> tenantServiceRepository
            , ITenantServicesExcelExporter tenantServicesExcelExporter
            , IRepository<InfoSeedService, int> lookup_infoSeedServiceRepository
            , IConfiguration configuration)
        {
            _tenantServiceRepository = tenantServiceRepository;
            _tenantServicesExcelExporter = tenantServicesExcelExporter;
            _lookup_infoSeedServiceRepository = lookup_infoSeedServiceRepository;
            this.configuration = configuration;
        }

        public async Task<PagedResultDto<GetTenantServiceForViewDto>> GetAll(GetAllTenantServicesInput input)
        {

            var filteredTenantServices = _tenantServiceRepository.GetAll()
                        .Include(e => e.InfoSeedServiceFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinServiceFeesFilter != null, e => e.ServiceFees >= input.MinServiceFeesFilter)
                        .WhereIf(input.MaxServiceFeesFilter != null, e => e.ServiceFees <= input.MaxServiceFeesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InfoSeedServiceServiceNameFilter), e => e.InfoSeedServiceFk != null && e.InfoSeedServiceFk.ServiceName == input.InfoSeedServiceServiceNameFilter);

            var pagedAndFilteredTenantServices = filteredTenantServices
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tenantServices = from o in pagedAndFilteredTenantServices
                                 join o1 in _lookup_infoSeedServiceRepository.GetAll() on o.InfoSeedServiceId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 select new GetTenantServiceForViewDto()
                                 {
                                     TenantService = new TenantServiceDto
                                     {
                                         ServiceFees = o.ServiceFees,
                                          FeesForFirstOrder=o.FeesForFirstOrder,
                                           FirstNumberOfOrders=o.FirstNumberOfOrders,
                                         
                                         Id = o.Id
                                     },
                                     InfoSeedServiceServiceName = s1 == null || s1.ServiceName == null ? "" : s1.ServiceName.ToString()
                                 };

            var totalCount = await filteredTenantServices.CountAsync();

            return new PagedResultDto<GetTenantServiceForViewDto>(
                totalCount,
                await tenantServices.ToListAsync()
            );
        }

        public async Task<GetTenantServiceForViewDto> GetTenantServiceForView(int id)
        {
            var tenantService = await _tenantServiceRepository.GetAsync(id);

            var output = new GetTenantServiceForViewDto { TenantService = ObjectMapper.Map<TenantServiceDto>(tenantService) };

            if (output.TenantService.InfoSeedServiceId != null)
            {
                var _lookupInfoSeedService = await _lookup_infoSeedServiceRepository.FirstOrDefaultAsync((int)output.TenantService.InfoSeedServiceId);
                output.InfoSeedServiceServiceName = _lookupInfoSeedService?.ServiceName?.ToString();
                output.ServiceName = _lookupInfoSeedService?.ServiceName?.ToString();
                output.ServiceFees = _lookupInfoSeedService?.ServiceFees.ToString();

                output.ServiceCreationDate = _lookupInfoSeedService?.ServiceCreationDate.ToString();
                output.ServiceStoppingDate = _lookupInfoSeedService?.ServiceStoppingDate.ToString();
                output.Remarks = _lookupInfoSeedService?.Remarks.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TenantServices_Edit)]
        public async Task<GetTenantServiceForEditOutput> GetTenantServiceForEdit(EntityDto input)
        {
            var tenantService = await _tenantServiceRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTenantServiceForEditOutput { TenantService = ObjectMapper.Map<CreateOrEditTenantServiceDto>(tenantService) };

            if (output.TenantService.InfoSeedServiceId != null)
            {
                var _lookupInfoSeedService = await _lookup_infoSeedServiceRepository.FirstOrDefaultAsync((int)output.TenantService.InfoSeedServiceId);
                output.InfoSeedServiceServiceName = _lookupInfoSeedService?.ServiceName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTenantServiceDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_TenantServices_Create)]
        protected virtual async Task Create(CreateOrEditTenantServiceDto input)
        {
            var tenantService = ObjectMapper.Map<TenantService>(input);

            if (AbpSession.TenantId != null)
            {
                tenantService.TenantId = (int?)AbpSession.TenantId;
            }

            await _tenantServiceRepository.InsertAsync(tenantService);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TenantServices_Edit)]
        protected virtual async Task Update(CreateOrEditTenantServiceDto input)
        {
            var tenantService = await _tenantServiceRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, tenantService);
        }




        [AbpAuthorize(AppPermissions.Pages_Administration_TenantServices_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _tenantServiceRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetTenantServicesToExcel(GetAllTenantServicesForExcelInput input)
        {

            var filteredTenantServices = _tenantServiceRepository.GetAll()
                        .Include(e => e.InfoSeedServiceFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinServiceFeesFilter != null, e => e.ServiceFees >= input.MinServiceFeesFilter)
                        .WhereIf(input.MaxServiceFeesFilter != null, e => e.ServiceFees <= input.MaxServiceFeesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InfoSeedServiceServiceNameFilter), e => e.InfoSeedServiceFk != null && e.InfoSeedServiceFk.ServiceName == input.InfoSeedServiceServiceNameFilter);

            var query = (from o in filteredTenantServices
                         join o1 in _lookup_infoSeedServiceRepository.GetAll() on o.InfoSeedServiceId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetTenantServiceForViewDto()
                         {
                             TenantService = new TenantServiceDto
                             {
                                 ServiceFees = o.ServiceFees,
                                 FeesForFirstOrder = o.FeesForFirstOrder,
                                 FirstNumberOfOrders = o.FirstNumberOfOrders,
                                 Id = o.Id
                             },
                             InfoSeedServiceServiceName = s1 == null || s1.ServiceName == null ? "" : s1.ServiceName.ToString()
                         });

            var tenantServiceListDtos = await query.ToListAsync();

            return _tenantServicesExcelExporter.ExportToFile(tenantServiceListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TenantServices)]
        public async Task<PagedResultDto<TenantServiceInfoSeedServiceLookupTableDto>> GetAllInfoSeedServiceForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_infoSeedServiceRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ServiceName != null && e.ServiceName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var infoSeedServiceList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<TenantServiceInfoSeedServiceLookupTableDto>();
            foreach (var infoSeedService in infoSeedServiceList)
            {
                lookupTableDtoList.Add(new TenantServiceInfoSeedServiceLookupTableDto
                {
                    Id = infoSeedService.Id,
                    DisplayName = infoSeedService.ServiceName?.ToString()
                });
            }

            return new PagedResultDto<TenantServiceInfoSeedServiceLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    
        public IList<TenantServiceModalDto> GetTenatServices(int tenantIdentifer)
        {
            var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@TenantId",tenantIdentifer)
                     };

            IList<TenantServiceModalDto> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.TenantServicesGet",
                       sqlParameters.ToArray(),
                       MapTenantService,
                       configuration.GetConnectionString("Default"));

            return result;
        }


        public async Task<bool> UpdateTenantServiceAsync(List<TenantServiceModalDto> services)
        {

            if (services.Count <= 0)
                return false;

            var allTenantS= GetTenantService().Result;

            var TenantS = allTenantS.Where(x => x.TenantId == services[0].TenantId).ToList();

            foreach (var item in services)
            {
                var service = TenantS.Where(x => x.InfoSeedServiceId == item.ServiceId).FirstOrDefault();
                if (service != null)
                {
                    if (item.IsSelected)
                    {


                        TenantService tenantService = new TenantService
                        {
                            InfoSeedServiceId = item.ServiceId,
                            ServiceFees = item.Fees,
                             FirstNumberOfOrders=item.FirstNumberOfOrders,
                              FeesForFirstOrder= item.FeesForFirstOrder,
                            TenantId = item.TenantId,
                            Id= service.Id
                        };

                        UpdateTenantService(tenantService);

                    }
                    else
                    {
                        DeleteTenantService(service.Id);
                    }

                       

                }
                else
                {
                    if (item.IsSelected)
                    {
                        TenantService tenantService = new TenantService
                        {
                             InfoSeedServiceId= item.ServiceId,
                              ServiceFees= item.Fees,
                            FirstNumberOfOrders = item.FirstNumberOfOrders,
                            FeesForFirstOrder = item.FeesForFirstOrder,
                            TenantId = item.TenantId,
                                
                        };

                        CreateTenantService(tenantService);
                        

                    }
                       
                }

            }


            //try
            //{
           

                //var sqlParameters = new List<SqlParameter> {
                //            new SqlParameter("@TenantId",services[0].TenantId),
                //            //new SqlParameter("@feesTotal",string.Join(',', dtos.Select(c=>c.Fees))),
                //                                 new SqlParameter("@Services",string.Join(',', services.Select(c=>c.ServiceId)))
                //     };

                //SqlDataHelper.ExecuteNoneQuery("dbo.TenantServicesUpdate", sqlParameters.ToArray());

            //}
            //catch (SqlException sqlex)
            //{
            //    throw new DataProviderException("SQL ERROR WHILE  EXECUTING dbo.TenantServicesUpdate", sqlex);
            //}
            //catch (Exception ex)
            //{
            //    throw new DataProviderException("UNEXPECTED EXCEPTION WHILE EXECUTING dbo.TenantServicesUpdate", ex);
            //}

            return true;
        }

        private static TenantServiceModalDto MapTenantService(IDataReader dataReader)
        {
            TenantServiceModalDto catalogue = new TenantServiceModalDto
            {
                 IsFeesPerTransaction = SqlDataHelper.GetValue<bool>(dataReader, "IsFeesPerTransaction"),
                TenantId = SqlDataHelper.GetValue<int?>(dataReader, "TenantId"),
                ServiceId = SqlDataHelper.GetValue<int>(dataReader, "ServiceId"),
                 FeesForFirstOrder = SqlDataHelper.GetValue<decimal>(dataReader, "FeesForFirstOrder"),
                  FirstNumberOfOrders = SqlDataHelper.GetValue<int>(dataReader, "FirstNumberOfOrders"),
                ServiceName = SqlDataHelper.GetValue<string>(dataReader, "ServiceName"),
                Fees = SqlDataHelper.GetValue<Decimal>(dataReader, "Fees"),
                TenantServiceCreationTime = SqlDataHelper.GetValue<DateTime?>(dataReader, "TenantServiceCreationTime"),
                IsSelected = false//SqlDataHelper.GetValue<bool>(dataReader, "IsSelected"),
            };
            return catalogue;
        }
        private void DeleteTenantService(int id)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM TenantServices Where Id = @Id";

                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        private void UpdateTenantService(TenantService  tenantService)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "UPDATE TenantServices SET TenantId = @Tid, ServiceFees = @Fees , InfoSeedServiceId = @SerId , FeesForFirstOrder=@FeesForFirstOrder ,FirstNumberOfOrders =@FirstNumberOfOrders   Where Id = @Id";

                command.Parameters.AddWithValue("@Id", tenantService.Id);
                command.Parameters.AddWithValue("@Tid", tenantService.TenantId);
                command.Parameters.AddWithValue("@Fees", tenantService.ServiceFees);

                command.Parameters.AddWithValue("@FirstNumberOfOrders", tenantService.FirstNumberOfOrders);
                command.Parameters.AddWithValue("@FeesForFirstOrder", tenantService.FeesForFirstOrder);
                command.Parameters.AddWithValue("@SerId", tenantService.InfoSeedServiceId);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void CreateTenantService(TenantService tenantService)
        {
            DateTime time = DateTime.Now;              // Use current time
            string format = "yyyy-MM-dd HH:mm:ss";

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "INSERT INTO TenantServices (CreationTime,IsDeleted,TenantId,ServiceFees,InfoSeedServiceId,FeesForFirstOrder,FirstNumberOfOrders) VALUES (@cre,@isD,@Tid, @Fees, @SerId,@FeesForFirstOrder,@FirstNumberOfOrders)"; ;

                command.Parameters.AddWithValue("@cre", time.ToString(format));
                command.Parameters.AddWithValue("@isD", false);
                command.Parameters.AddWithValue("@Tid", tenantService.TenantId);
                command.Parameters.AddWithValue("@FirstNumberOfOrders", tenantService.FirstNumberOfOrders);
                command.Parameters.AddWithValue("@FeesForFirstOrder", tenantService.FeesForFirstOrder);
                command.Parameters.AddWithValue("@Fees", tenantService.ServiceFees);
                command.Parameters.AddWithValue("@SerId", tenantService.InfoSeedServiceId);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public async Task<List<TenantService>> GetTenantService()
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[TenantServices]";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<TenantService> tenantService = new List<TenantService>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                tenantService.Add(new TenantService
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                     ServiceFees = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ServiceFees"]),
                      FirstNumberOfOrders = Convert.ToInt32(dataSet.Tables[0].Rows[i]["FirstNumberOfOrders"]),
                       FeesForFirstOrder= Convert.ToDecimal(dataSet.Tables[0].Rows[i]["FeesForFirstOrder"]),
                      InfoSeedServiceId= Convert.ToInt32(dataSet.Tables[0].Rows[i]["InfoSeedServiceId"])
                      
                });
            }

            conn.Close();
            da.Dispose();
            return tenantService;
        }

    }
}