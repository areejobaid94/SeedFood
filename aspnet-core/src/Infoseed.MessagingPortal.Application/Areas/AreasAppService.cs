using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Framework.Data;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Areas.Exporting;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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


        public AreasAppService(IRepository<Area, long> areaRepository, IAreasExcelExporter areasExcelExporter, ICacheManager cacheManager)
        {
            _areaRepository = areaRepository;
            _areasExcelExporter = areasExcelExporter;
            _cacheManager = cacheManager;
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
        protected virtual async Task Create(CreateOrEditAreaDto input)
        {
            try
            {
                var area = ObjectMapper.Map<Area>(input);

                area.UserId = 0;
                area.AreaCoordinate = "";
                area.AreaCoordinateEnglish = "";
                area.SettingJson = "";
                // area.AreaName=area.AreaName.Substring(0, 20);
                // area.AreaNameEnglish=area.AreaNameEnglish.Substring(0, 20);
                if (AbpSession.TenantId != null)
                {
                    area.TenantId = (int?)AbpSession.TenantId;
                }
                _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());


                await _areaRepository.InsertAsync(area);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        //[AbpAuthorize(AppPermissions.Pages_Areas_Edit)]
        protected virtual async Task Update(CreateOrEditAreaDto input)
        {
            try
            {
                _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());

                var area = await _areaRepository.FirstOrDefaultAsync((long)input.Id);
                ObjectMapper.Map(input, area);
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
        
        private WorkModel getBranchSetting(long BranchId)
        {
            try
            {
                WorkModel entity = new WorkModel();
                var SP_Name = Constants.Area.SP_BranchSettingGet;

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@BranchId",BranchId)
                };

                entity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBranchSetting, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                if (entity != null)
                {
                    entity.StartDateFri = getValidValue(entity.StartDateFri);
                    entity.StartDateSat = getValidValue(entity.StartDateSat);
                    entity.StartDateSun = getValidValue(entity.StartDateSun);
                    entity.StartDateMon = getValidValue(entity.StartDateMon);
                    entity.StartDateTues = getValidValue(entity.StartDateTues);
                    entity.StartDateWed = getValidValue(entity.StartDateWed);
                    entity.StartDateThurs = getValidValue(entity.StartDateThurs);



                    entity.EndDateFri = getValidValue(entity.EndDateFri);
                    entity.EndDateSat = getValidValue(entity.EndDateSat);
                    entity.EndDateSun = getValidValue(entity.EndDateSun);
                    entity.EndDateMon = getValidValue(entity.EndDateMon);
                    entity.EndDateTues = getValidValue(entity.EndDateTues);
                    entity.EndDateWed = getValidValue(entity.EndDateWed);
                    entity.EndDateThurs = getValidValue(entity.EndDateThurs);



                    entity.StartDateFriSP = getValidValue(entity.StartDateFriSP);
                    entity.StartDateSatSP = getValidValue(entity.StartDateSatSP);
                    entity.StartDateSunSP = getValidValue(entity.StartDateSunSP);
                    entity.StartDateMonSP = getValidValue(entity.StartDateMonSP);
                    entity.StartDateTuesSP = getValidValue(entity.StartDateTuesSP);
                    entity.StartDateWedSP = getValidValue(entity.StartDateWedSP);
                    entity.StartDateThursSP = getValidValue(entity.StartDateThursSP);



                    entity.EndDateFriSP = getValidValue(entity.EndDateFriSP);
                    entity.EndDateSatSP = getValidValue(entity.EndDateSatSP);
                    entity.EndDateSunSP = getValidValue(entity.EndDateSunSP);
                    entity.EndDateMonSP = getValidValue(entity.EndDateMonSP);
                    entity.EndDateTuesSP = getValidValue(entity.EndDateTuesSP);
                    entity.EndDateWedSP = getValidValue(entity.EndDateWedSP);
                    entity.EndDateThursSP = getValidValue(entity.EndDateThursSP);
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
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
                var SP_Name = Constants.Area.SP_AreasGet;
                var sqlParameters = new List<SqlParameter> {
                      new SqlParameter("@TenantId",AbpSession.TenantId.Value)
                     ,new SqlParameter("@PageNumber",pageNumber)
                     ,new SqlParameter("@PageSize",pageSize)

                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                areas.lstAreas = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapArea, AppSettingsModel.ConnectionStrings).ToList();
                areas.TotalCount = (int)OutputParameter.Value;
                return areas;
            }
            catch (Exception)
            {

                throw;
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
        private AreaDto getAreasById(int Id, int tenantID)
        {
            try
            {
                AreaDto lstAreaDto = new AreaDto();

                var SP_Name = Constants.Area.SP_AreasByIdGet;
                var sqlParameters = new List<SqlParameter> {
                      new SqlParameter("@TenantId",tenantID)
                     ,new SqlParameter("@Id",Id)
                };
                lstAreaDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapArea, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return lstAreaDto;
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
                _cacheManager.GetCache("CacheTenant_Areas").Remove("Area_" + AbpSession.TenantId.ToString());
                var SP_Name = Constants.Area.SP_AreaDelete;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                    new System.Data.SqlClient.SqlParameter("@AreaId",areaId)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                int result = (int)OutputParameter.Value;

                if (result != 0)
                {
                    if (result == 1)
                    {
                        return "MENU";
                    }
                    if (result == 2)
                    {
                        return "ITEM";
                    }
                    if (result == 3)
                    {
                        return "ORDER";
                    }
                    if (result == 4)
                    {
                        return "DELIVERYCOST";
                    }
                    if (result == 5)
                    {
                        return "LOCATION";
                    }
                }
                return "DELETED";

            }
            catch (Exception ex)
            {
                throw ex;
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
        private void BranchSettingUpdate(long BranchId, string branchSettingJson)
        {
            try
            {
                var SP_Name = "[dbo].[BranchSettingUpdate]";

                var sqlParameters = new List<SqlParameter> {
            new SqlParameter("@SettingJson",branchSettingJson)
           ,new SqlParameter("@BranchId",BranchId)
            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}