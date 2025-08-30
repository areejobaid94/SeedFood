using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Framework.Data;
using Framework.Data.Sql;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Areas.Exporting;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Areas
{
    //[AbpAuthorize(AppPermissions.Pages_Areas)]
    public class AreasAppService : MessagingPortalAppServiceBase, IAreasAppService
    {
        private readonly IRepository<Area, long> _areaRepository;
        private readonly IAreasExcelExporter _areasExcelExporter;
        private readonly ICacheManager _cacheManager;
        private readonly string _postgresConnection;

        public AreasAppService(IRepository<Area, long> areaRepository, IAreasExcelExporter areasExcelExporter, ICacheManager cacheManager, ICampaginExcelExporter campaginExcelExporter,
            IConfiguration configuration)
        {
            _areaRepository = areaRepository;
            _areasExcelExporter = areasExcelExporter;
            _cacheManager = cacheManager;
            _postgresConnection = configuration.GetConnectionString("postgres");
        }

        public  AreasAppService()
        {

        }
        public async Task<PagedResultDto<GetAreaForViewDto>> GetAll(GetAllAreasInput input)
        {
            try
            {

                var Areas = _areaRepository.GetAll().ToList();

                var query = Areas.AsQueryable();

                var pagedAndFilteredOrders = query
                    .OrderBy(input.Sorting ?? "id asc");

                List<GetAreaForViewDto> AreaList = new List<GetAreaForViewDto>();

                foreach (var area in pagedAndFilteredOrders.ToArray())
                {
                    // var user = GetUser(AbpSession.TenantId, area.UserId);


                    AreaList.Add(new GetAreaForViewDto
                    {

                        Area = new AreaDto
                        {

                            AreaNameEnglish = area.AreaNameEnglish,
                            AreaCoordinateEnglish = area.AreaCoordinateEnglish,
                            UserId = area.UserId,
                            BranchID = area.BranchID,
                            AreaName = area.AreaName,
                            AreaCoordinate = area.AreaCoordinate,
                            Id = area.Id,
                            RestaurantsType = area.RestaurantsType,
                            IsAssginToAllUser = area.IsAssginToAllUser,
                            IsAvailableBranch = area.IsAvailableBranch,
                            Latitude = area.Latitude,
                            Longitude = area.Longitude,



                        },
                        IsAvailableBranch = area.IsAvailableBranch,
                        IsRestaurantsTypeAll = area.IsRestaurantsTypeAll
                        //  Surname = user.Name,
                        //UserName = user.Surname,
                        // RestaurantsName = GetRType(area.TenantId).Where(x => x.Id == area.RestaurantsType).FirstOrDefault().Name



                    }); ;

                }


                var totalCount = AreaList.Count();

                return new PagedResultDto<GetAreaForViewDto>(
                    totalCount,
                     AreaList.ToArray()
                );

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<GetAreaForViewDto> GetAreaForView(long id)
        {
            try
            {
                var area = await _areaRepository.GetAsync(id);

                var output = new GetAreaForViewDto { Area = ObjectMapper.Map<AreaDto>(area) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

       // [AbpAuthorize(AppPermissions.Pages_Areas_Edit)]
        public async Task<GetAreaForEditOutput> GetAreaForEdit(EntityDto<long> input)
        {
            try
            {
                var area = await _areaRepository.FirstOrDefaultAsync(input.Id);

                var output = new GetAreaForEditOutput { Area = ObjectMapper.Map<CreateOrEditAreaDto>(area) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task CreateOrEdit(CreateOrEditAreaDto input)
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
        protected virtual async Task<long> Create(CreateOrEditAreaDto input)
        {
            try
            {
                var area = ObjectMapper.Map<Area>(input);

                // Set default values
                area.UserId = 0;
                area.AreaCoordinate = "";
                area.AreaCoordinateEnglish = "";
                area.SettingJson = "";
                if (AbpSession.TenantId != null)
                {
                    area.TenantId = (int?)AbpSession.TenantId;
                }

                _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());

                // Prepare parameters
                var npgsqlParams = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("p_tenantid", area.TenantId ?? (object)DBNull.Value),
                    new NpgsqlParameter("p_areaname", area.AreaName),
                    new NpgsqlParameter("p_areacoordinate", area.AreaCoordinate ?? ""),
                    new NpgsqlParameter("p_branchid", area.BranchID ?? ""),
                    new NpgsqlParameter("p_userid", area.UserId ?? 0),
                    new NpgsqlParameter("p_isassgintoalluser", area.IsAssginToAllUser),
                    new NpgsqlParameter("p_isavailablebranch", area.IsAvailableBranch),
                    new NpgsqlParameter("p_restaurantstype", area.RestaurantsType),
                    new NpgsqlParameter("p_areanameenglish", area.AreaNameEnglish ?? ""),
                    new NpgsqlParameter("p_areacoordinateenglish", area.AreaCoordinateEnglish ?? ""),
                    new NpgsqlParameter("p_isrestaurantstypeall", area.IsRestaurantsTypeAll),
                    new NpgsqlParameter("p_latitude", area.Latitude ?? (object)DBNull.Value),
                    new NpgsqlParameter("p_longitude", area.Longitude ?? (object)DBNull.Value),
                    new NpgsqlParameter("p_settingjson", area.SettingJson ?? ""),
                    new NpgsqlParameter("p_userids", area.UserIds ?? "")
                };

                // Execute PostgreSQL function
                var newId = PostgresDataHelper.ExecuteFunction<long>(
                    "dbo.areas_add",
                    npgsqlParams,
                    reader => reader.GetInt64(0),
                    _postgresConnection
                ).FirstOrDefault();

                return newId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create area: " + ex.Message);
                throw;
            }
        }


        protected virtual async Task Update(CreateOrEditAreaDto input)
        {
            try
            {
                // Clear the cache
                _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());

                // Fetch the existing area from DB
                var area = getAreasById((int)input.Id, (int)AbpSession.TenantId);

                if (area == null)
                {
                    throw new Exception("Area not found.");
                }

                // Only update the fields that exist in the input DTO
                if (!string.IsNullOrEmpty(input.AreaName))
                    area.AreaName = input.AreaName;

                if (!string.IsNullOrEmpty(input.AreaCoordinate))
                    area.AreaCoordinate = input.AreaCoordinate;

                if (!string.IsNullOrEmpty(input.AreaNameEnglish))
                    area.AreaNameEnglish = input.AreaNameEnglish;

                if (!string.IsNullOrEmpty(input.AreaCoordinateEnglish))
                    area.AreaCoordinateEnglish = input.AreaCoordinateEnglish;

                if (input.UserId.HasValue)
                    area.UserId = input.UserId.Value;

                area.RestaurantsType = input.RestaurantsType;
                area.IsAssginToAllUser = input.IsAssginToAllUser;
                area.IsAvailableBranch = input.IsAvailableBranch;
                area.IsRestaurantsTypeAll = input.IsRestaurantsTypeAll;

                if (input.Latitude.HasValue)
                    area.Latitude = input.Latitude;

                if (input.Longitude.HasValue)
                    area.Longitude = input.Longitude;

                if (!string.IsNullOrEmpty(input.UserIds))
                    area.UserIds = input.UserIds;

                if (!string.IsNullOrEmpty(input.BranchID))
                    area.BranchID = input.BranchID;

                var npgsqlParams = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("p_id", area.Id),
                    new NpgsqlParameter("p_areaname", area.AreaName),
                    new NpgsqlParameter("p_areacoordinate", area.AreaCoordinate ?? ""),
                    new NpgsqlParameter("p_branchid", area.BranchID ?? ""),
                    new NpgsqlParameter("p_userid", area.UserId ?? 0),
                    new NpgsqlParameter("p_isassgintoalluser", area.IsAssginToAllUser),
                    new NpgsqlParameter("p_isavailablebranch", area.IsAvailableBranch),
                    new NpgsqlParameter("p_restaurantstype", area.RestaurantsType),
                    new NpgsqlParameter("p_areanameenglish", area.AreaNameEnglish ?? ""),
                    new NpgsqlParameter("p_areacoordinateenglish", area.AreaCoordinateEnglish ?? ""),
                    new NpgsqlParameter("p_isrestaurantstypeall", area.IsRestaurantsTypeAll),
                    new NpgsqlParameter("p_latitude", area.Latitude ?? (object)DBNull.Value),
                    new NpgsqlParameter("p_longitude", area.Longitude ?? (object)DBNull.Value),
                    new NpgsqlParameter("p_settingjson", area.SettingJson ?? ""),
                    new NpgsqlParameter("p_userids", area.UserIds ?? "")

            };

                // Call PostgreSQL function to update
                await Task.Run(() =>
                {
                    PostgresDataHelper.ExecuteFunction<long>(
                        "dbo.areas_update",  // You need to create this function in PSQL
                        npgsqlParams,
                        reader => reader.GetInt64(0), // Return updated area Id
                        _postgresConnection
                    );
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //[AbpAuthorize(AppPermissions.Pages_Areas_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());
            await _areaRepository.DeleteAsync(input.Id);
        }

        // Available Branches
        public List<AreaDto> GetAvailableAreas(int tenantId)
        {
            return getAllAreas(tenantId, true);
        }
        // Without Pagination
        public List<AreaDto> GetAllAreas(int tenantID, bool? isAvailableBranch = null)
        {
            return getAllAreas(tenantID, isAvailableBranch);
        }
        // Pagination
        public AreasEntity GetAreas(int pageNumber = 0, int pageSize = 50)
        {
            return getAreas(pageNumber, pageSize);
        }
        public AreaDto GetAreaById(int id ,int tenantID)
        {
            return getAreasById(id,tenantID);
        }
        public string DeleteArea(long areaId)
        {
            _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());

            return deleteArea(areaId);
        }
        public async Task<FileDto> GetAreasToExcel(GetAllAreasForExcelInput input)
        {
            try
            {
                var filteredAreas = _areaRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.AreaName.Contains(input.Filter) || e.AreaCoordinate.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AreaNameFilter), e => e.AreaName == input.AreaNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AreaCoordinateFilter), e => e.AreaCoordinate == input.AreaCoordinateFilter);

                var query = (from o in filteredAreas
                             select new GetAreaForViewDto()
                             {
                                 Area = new AreaDto
                                 {
                                     BranchID = o.BranchID,
                                     AreaName = o.AreaName,
                                     AreaCoordinate = o.AreaCoordinate,
                                     Id = o.Id
                                 }
                             });


                var areaListDtos = await query.ToListAsync();

                return _areasExcelExporter.ExportToFile(areaListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public WorkModel GetMenuSetting(long BranchId)
        {
            return getBranchSetting(BranchId);
        }
        public void SaveSetting(long BranchId, WorkModel workModel)
        {
            try
            {
                workModel.StartDateFri = checkValidValue(workModel.StartDateFri);
                workModel.StartDateSat = checkValidValue(workModel.StartDateSat);
                workModel.StartDateSun = checkValidValue(workModel.StartDateSun);
                workModel.StartDateMon = checkValidValue(workModel.StartDateMon);
                workModel.StartDateTues = checkValidValue(workModel.StartDateTues);
                workModel.StartDateWed = checkValidValue(workModel.StartDateWed);
                workModel.StartDateThurs = checkValidValue(workModel.StartDateThurs);

                workModel.EndDateFri = checkValidValue(workModel.EndDateFri);
                workModel.EndDateSat = checkValidValue(workModel.EndDateSat);
                workModel.EndDateSun = checkValidValue(workModel.EndDateSun);
                workModel.EndDateMon = checkValidValue(workModel.EndDateMon);
                workModel.EndDateTues = checkValidValue(workModel.EndDateTues);
                workModel.EndDateWed = checkValidValue(workModel.EndDateWed);
                workModel.EndDateThurs = checkValidValue(workModel.EndDateThurs);


                workModel.StartDateFriSP = checkValidValue(workModel.StartDateFriSP);
                workModel.StartDateSatSP = checkValidValue(workModel.StartDateSatSP);
                workModel.StartDateSunSP = checkValidValue(workModel.StartDateSunSP);
                workModel.StartDateMonSP = checkValidValue(workModel.StartDateMonSP);
                workModel.StartDateTuesSP = checkValidValue(workModel.StartDateTuesSP);
                workModel.StartDateWedSP = checkValidValue(workModel.StartDateWedSP);
                workModel.StartDateThursSP = checkValidValue(workModel.StartDateThursSP);

                workModel.EndDateFriSP = checkValidValue(workModel.EndDateFriSP);
                workModel.EndDateSatSP = checkValidValue(workModel.EndDateSatSP);
                workModel.EndDateSunSP = checkValidValue(workModel.EndDateSunSP);
                workModel.EndDateMonSP = checkValidValue(workModel.EndDateMonSP);
                workModel.EndDateTuesSP = checkValidValue(workModel.EndDateTuesSP);
                workModel.EndDateWedSP = checkValidValue(workModel.EndDateWedSP);
                workModel.EndDateThursSP = checkValidValue(workModel.EndDateThursSP);

                BranchSettingUpdate(BranchId, JsonConvert.SerializeObject(workModel, Formatting.Indented));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public LocationInfoModelDto GetLocationDeliveryCost(int tenantID, string JsonLocation)
        {
            return LocationDeliveryCost(tenantID, JsonLocation);
        }



        #region Private Methods

        private WorkModel getBranchSetting(long branchId)
        {
            try
            {
                var spName = "dbo.branch_setting_get";
                var sqlParameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("p_branchid", branchId)
                };

                var entity = PostgresDataHelper.ExecuteFunction(
                    spName,
                    sqlParameters.ToArray(),
                    DataReaderMapper.MapBranchSettingPSQL,
                    _postgresConnection
                ).FirstOrDefault();

                if (entity == null)
                    return null;

                // Lists of property names for StartDate and EndDate
                var days = new[] { "Fri", "Sat", "Sun", "Mon", "Tues", "Wed", "Thurs" };
                foreach (var day in days)
                {
                    // Update regular dates
                    var startProp = entity.GetType().GetProperty($"StartDate{day}");
                    var endProp = entity.GetType().GetProperty($"EndDate{day}");
                    if (startProp != null) startProp.SetValue(entity, getValidValue(startProp.GetValue(entity)));
                    if (endProp != null) endProp.SetValue(entity, getValidValue(endProp.GetValue(entity)));

                    // Update SP dates
                    var startSPProp = entity.GetType().GetProperty($"StartDate{day}SP");
                    var endSPProp = entity.GetType().GetProperty($"EndDate{day}SP");
                    if (startSPProp != null) startSPProp.SetValue(entity, getValidValue(startSPProp.GetValue(entity)));
                    if (endSPProp != null) endSPProp.SetValue(entity, getValidValue(endSPProp.GetValue(entity)));
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw; // preserve stack trace
            }
        }

        private LocationInfoModelDto LocationDeliveryCost(int tenantID, string JsonLocation)
        {
            try
            {
                LocationInfoModelDto location = new LocationInfoModelDto();
                var SP_Name = Constants.Location.SP_LocationDeliveryCostGet;

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantId",tenantID)
                    ,new SqlParameter("@JsonLocation",JsonLocation)
                };

                location = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMenuLocation, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return location;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private AreasEntity getAreas(int pageNumber = 0, int pageSize = 50)
        {
            try
            {
                AreasEntity areas = new AreasEntity();

                var npgsqlParams = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("p_tenantid", AbpSession.TenantId.Value),
                    new NpgsqlParameter("p_pagenumber", pageNumber * pageSize), // OFFSET calculation
                    new NpgsqlParameter("p_pagesize", pageSize)
                };

                var results = PostgresDataHelper.ExecuteFunction(
                    "dbo.areas_get",
                    npgsqlParams,
                    DataReaderMapper.MapAreaPSQL,
                    _postgresConnection
                ).ToList();

                areas.TotalCount = 0;
                if (results.Any())
                {
                    areas.lstAreas = results;
                    areas.TotalCount = (int)results.First().TotalCount;
                }
                else
                {
                    areas.lstAreas = new List<AreaDto>();
                    areas.TotalCount = 0;
                }

                return areas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private List<AreaDto> getAllAreas(int tenantID, bool? isAvailableBranch = null)
        {
            try
            {
                List<AreaDto> lstAreaDto = new List<AreaDto>();
                var SP_Name = Constants.Area.SP_AreasByTenantIdGet;
                var sqlParameters = new List<SqlParameter> {
                      new SqlParameter("@TenantId",tenantID)
                     ,new SqlParameter("@IsAvailableBranch",isAvailableBranch)

                };

                lstAreaDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapArea, AppSettingsModel.ConnectionStrings).ToList();

                return lstAreaDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private AreaDto getAreasById(int id, int tenantID)
        {
            try
            {
                var npgsqlParams = new NpgsqlParameter[]
                {
            new NpgsqlParameter("p_tenantid", tenantID),
            new NpgsqlParameter("p_id", id)
                };

                // Execute PostgreSQL function and map results
                var area = PostgresDataHelper.ExecuteFunction(
                    "dbo.areas_byid_get",
                    npgsqlParams,
                    DataReaderMapper.MapAreaPSQL, // create this mapper to map IDataReader to AreaDto including URL
                    _postgresConnection
                ).FirstOrDefault();

                return area ?? new AreaDto();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string deleteArea(long areaId)
        {
            try
            {
                // Clear cache
                _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());

                // Prepare PostgreSQL parameter
                var npgsqlParams = new NpgsqlParameter[]
                {
                   new NpgsqlParameter("p_areaid", areaId)
                };

                // Execute the function and get the result
                int result = PostgresDataHelper.ExecuteFunction<int>(
                    "dbo.area_delete",
                    npgsqlParams,
                    reader => reader.GetInt32(0),  // Function returns an int
                    _postgresConnection
                ).FirstOrDefault();

                // Map result to string
                return result switch
                {
                    1 => "MENU",
                    2 => "ITEM",
                    3 => "ORDER",
                    4 => "DELIVERYCOST",
                    5 => "LOCATION",
                    _ => "DELETED"
                };
            }
            catch (Exception ex)
            {
                throw; // Keep the exception as-is
            }
        }


        private string checkValidValue(dynamic value)
        {
            string result = null;
            try
            {

                DateTime dateTime = DateTime.Parse(value.ToString());
                dateTime = dateTime.AddHours(AppSettingsModel.AddHour);
                result = dateTime.ToString("HH:mm");
                return result;

            }
            catch (Exception )
            {
                // return null if get unexpected value 
                return result;
            }
        }
        private string getValidValue(dynamic value)
        {
            string result = null;
            try
            {
                result = value.ToString();
                return result;

            }
            catch (Exception )
            {
                // return null if get unexpected value 

                return result;
            }


        }
        private void BranchSettingUpdate(long branchId, string branchSettingJson)
        {
            try
            {
                var funcName = "dbo.branch_setting_update";

                var npgsqlParams = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("p_branchid", branchId),
                    new NpgsqlParameter("p_settingjson", branchSettingJson ?? "")
                };

                PostgresDataHelper.ExecuteFunction<object>(
                    funcName,
                    npgsqlParams,
                    reader => null, // function returns void, so nothing to read
                    _postgresConnection
                );
            }
            catch (Exception ex)
            {
                throw; // keep stack trace clean
            }
        }

        #endregion

    }
}