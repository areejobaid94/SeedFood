using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Framework.Data;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Web.Models.Location;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.Contacts.Dtos;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : MessagingPortalControllerBase
    {
        private IConfiguration configuration;
        public LocationsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        
        [HttpGet("GetAllLocationList")]
        public async Task<GetLocationForViewDto> GetAllLocationList(int SkipCountt ,int MaxResultCountt, int TenantId,string Sorting)
        {
           
            var SkipCount = SkipCountt;
            var MaxResultCount = MaxResultCountt;

            GetLocationForViewDto getLocationForViewDto = new GetLocationForViewDto();
            List<LocationInfoModel> locationInfoModels = new List<LocationInfoModel>();
            List<LocationInfoModel> locationInfoModelsFormat = new List<LocationInfoModel>();

            var List3 = GetAllAreaLocation3();
            var List2 = GetAllAreaLocation2();
            var List1 = GetAllAreaLocation1();
            var AllList = GetAllAreaLocationList();
            var order = GetAllOrder(TenantId);

            foreach (var item3 in List3)
            {
                var item2 = List2.Where(x => x.Id == item3.ParentId).FirstOrDefault();
                var item1 = List1.Where(x => x.Id == item2.ParentId).FirstOrDefault();

                var countO = order.Where(x => x.LocationID == item3.Id).ToList().Count();

                locationInfoModels.Add(new LocationInfoModel
                {
                    GoogleURL= item1.GoogleURL,
                    CityName = item1.LocationName,
                    AreaName = item2.LocationName,
                    DistrictName = item3.LocationName,
                    OrderCount= countO


                });

            }


            var query = locationInfoModels.AsQueryable();

            //var pagedAndFilteredOrders = query
            //    .OrderBy(getAllLocationInput.Sorting ?? "id asc");

            var pagedAndFilteredOrders = query.OrderBy(Sorting ?? "orderCount ASC");

            var list = pagedAndFilteredOrders.ToList();

            list.Reverse();

            var count = 0;

            foreach (var item in list)
            {

                count++;
                if (MaxResultCount >= count)
                {
                    locationInfoModelsFormat.Add(item);


                }
            }
            getLocationForViewDto.locationInfoModel = locationInfoModelsFormat;

            var totalCount = locationInfoModelsFormat.Count();
            return getLocationForViewDto;

        }
        [HttpGet("GetAllDeliveryLocationList")]
        public async Task<List<DeliveryLocationInfoModel>> GetAllDeliveryLocationList(int SkipCountt, int MaxResultCountt, int TenantId, string Sorting)
        {
            var SkipCount = SkipCountt;
            var MaxResultCount = MaxResultCountt;

            List<DeliveryLocationInfoModel> deliveryLocationInfoModels = new List<DeliveryLocationInfoModel>();
            List<DeliveryLocationInfoModel> deliveryLocationInfoModels2 = new List<DeliveryLocationInfoModel>();



            var deliveryLocationCost = GetAllDeliveryLocationCost(TenantId);

            var List3 = GetAllAreaLocation3();
            var List2 = GetAllAreaLocation2();
            var List1 = GetAllAreaLocation1();

            var order = GetAllDeliveryOrder(TenantId);

            foreach (var delivery in deliveryLocationCost)
            {
                var fromitem3 = List3.Where(x => x.Id == delivery.FromLocationId).FirstOrDefault();
                var fromitem2 = List2.Where(x => x.Id == fromitem3.ParentId).FirstOrDefault();
                var fromitem1 = List1.Where(x => x.Id == fromitem2.ParentId).FirstOrDefault();

                var toitem3 = List3.Where(x => x.Id == delivery.ToLocationId).FirstOrDefault();
                var toitem2 = List2.Where(x => x.Id == toitem3.ParentId).FirstOrDefault();
                var toitem1 = List1.Where(x => x.Id == toitem2.ParentId).FirstOrDefault();

                var countO = order.Where(x => x.FromLocationID == delivery.FromLocationId && x.ToLocationID== delivery.ToLocationId).ToList().Count();

                deliveryLocationInfoModels.Add(new DeliveryLocationInfoModel
                {
                    Id = delivery.Id,
                    FromCityName = fromitem1.LocationName,
                    FromCityId = fromitem1.Id,

                    FromAreaName = fromitem2.LocationName,
                    FromAreaId = fromitem2.Id,

                    FromDistrictName = fromitem3.LocationName,
                    FromDistrictId = fromitem3.Id,


                    ToCityName = toitem1.LocationName,
                    ToCityId = toitem1.Id,

                    ToAreaName = toitem2.LocationName,
                    ToAreaId = toitem2.Id,

                    ToDistrictName = toitem3.LocationName,
                    ToDistrictId = toitem3.Id,

                    FromGoogleURL = fromitem3.GoogleURL,
                    ToGoogleURL = toitem3.GoogleURL,

                    DeliveryCost = delivery.DeliveryCost,
                    OrderCount= countO

                });

            }



            var query = deliveryLocationInfoModels.AsQueryable();

            //var pagedAndFilteredOrders = query
            //    .OrderBy(getAllLocationInput.Sorting ?? "id asc");

            var pagedAndFilteredOrders = query.OrderBy(Sorting ?? "orderCount ASC");

            var list = pagedAndFilteredOrders.ToList();

            list.Reverse();

            var count = 0;

            foreach (var item in list)
            {

                count++;
                if (MaxResultCount >= count)
                {
                    deliveryLocationInfoModels2.Add(item);


                }
            }

            var totalCount = deliveryLocationInfoModels2.Count();
            return deliveryLocationInfoModels2;

        }



        [HttpGet("GetLocations")]
        public async Task<IActionResult> GetLocations(int? locationid)
        {
            //AbpSession.TenantId
            if (locationid == -1)
                locationid = null;
            var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@TenantId",AbpSession.TenantId),
                                                 new SqlParameter("@LocationId",locationid),

                     };

            IList<LocationInfoModel> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.LocationsGet",
                       sqlParameters.ToArray(),
                       MapLocation, configuration.GetConnectionString("Default"));

            List<LocationInfoModel> all = new List<LocationInfoModel>();


            var listLocationDelivery = GetAllLocationDeliveryListP(AbpSession.TenantId);
            var listLocation = GetAllLocation();
            var areaList = GetArea();


            foreach (var list in result)
            {
                var xx = listLocationDelivery.Where(x => x.LocationId == list.Id).FirstOrDefault();

                if(xx!=null)
                {
                    var location = listLocation.Where(x => x.Id == xx.LocationId).FirstOrDefault();//GetAllLocationsListP(list.LocationId);



                    var Area = listLocation.Where(x => x.Id == location.ParentId).FirstOrDefault();// GetAllArea(location.ParentId);
                    //var City = listLocation.Where(x => x.Id == location.ParentId).FirstOrDefault();//GetAllCitty(Area.ParentId);
                    var branch = areaList.Where(x => x.Id == xx.BranchAreaId).FirstOrDefault(); //GetArea(list.BranchAreaId);


                    location.CityId = list.CityId;
                    location.CityName = list.CityName;

                    location.AreaId = Area.Id;
                    location.AreaName = Area.LocationName;
                    location.GoogleURL = list.GoogleURL;
                    location.DeliveryCost = xx.DeliveryCost;
                    location.TenantId = xx.TenantId;
                    location.BranchAreaId = xx.BranchAreaId;
                    if (branch!=null)
                    {
                        location.BranchAreaRes = branch.AreaName;
                        location.BranchAreaCor = branch.AreaCoordinate;
                    }

                  
                    all.Add(location);

                }
               


            }

            return Ok(all);
        }

        [HttpGet("GetRootLocations")]
        public async Task<IActionResult> GetRootLocations()
        {
            //AbpSession.TenantId
            var sqlParameters = new List<SqlParameter>
            {
            };

            IList<LocationInfoModel> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.LocationsGetRoots",
                       sqlParameters.ToArray(),
                       MapLocation,
                        configuration.GetConnectionString("Default"));
  
            return Ok(result);
        }

        [HttpGet("GetLocationsByParentId")]
        public async Task<IActionResult> GetLocationsByParentId(int parentId)
        {
            //AbpSession.TenantId
            var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@ParentId",parentId)
                     };
            IList<LocationInfoModel> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.LocationsGetByParentId",
                       sqlParameters.ToArray(),
                       MapLocation,
                        configuration.GetConnectionString("Default"));       

            return Ok(result);
        }


        [HttpGet("GetAreaByCityId")]
        public  List<LocationInfoModel> GetAreaByCityId(int parentId)
        {

            var List2 = GetAllAreaLocation2().Where(x=>x.ParentId== parentId).ToList();


            return List2;
        }
        [HttpGet("GetDistrictsByAreaId")]
        public List<LocationInfoModel> GetDistrictsByAreaId(int parentId)
        {

            var List3 = GetAllAreaLocation3().Where(x => x.ParentId == parentId).ToList();


            return List3;
        }

        private async Task<List<LocationInfoModel>> GetLocationsByParentIdF(int? parentId)
        {
            //AbpSession.TenantId
            var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@ParentId",parentId)
                     };


            IList<LocationInfoModel> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.LocationsGetByParentId",
                       sqlParameters.ToArray(),
                       MapLocation,
                        configuration.GetConnectionString("Default"));




            return result.ToList();
        }

        [HttpPost("EditLocation")]
        public async Task<IActionResult> EditLocation(LocationInfoModel model)
        {
            //AbpSession.TenantId
            var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@LocationId",model.Id),
                                                 new SqlParameter("@DeliveryCost",model.DeliveryCost),
                                                 new SqlParameter("@TenantId",AbpSession.TenantId),
                                                 new SqlParameter("@BranchAreaId",model.BranchAreaId)
                     };

                   SqlDataHelper.ExecuteNoneQuery(
                       "dbo.LocationEdit",
                       sqlParameters.ToArray(),
                       configuration.GetConnectionString("Default"));

            return Ok(model);
        }


        [HttpPost("DeleteLocation")]
        public async Task<IActionResult> DeleteLocation(int locationId)
        {
            //AbpSession.TenantId
            var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@LocationId",locationId),                                                
                                                 new SqlParameter("@TenantId",AbpSession.TenantId)
                     };

            SqlDataHelper.ExecuteNoneQuery(
                "dbo.LocationDelete",
                sqlParameters.ToArray(),
                configuration.GetConnectionString("Default"));

            return Ok();
        }


        [HttpPost("AddLocation")]
        public async Task<IActionResult> AddLocation(LocationInfoModel model)
        {
            //AbpSession.TenantId
            //List<LocationInfoModel> model = new List<LocationInfoModel>();

            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@LocationId",model.Id),
                    new SqlParameter("@DeliveryCost",model.DeliveryCost),
                    new SqlParameter("@TenantId",AbpSession.TenantId),
                    new SqlParameter("@BranchAreaId",model.BranchAreaId),
                    new SqlParameter("@HasSubDistrict",model.HasSubDistrict)
            };


             SqlDataHelper.ExecuteNoneQuery("dbo.LocationAdd",sqlParameters.ToArray(), configuration.GetConnectionString("Default"));

            return Ok(model);
        }

        private static LocationInfoModel MapLocation(IDataReader dataReader)
        {
            LocationInfoModel model = new LocationInfoModel
            {
                Id = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                LocationName = SqlDataHelper.GetValue<string>(dataReader, "LocationName"),
                LevelId = SqlDataHelper.GetValue<int>(dataReader, "LevelId"),
                ParentId = SqlDataHelper.GetValue<int>(dataReader, "ParentId"),
                GoogleURL = SqlDataHelper.GetValue<string>(dataReader, "GoogleURL"),
                LocationNameEn = SqlDataHelper.GetValue<string>(dataReader, "LocationNameEn"),
                AreaName = SqlDataHelper.GetValue<string>(dataReader, "AreaName"),
                CityName = SqlDataHelper.GetValue<string>(dataReader, "CityName"),
                DeliveryCost = SqlDataHelper.GetValue<string>(dataReader, "DeliveryCost"),
                 AreaId = SqlDataHelper.GetValue<int>(dataReader, "AreaId"),
                  CityId = SqlDataHelper.GetValue<int>(dataReader, "CityId"),


            };
            return model;
        }

        //Done
        [HttpGet("GetAllLocationsList")]
        public async Task<PagedResultDto<LocationInfoModel>> GetAllLocationsList( int? TenantId, int SkipCountt, int MaxResultCountt, string Sorting)
        {
 
            GetAllContactsInput input = new GetAllContactsInput();

            input.Sorting = Sorting;
            input.SkipCount = SkipCountt;
            input.MaxResultCount = MaxResultCountt;
            List<LocationInfoModel> all = new List<LocationInfoModel>() ;
            

            var listLocationDelivery = GetAllLocationDeliveryListP(TenantId);
            var listLocation = GetAllLocation();
            var areaList= GetArea();

           // var districtCost = GetAllSubDistrict();


            foreach (var list in listLocationDelivery)
            {
                var location = listLocation.Where(x => x.Id == list.LocationId).FirstOrDefault();//GetAllLocationsListP(list.LocationId);



                   var Area = listLocation.Where(x => x.Id == location.ParentId).FirstOrDefault();// GetAllArea(location.ParentId);
                    var City = listLocation.Where(x => x.Id == Area.ParentId).FirstOrDefault();//GetAllCitty(Area.ParentId);
                   var branch = areaList.Where(x => x.Id == list.BranchAreaId).FirstOrDefault(); //GetArea(list.BranchAreaId);

                if (branch != null)
                {

                    location.BranchAreaRes = branch.AreaName;
                    location.BranchAreaCor = branch.AreaCoordinate;
                }

              //  location.Id= list.Id;
                location.CityId = City.Id;
                location.CityName = City.LocationName;

                location.AreaId = Area.Id;
                location.AreaName = Area.LocationName;

                location.DeliveryCost = list.DeliveryCost;
                location.TenantId = list.TenantId;
                location.BranchAreaId = list.BranchAreaId;
                location.GoogleURL = location.GoogleURL;
              
                location.HasSubDistrict = list.HasSubDistrict;

                location.DeliveryCostId = list.Id;

                all.Add(location);
               

            }


    


           var pagedAndFilteredContacts = all.AsQueryable()
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input).ToList();

            var totalCount = all.Count();

            return new PagedResultDto<LocationInfoModel>(
                totalCount,
                pagedAndFilteredContacts
            );

        }




        [HttpGet("GetAllDeliveryLocationCost")]
        public async Task<List<DeliveryLocationInfoModel>> GetAllDeliveryLocationCost(int SkipCountt, int MaxResultCountt, int TenantId, string Sorting)
        {
            var SkipCount = SkipCountt;
            var MaxResultCount = MaxResultCountt;

            List<DeliveryLocationInfoModel> deliveryLocationInfoModels = new List<DeliveryLocationInfoModel>();
            List<DeliveryLocationInfoModel> deliveryLocationInfoModels2 = new List<DeliveryLocationInfoModel>();
            // GetLocationForViewDto getLocationForViewDto = new GetLocationForViewDto();
            //List<LocationInfoModel> locationInfoModels = new List<LocationInfoModel>();
            //List<LocationInfoModel> locationInfoModelsFormat = new List<LocationInfoModel>();


            var deliveryLocationCost = GetAllDeliveryLocationCost(TenantId);

            var List3 = GetAllAreaLocation3();
            var List2 = GetAllAreaLocation2();
            var List1 = GetAllAreaLocation1();
          //  var AllList = GetAllAreaLocationList();

           // var order = GetAllOrder(TenantId);

            foreach (var delivery in deliveryLocationCost)
            {
                var fromitem3 = List3.Where(x => x.Id == delivery.FromLocationId).FirstOrDefault();
                var fromitem2 = List2.Where(x => x.Id == fromitem3.ParentId).FirstOrDefault();
                var fromitem1 = List1.Where(x => x.Id == fromitem2.ParentId).FirstOrDefault();

                var toitem3 = List3.Where(x => x.Id == delivery.ToLocationId).FirstOrDefault();
                var toitem2 = List2.Where(x => x.Id == toitem3.ParentId).FirstOrDefault();
                var toitem1 = List1.Where(x => x.Id == toitem2.ParentId).FirstOrDefault();

                // var countO = order.Where(x => x.LocationID == item3.Id).ToList().Count();

                deliveryLocationInfoModels.Add(new DeliveryLocationInfoModel
                {
                     Id= delivery.Id,
                    FromCityName = fromitem1.LocationName,
                    FromCityId= fromitem1.Id,

                    FromAreaName = fromitem2.LocationName,
                    FromAreaId = fromitem2.Id,

                    FromDistrictName = fromitem3.LocationName,
                    FromDistrictId = fromitem3.Id,


                    ToCityName = toitem1.LocationName,
                    ToCityId = toitem1.Id,

                    ToAreaName = toitem2.LocationName,
                    ToAreaId = toitem2.Id,

                    ToDistrictName = toitem3.LocationName,
                    ToDistrictId = toitem3.Id,

                    FromGoogleURL= fromitem3.GoogleURL,
                    ToGoogleURL= toitem3.GoogleURL,

                    DeliveryCost= delivery.DeliveryCost

                });

            }



            var query = deliveryLocationInfoModels.AsQueryable();

            //var pagedAndFilteredOrders = query
            //    .OrderBy(getAllLocationInput.Sorting ?? "id asc");

            var pagedAndFilteredOrders = query.OrderBy(Sorting ?? "deliveryCost ASC");

            var list = pagedAndFilteredOrders.ToList();

            list.Reverse();

            var count = 0;

            foreach (var item in list)
            {

                count++;
                if (MaxResultCount >= count)
                {
                    deliveryLocationInfoModels2.Add(item);


                }
            }

            var totalCount = deliveryLocationInfoModels2.Count();
            return deliveryLocationInfoModels2;




          //  return deliveryLocationInfoModels;

        }
        [HttpPost("AddDeliveryLocationCost")]
        public void AddDeliveryLocationCost(DeliveryLocationInfoModel deliveryLocationCost)
        {
            if (deliveryLocationCost.Id == 0)
            {

                Add(deliveryLocationCost);
            }
            else
            {
                Update(deliveryLocationCost);

            }

        }
        [HttpGet("GetAllSubDistrict")]
        public List<SubDistrictsModel> GetAllSubDistrict(int TenantId, int deliveryCostID)
        {
            SubDistrictsModel subDistrictsModel = new SubDistrictsModel
            {
                TenantId = TenantId,
                 LocationDeliveryCostId= deliveryCostID

            };

            var list = getSubDis(TenantId, deliveryCostID);
            if (list.Count == 0)
            {
                list.Add(subDistrictsModel);
                list.Add(subDistrictsModel);
                list.Add(subDistrictsModel);
            }

            if (list.Count == 1)
            {
                list.Add(subDistrictsModel);
                list.Add(subDistrictsModel);
            }
                
            if (list.Count == 2)
            {
                list.Add(subDistrictsModel);
            }

            return list;

        }

        [HttpPost("EdiatOrCreateSubDistrict")]
        public void EdiatOrCreateSubDistrict(int TenantId, int deliveryCostID, List<SubDistrictsModel> subDistrictsModels)
        {

            foreach(var item in subDistrictsModels)
            {
                if(item.Id==0)
                {

                    if (item.LongitudeAndLatitude != null)
                    {
                        item.TenantId = TenantId;
                        item.Longitude = decimal.Parse(item.LongitudeAndLatitude.Split(",")[0]);
                        item.Latitude = decimal.Parse(item.LongitudeAndLatitude.Split(",")[1]);
                        item.LocationDeliveryCostId = deliveryCostID;
                        AddSub(item);

                    }
                   

                }
                else
                {
                    if (item.LongitudeAndLatitude == null|| item.LongitudeAndLatitude == "")
                    {
                        delete(item.Id);
                    }
                    else
                    {
                        item.TenantId = TenantId;
                        item.Longitude = decimal.Parse(item.LongitudeAndLatitude.Split(",")[0]);
                        item.Latitude = decimal.Parse(item.LongitudeAndLatitude.Split(",")[1]);
                        item.LocationDeliveryCostId = deliveryCostID;
                        UpdateSub(item);
                        

                    }
             
                }

            }
            
            

        }

        private List<SubDistrictsModel> getSubDis(int TenantId, int deliveryCostID)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[LocationDeliveryCostDetails] where TenantId =" + TenantId + " and LocationDeliveryCostId =" + deliveryCostID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<SubDistrictsModel> location = new List<SubDistrictsModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {


                    location.Add(new SubDistrictsModel
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString() ?? "0"),
                        LocationDeliveryCostId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LocationDeliveryCostId"].ToString() ?? "0"),
                        DeliveryCost = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? "0"),
                        Latitude = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["Latitude"].ToString() ?? "0"),
                        Longitude = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["Longitude"].ToString() ?? "0"),
                        LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString() ?? "",
                        LocationNameEn = (dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString() ?? ""),
                        LongitudeAndLatitude = dataSet.Tables[0].Rows[i]["Longitude"].ToString() + "," + dataSet.Tables[0].Rows[i]["Latitude"].ToString()

                    });





                }

                conn.Close();
                da.Dispose();

                return location;
            }
            catch (Exception )
            {
                return null;
            }
        }



        [HttpPost("Delete")]
        public void delete(DeliveryLocationInfoModel deliveryLocationCost)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "DELETE FROM  DeliveryLocationCost Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }

        private void delete(int id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "DELETE FROM  LocationDeliveryCostDetails Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", id);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }
        private void Add(DeliveryLocationInfoModel deliveryLocationCost)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO DeliveryLocationCost (FromLocationId , ToLocationId, TenantId, DeliveryCost, BranchAreaId) VALUES (@FromLocationId ,@ToLocationId, @TenantId, @DeliveryCost, @BranchAreaId) ";

                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
                        command.Parameters.AddWithValue("@FromLocationId", deliveryLocationCost.FromDistrictId);
                        command.Parameters.AddWithValue("@ToLocationId", deliveryLocationCost.ToDistrictId);
                        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
                        command.Parameters.AddWithValue("@DeliveryCost", deliveryLocationCost.DeliveryCost);
                        command.Parameters.AddWithValue("@BranchAreaId", deliveryLocationCost.BranchAreaId);


                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }
        private void AddSub(SubDistrictsModel  subDistrictsModel)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO LocationDeliveryCostDetails (TenantId , LocationDeliveryCostId, LocationName, CreationTime, DeliveryCost ,Longitude ,Latitude) VALUES"
                            + " (@TenantId ,@LocationDeliveryCostId, @LocationName, @CreationTime, @DeliveryCost, @Longitude ,@Latitude) ";

                        command.Parameters.AddWithValue("@TenantId", subDistrictsModel.TenantId);
                        command.Parameters.AddWithValue("@LocationDeliveryCostId", subDistrictsModel.LocationDeliveryCostId);
                        command.Parameters.AddWithValue("@LocationName", subDistrictsModel.LocationName);
                        command.Parameters.AddWithValue("@CreationTime", DateTime.Now);
                        command.Parameters.AddWithValue("@DeliveryCost", subDistrictsModel.DeliveryCost);
                        command.Parameters.AddWithValue("@Longitude", subDistrictsModel.Longitude);
                        command.Parameters.AddWithValue("@Latitude", subDistrictsModel.Latitude);


                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }
        private void Update(DeliveryLocationInfoModel deliveryLocationCost)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "UPDATE DeliveryLocationCost SET FromLocationId = @FromLocationId, ToLocationId = @ToLocationId , TenantId = @TenantId, DeliveryCost = @DeliveryCost , BranchAreaId = @BranchAreaId Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
                        command.Parameters.AddWithValue("@FromLocationId", deliveryLocationCost.FromDistrictId);
                        command.Parameters.AddWithValue("@ToLocationId", deliveryLocationCost.ToDistrictId);
                        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
                        command.Parameters.AddWithValue("@DeliveryCost", deliveryLocationCost.DeliveryCost);
                        command.Parameters.AddWithValue("@BranchAreaId", deliveryLocationCost.BranchAreaId);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }
        private void UpdateSub(SubDistrictsModel  subDistrictsModel)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "UPDATE LocationDeliveryCostDetails SET   LocationName = @LocationName, DeliveryCost = @DeliveryCost , Longitude = @Longitude ,Latitude=@Latitude   Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", subDistrictsModel.Id);
                        command.Parameters.AddWithValue("@LocationName", subDistrictsModel.LocationName);
                       
                        command.Parameters.AddWithValue("@DeliveryCost", subDistrictsModel.DeliveryCost);
                        command.Parameters.AddWithValue("@Longitude", subDistrictsModel.Longitude);
                        command.Parameters.AddWithValue("@Latitude", subDistrictsModel.Latitude);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }



        private List<LocationInfoModel> GetAllLocation()
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Locations]";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<LocationInfoModel> location = new List<LocationInfoModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);
                    try
                    {
                        location.Add(new LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString() ?? "0"),
                            ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString() ?? "0") ,
                            //GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString() ?? "",
                            GoogleURL = (dataSet.Tables[0].Rows[i]["GoogleURL"].ToString() ?? ""),
                            //LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                        });
                    }
                    catch
                    {
                        location.Add(new LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString() ?? "0"),
                            // ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString() ?? "0") ,
                            //GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString() ?? "",
                            //LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                        });
                    }
                   



                }

                conn.Close();
                da.Dispose();

                return location;
            }
            catch(Exception )
            {
                return null;
            }
          

        }

        private List<LocationInfoModel> GetAllLocationDeliveryListP(int? TenantId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[LocationDeliveryCost] where TenantId= " + TenantId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<LocationInfoModel> location = new List<LocationInfoModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                location.Add(new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                    LocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LocationId"].ToString()),
                    DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
                     BranchAreaId= Convert.ToInt32(dataSet.Tables[0].Rows[i]["BranchAreaId"].ToString()),
                      HasSubDistrict= Convert.ToBoolean(dataSet.Tables[0].Rows[i]["HasSubDistrict"].ToString()),
                    //  GoogleURL = (dataSet.Tables[0].Rows[i]["GoogleURL"].ToString()??""),
                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }

        private LocationInfoModel GetAllLocationsListP(int? LocationId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations] where Id= " + LocationId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            LocationInfoModel location = new LocationInfoModel();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                location=new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                     LevelId= Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                     ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                    GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                };



            }

            conn.Close();
            da.Dispose();

            return location;

        }
        private LocationInfoModel GetAllArea(int? LocationId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations] where Id= " + LocationId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            LocationInfoModel location = new LocationInfoModel();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                location=new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                    ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                    //GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    //LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                };



            }

            conn.Close();
            da.Dispose();

            return location;

        }

        private LocationInfoModel GetAllCitty(int? LocationId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations] where Id= " + LocationId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

           LocationInfoModel location = new LocationInfoModel();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                location=new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                    //ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                    //GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    //LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                };



            }

            conn.Close();
            da.Dispose();

            return location;

        }

        private List<AreaDto> GetArea()
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where TenantId=" + AbpSession.TenantId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<AreaDto> area = new List<AreaDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                area.Add(new AreaDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                     AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
                    AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),                  
                    // UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"].ToString()),
                    
                });



            }

            conn.Close();
            da.Dispose();

            return area;

        }

        private List<LocationInfoModel> GetAllAreaLocation3()
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations] where LevelId = 3";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<LocationInfoModel> location = new List<LocationInfoModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                location.Add(new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                    ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                    GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }
        private List<LocationInfoModel> GetAllAreaLocation2()
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations] where LevelId = 2";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<LocationInfoModel> location = new List<LocationInfoModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                location.Add(new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                    ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                    GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }
        private List<LocationInfoModel> GetAllAreaLocation1()
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations] where LevelId = 1";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<LocationInfoModel> location = new List<LocationInfoModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                location.Add(new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                   // ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                    //GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }
        private List<LocationInfoModel> GetAllAreaLocationList()
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations]";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<LocationInfoModel> location = new List<LocationInfoModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                location.Add(new LocationInfoModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }

        private List<Order> GetAllOrder(int TenantId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where OrderType = 1 and TenantId="+ TenantId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Order> location = new List<Order>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                location.Add(new Order
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                    OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                    LocationID= int.Parse(dataSet.Tables[0].Rows[i]["LocationID"].ToString()),

                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }
        private List<Order> GetAllDeliveryOrder(int TenantId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where OrderType = 1 and TenantId=" + TenantId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Order> location = new List<Order>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                location.Add(new Order
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                    OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                    LocationID = int.Parse(dataSet.Tables[0].Rows[i]["LocationID"].ToString()),
                     FromLocationID= int.Parse(dataSet.Tables[0].Rows[i]["FromLocationID"].ToString()),
                      ToLocationID = int.Parse(dataSet.Tables[0].Rows[i]["ToLocationID"].ToString())
                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }

        private List<Models.Location.DeliveryLocationCost> GetAllDeliveryLocationCost(int TenantId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[DeliveryLocationCost] where  TenantId=" + TenantId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Models.Location.DeliveryLocationCost> location = new List<Models.Location.DeliveryLocationCost>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                location.Add(new Models.Location.DeliveryLocationCost
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    FromLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["FromLocationId"]),
                    ToLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ToLocationId"]),
                    DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    BranchAreaId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BranchAreaId"])


                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }



      


    }
}
