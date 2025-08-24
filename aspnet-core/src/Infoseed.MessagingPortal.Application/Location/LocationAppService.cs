using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Framework.Data;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Location.Dto;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Infoseed.MessagingPortal.Location
{
    public class LocationAppService : MessagingPortalAppServiceBase, ILocationAppService
    {
        private readonly IAreasAppService _areasAppService;
        public LocationAppService(IAreasAppService areasAppService)
        {
            _areasAppService = areasAppService;
        }
        public List<LocationModel> GetRootLocations()
        {
            return getRootLocation();
        }

        public List<LocationModel> GetCountryLocation()
        {
            return getCountryLocation();
        }
        public List<LocationModel> GetLocationsByParentId(int parentId)
        {
            return getLocationsByParentId(parentId);
        }
        public PagedResultDto<LocationModel> GetAllLocations(int skipCount, int maxResultCount, string sorting, int? tenantId = null, int? cityId = null, int? areaId = null)
        {
            return getAllLocations(skipCount, maxResultCount, sorting, tenantId,cityId,areaId);
        }

        public int CreateOrUpdateLocation(LocationModel model)
        {
            return createOrUpdateLocation(model);
        }
        public void CreateDefaultLocation(int tenantId)
        {
            createDefaultLocation(tenantId);
        }
        #region Private Methods
        private void createDefaultLocation(int tenantId)
        {
            try
            {
                var SP_Name = Constants.Location.SP_LocationDeliveryCostDefaultAdd;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@TenantId",tenantId)

                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<LocationModel> getRootLocation()
        {
            try
            {
                List<LocationModel> location = new List<LocationModel>();
                var SP_Name = Constants.Location.SP_LocationsGetRoots;

                var sqlParameters = new List<SqlParameter>();
                location = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapLocation, AppSettingsModel.ConnectionStrings).ToList();

                return location;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private List<LocationModel> getCountryLocation()
        {
            try
            {
                List<LocationModel> location = new List<LocationModel>();
                var SP_Name = Constants.Location.SP_LocationsGetCountry;

                var sqlParameters = new List<SqlParameter>();
                location = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapLocation, AppSettingsModel.ConnectionStrings).ToList();

                return location;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private List<LocationModel> getLocationsByParentId(int parentId)
        {
            try
            {
                List<LocationModel> location = new List<LocationModel>();
                var SP_Name = Constants.Location.SP_LocationsGetByParentId;

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@ParentId",parentId)
                };
                location = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapLocation, AppSettingsModel.ConnectionStrings).ToList();

                return location;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private PagedResultDto<LocationModel> getAllLocations(int SkipCount, int MaxResultCount, string Sorting, int? tenantId = null,int? cityId = null , int? areaId = null)
        {
            try
            {
                if (tenantId == null)
                {
                    tenantId = AbpSession.TenantId.Value;
                }
                List<LocationModel> AllLocation = new List<LocationModel>();
                LocationModel input = new LocationModel();
                input.Sorting = Sorting;
                input.SkipCount = SkipCount;
                input.MaxResultCount = MaxResultCount;
                List<LocationModel> locations = getAllLocation();
                List<LocationModel> deliveryCosts = getAllDeliveryCost(tenantId.Value);
                List<AreaDto> areas = _areasAppService.GetAllAreas(tenantId.Value);
                var branch = new AreaDto();
                var City = new LocationModel();
                var Area = new LocationModel();
                var location = new LocationModel();

                if ((cityId == null && areaId == null) || cityId == 357 || areaId == 359)
                {
                    foreach (var deliveryCost in deliveryCosts)
                    {

                        location = locations.Where(x => x.LocationId == deliveryCost.LocationId).FirstOrDefault();
                        branch = areas.Where(x => x.Id == deliveryCost.BranchAreaId).FirstOrDefault();


                        Area = locations.Where(x => x.LocationId == location.ParentId).FirstOrDefault();
                        City = locations.Where(x => x.LocationId == Area.ParentId).FirstOrDefault();

                        if (branch != null)
                        {
                            location.BranchAreaRes = branch.AreaName;
                            location.BranchAreaCor = branch.AreaCoordinate;
                        }

                        if (City != null)
                        {
                            location.CityId = City.LocationId;
                            location.CityName = City.LocationName;
                        }

                        if (Area != null)
                        {
                            location.AreaId = Area.LocationId;
                            location.AreaName = Area.LocationName;
                        }
                        
                        location.DeliveryCost = deliveryCost.DeliveryCost;
                        location.TenantId = deliveryCost.TenantId;
                        location.BranchAreaId = deliveryCost.BranchAreaId;
                        location.HasSubDistrict = deliveryCost.HasSubDistrict;
                        location.DeliveryCostId = deliveryCost.DeliveryCostId;
                        location.IsAvailable = deliveryCost.IsAvailable;

                        AllLocation.Add(location);

                    }
                }
                else
                {
                    if (areaId != null && cityId != null)
                    {
                        foreach (var deliveryCost in deliveryCosts)
                        {
                            location = locations.Where(x => x.LocationId == deliveryCost.LocationId && x.ParentId == areaId ).FirstOrDefault();
                            
                            if (location != null)
                            {

                                Area = locations.Where(x => x.ParentId == cityId && x.LocationId == location.ParentId).FirstOrDefault();
                                City = locations.Where(x => x.LocationId == Area.ParentId).FirstOrDefault();

                                branch = areas.Where(x => x.Id == deliveryCost.BranchAreaId).FirstOrDefault();
                                if (branch != null)
                                {
                                    location.BranchAreaRes = branch.AreaName;
                                    location.BranchAreaCor = branch.AreaCoordinate;
                                }

                                location.CityId = City.LocationId;
                                location.CityName = City.LocationName;

                                location.AreaId = Area.LocationId;
                                location.AreaName = Area.LocationName;

                                location.DeliveryCost = deliveryCost.DeliveryCost;
                                location.TenantId = deliveryCost.TenantId;
                                location.BranchAreaId = deliveryCost.BranchAreaId;
                                location.HasSubDistrict = deliveryCost.HasSubDistrict;
                                location.DeliveryCostId = deliveryCost.DeliveryCostId;
                                location.IsAvailable = deliveryCost.IsAvailable;

                                AllLocation.Add(location);
                            }
                        }

                    }
                }
                var result = AllLocation.AsQueryable().OrderBy(input.Sorting ?? "LocationId asc").PageBy(input).ToList();

                var TotalCount = deliveryCosts.Count();
                return new PagedResultDto<LocationModel>(TotalCount,result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private List<LocationModel> getAllLocation()
        {
            try
            {
                List<LocationModel> location = new List<LocationModel>();
                var SP_Name = Constants.Location.SP_LocationsGetAll;

                var sqlParameters = new List<SqlParameter>();
                location = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.MapLocation, AppSettingsModel.ConnectionStrings).ToList();

                return location;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<LocationModel> getAllDeliveryCost(int tenantId)
        {
            try
            {
                List<LocationModel> location = new List<LocationModel>();
                var SP_Name = Constants.Location.SP_LocationsAllDeliveryCostGet;

                var sqlParameters = new List<SqlParameter>
                {
                        new SqlParameter("@TenantId",tenantId)
                };
                location = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapLocation, AppSettingsModel.ConnectionStrings).ToList();

                return location;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int createOrUpdateLocation(LocationModel model)
        {
            try
            {
                var SP_Name = Constants.Location.SP_LocationAdd;
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@LocationId",model.LocationId),
                    new SqlParameter("@DeliveryCost",model.DeliveryCost),
                    new SqlParameter("@TenantId",AbpSession.TenantId),
                    new SqlParameter("@BranchAreaId",model.BranchAreaId),
                    new SqlParameter("@HasSubDistrict",model.HasSubDistrict),
                    new SqlParameter("@IsAvailable",model.IsAvailable),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        #endregion
    }
}
