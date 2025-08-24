using Abp.UI;
using Abp.Webhooks;
using Framework.Data;
using GeoCoordinatePortable;
using IdentityModel.Client;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.DeliveryCost;
using Infoseed.MessagingPortal.DeliveryCost.Dto;
using Infoseed.MessagingPortal.Orders;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Infoseed.MessagingPortal.Careem_Express
{
    public class CareemExpressService: MessagingPortalAppServiceBase, ICareemExpressService
    {
        private readonly IDocumentClient _IDocumentClient;
        private readonly IDeliveryCostAppService _iDeliveryCostAppService;
        private readonly IAreasAppService _iAreasAppService;
        private readonly IConfiguration _configuration;
        private readonly IOrdersAppService _iOrdersAppService;


        public CareemExpressService(IDocumentClient IDocumentClient, IDeliveryCostAppService iDeliveryCostAppService,
            IAreasAppService iAreasAppService, IConfiguration configuration, IOrdersAppService IOrdersAppService)
        {
            _IDocumentClient = IDocumentClient;
            _iDeliveryCostAppService = iDeliveryCostAppService;
            _iAreasAppService= iAreasAppService;
            _configuration = configuration;
            _iOrdersAppService= IOrdersAppService;
        }

        public async Task<CreateDeliveryResponse> CreateDelivery([FromBody] CreateDeliveryDTO dto)
        {
            try
            {
                CreateDeliveryResponse deliveryResponse = new CreateDeliveryResponse();
                CareemCreateDeliveryDTO model = new CareemCreateDeliveryDTO();
                model.volume = null;

                SendLocationUserModel locationUserModel = new SendLocationUserModel
                {
                    query = $"{dto.customerLatitude},{dto.customerLongitude}",
                    tenantID = dto.tenantId,
                    isOrderOffer = false,
                    OrderTotal = 0,
                    menu = 0,
                    local = "ar",
                    isChangeLocation = false,
                    address = ""
                };

                var merchantLocation = GetlocationUserModel(locationUserModel);
                AreaDto area = _iAreasAppService.GetAreaById(merchantLocation.LocationId, dto.tenantId);
                var tenant = await GetTenantById(dto.tenantId);

                var pickupLocation = GetLocation($"{area.Latitude},{area.Longitude}");

                model.pickup = new Pickup
                {
                    coordinate = new Coordinate
                    {
                        latitude = (double)area.Latitude,
                        longitude = (double)area.Longitude
                    },
                    area = area.AreaName,
                    building_name = tenant.TenancyName,
                    city = pickupLocation.City,
                    street = pickupLocation.Route,
                    notes = dto.pickupNote
                };

                model.dropoff = new Dropoff
                {
                    coordinate = new Coordinate
                    {
                        latitude = dto.customerLatitude,
                        longitude = dto.customerLongitude
                    },
                    area = merchantLocation.Area,
                    building_name = dto.customerBuildingName,
                    city = merchantLocation.City,
                    street = dto.customerStreet,
                };

                model.outlet = new Outlet
                {
                    name = tenant.TenancyName,
                    phone_number = tenant.PhoneNumber
                };

                model.order = new Order
                {
                    reference = dto.tenantId.ToString() + "," + dto.orderNumber.ToString() + ","+ dto.customerPhoneNumber.ToString(),
                }; 

                var order = _iOrdersAppService.GetOrderByNumber(dto.orderNumber, dto.tenantId);
                var deliveryEstimation = JsonSerializer.Deserialize<EstimateDeliveryResponse>(order.DeliveryEstimation);

                model.cash_collection = new Cash_Collection
                {
                    amount = (double)order.Total + 0.20 + deliveryEstimation.trip_cost,
                    payment_type = "CASH"
                };

                var customer = GetContactByPhoneNumber(dto.customerPhoneNumber, dto.tenantId);
                model.customer = new Customer
                {
                    name = customer.DisplayName,
                    phone_number = dto.customerPhoneNumber,
                };

                using (var client = new HttpClient())
                {
                    var requestUrl = "https://sagateway.careem-engineering.com/b2b/deliveries";

                    var body = JsonSerializer.Serialize(model);
                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    var accessToken = string.Empty; //where to get it ??
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    try
                    {
                        var response = await client.PostAsync(requestUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            throw new UserFriendlyException($"Failed to create delivery: {error}");
                        }

                        var responseBody = await response.Content.ReadAsStringAsync();
                        deliveryResponse = JsonSerializer.Deserialize<CreateDeliveryResponse>(responseBody);

                        return deliveryResponse;
                    }
                    catch (HttpRequestException ex)
                    {
                        throw;
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpDelete]
        public async Task<string> CancelDelivery([FromQuery] string deliveryId, [FromQuery] string cancellationReason)
        {
            using (var client = new HttpClient())
            {
                var requestUrl = $"https://sagateway.careem-engineering.com/b2b/deliveries/{deliveryId}?cancellation_reason={cancellationReason}";

                var accessToken = string.Empty; //where to get it ??
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    var response = await client.DeleteAsync(requestUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new UserFriendlyException($"Failed to cancel delivery: {error}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }
        }

        public async Task<GetDeliveryResponse> GetDelivery([FromQuery] string deliveryId)
        {
            var delivery = new GetDeliveryResponse();
            using (var client = new HttpClient())
            {
                var requestUrl = $"https://sagateway.careem-engineering.com/b2b/deliveries/{deliveryId}";

                var accessToken = string.Empty; //where to get it ??
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    var response = await client.GetAsync(requestUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new UserFriendlyException($"Failed to get delivery: {error}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    delivery = JsonSerializer.Deserialize<GetDeliveryResponse>(responseBody);

                    return delivery;
                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<TrackDeliveryResponse> TrackDelivery(string deliveryId)
        {
            var delivery = new TrackDeliveryResponse();
            using (var client = new HttpClient())
            {
                var requestUrl = $"https://sagateway.careem-engineering.com/b2b/deliveries/{deliveryId}/tracking";

                var accessToken = string.Empty; //where to get it ??
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    var response = await client.GetAsync(requestUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new UserFriendlyException($"Failed to track delivery: {error}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    delivery = System.Text.Json.JsonSerializer.Deserialize<TrackDeliveryResponse>(responseBody);

                    return delivery;

                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }
        }


        #region private 

        private GetLocationInfoModel GetlocationUserModel(SendLocationUserModel input)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            try
            {
                if (IsvalidLatLong(input.query.Split(",")[0], input.query.Split(",")[1]))
                {
                    TenantModel tenant = GetTenantById(input.tenantID).Result;

                    input.isOrderOffer = tenant.isOrderOffer;
                    if (tenant.DeliveryCostType == DeliveryCostType.PerKiloMeter)
                    {
                        GetLocationInfoModel getLocationInfoModel = getLocationInfoPerKiloMeter(input.tenantID, input.query);
                        return getLocationInfoModel;
                    }
                    else
                    {
                        GetLocationInfoModel infoLocation = new GetLocationInfoModel();
                        string result = "-1";
                        string Country = "";
                        string City = "";
                        string Area = "";
                        string Distric = "";
                        string Route = "";

                        if (input.isChangeLocation)
                        {
                            Country = input.address.Split(" - ")[4].Replace("'", "").Trim();
                            City = input.address.Split(" - ")[3].Replace("'", "").Trim();
                            Area = input.address.Split(" - ")[2].Replace("'", "").Trim();
                            Distric = input.address.Split(" - ")[1].Replace("'", "").Trim();
                            Route = input.address.Split(" - ")[0].Replace("'", "").Trim();
                        }
                        else
                        {
                            var rez = GetLocation(input.query);
                            Country = rez.Country.Replace("'", "").Trim();
                            City = rez.City.Replace("'", "").Trim();
                            Area = rez.Area.Replace("'", "").Trim();
                            Distric = rez.Distric.Replace("'", "").Trim();
                            Route = rez.Route.Replace("'", "").Trim();
                        }

                        if (Distric == "Irbid Qasabah District" && Area == "")
                        {
                            Distric = Route;
                        }

                        if (Distric == "الجوادين" && Area == "")
                        {
                            Distric = "aljawadayn";
                        }
                        else if (Distric == "الساهرون" && Area == "")
                        {
                            Distric = "alsaahirun";
                        }
                        else if (Distric == "حي الضباط" && Area == "")
                        {
                            Distric = "hayu aldubaat";
                        }
                        else if (Distric == "الحي العسكري" && Area == "")
                        {
                            Distric = "alhayu aleaskariu";
                        }
                        else if (Distric == "البيضاء" && Area == "")
                        {
                            Distric = "albayda";
                        }
                        else if (Distric == "الرئاسة" && Area == "")
                        {
                            Distric = "alriyasa";
                        }
                        else if (Distric == "عدن" && Area == "")
                        {
                            Distric = "eadn";
                        }

                        try
                        {

                            decimal Latitude = decimal.Parse(input.query.Split(",")[0]);
                            decimal Longitude = decimal.Parse(input.query.Split(",")[1]);
                            try {
                                using (var connection = new SqlConnection(connString))
                                using (var command = connection.CreateCommand())
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "dbo.GetDeliveryCost";

                                    command.Parameters.AddWithValue("@Longitude", Longitude);
                                    command.Parameters.AddWithValue("@Latitude", Latitude);
                                    command.Parameters.AddWithValue("@City", City);
                                    command.Parameters.AddWithValue("@Area", Area);
                                    command.Parameters.AddWithValue("@Distric", Distric);
                                    command.Parameters.AddWithValue("@TenantId", input.tenantID);

                                    System.Data.SqlClient.SqlParameter returnValue = command.Parameters.Add("@DeliveryCost", SqlDbType.NVarChar);
                                    returnValue.Direction = ParameterDirection.ReturnValue;

                                    connection.Open();
                                    command.ExecuteNonQuery();

                                    result = returnValue.Value.ToString();

                                }
                            }

                            catch (Exception ex)
                            {
                                throw ex;
                            }
                         

                            var spilt = result.Split(",");
                            decimal va1 = -1;

                            try
                            {
                                va1 = decimal.Parse(spilt[0].ToString());
                            }
                            catch
                            {
                                throw;
                            }

                            if (va1 < 0)
                            {
                                infoLocation.Country = Country;
                                infoLocation.City = City;
                                infoLocation.Area = Area;
                                infoLocation.Distric = Distric;

                                infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                infoLocation.DeliveryCostAfter = -1;
                                infoLocation.DeliveryCostBefor = -1;
                                infoLocation.LocationId = 0;

                                return infoLocation;
                            }

                            if (spilt[0] == "-1" || spilt[0] == "")
                            {

                                infoLocation.Country = Country;
                                infoLocation.City = City;
                                infoLocation.Area = Area;
                                infoLocation.Distric = Distric;
                                infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                infoLocation.DeliveryCostAfter = -1;
                                infoLocation.DeliveryCostBefor = -1;
                                infoLocation.LocationId = 0;

                                return infoLocation;

                            }
                            else
                            {
                                try
                                {

                                    var locationList = GetAllLocationInfoModel();
                                    var cost = decimal.Parse(spilt[0]);
                                    var add = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;

                                    var cityModel = locationList.Where(x => x.LocationNameEn == City).FirstOrDefault();
                                    var areaModel = locationList.Where(x => x.LocationNameEn == Area).FirstOrDefault();
                                    var districModel = locationList.Where(x => x.LocationNameEn == Distric).FirstOrDefault();

                                    var areaname = GetAreas(input.tenantID).Where(x => x.Id == int.Parse(spilt[1])).FirstOrDefault();

                                    var cityName = "";
                                    var areaName = "";
                                    var districName = "";

                                    if (cityModel != null)
                                        cityName = cityModel.LocationName;
                                    if (areaModel != null)
                                        areaName = areaModel.LocationName;
                                    if (districModel != null)
                                        districName = districModel.LocationName;

                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = add;

                                    infoLocation.LocationId = int.Parse(spilt[1]);
                                    infoLocation.LocationAreaName = areaname.AreaName;

                                    if (!areaname.IsAvailableBranch)
                                    {
                                        infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                        infoLocation.DeliveryCostAfter = -1;
                                        infoLocation.DeliveryCostBefor = -1;
                                        infoLocation.LocationId = 0;

                                        return infoLocation;
                                    }

                                    OrderOfferFun(input.tenantID, input.isOrderOffer, input.OrderTotal, infoLocation, cityName, areaName, districName, cost);

                                    if (areaname != null)
                                    {
                                        if (input.local == "ar")
                                        {
                                            infoLocation.LocationAreaName = areaname.AreaName;
                                        }
                                        else
                                        {
                                            if (areaname.AreaNameEnglish == null)
                                            {
                                                infoLocation.LocationAreaName = areaname.AreaName;
                                            }
                                            else
                                            {
                                                infoLocation.LocationAreaName = areaname.AreaNameEnglish;
                                            }
                                        }
                                    }
                                    infoLocation.DeliveryCostAfter = cost;
                                    if (infoLocation.DeliveryCostBefor == null)
                                    {
                                        infoLocation.DeliveryCostBefor = 0;
                                    }
                                    return infoLocation;
                                }
                                catch
                                {
                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                    infoLocation.DeliveryCostAfter = -1;
                                    infoLocation.DeliveryCostBefor = -1;
                                    infoLocation.LocationId = 0;

                                    return infoLocation;
                                }
                            }

                        }
                        catch (Exception)
                        {
                            infoLocation.Country = Country;
                            infoLocation.City = City;
                            infoLocation.Area = Area;
                            infoLocation.Distric = Distric;

                            infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                            infoLocation.DeliveryCostAfter = -1;
                            infoLocation.DeliveryCostBefor = -1;
                            infoLocation.LocationId = 0;

                            return infoLocation;
                        }
                    }
                }
                else
                {
                    GetLocationInfoModel infoLocation = new GetLocationInfoModel();
                    infoLocation.Country = "";
                    infoLocation.City = "";
                    infoLocation.Area = "";
                    infoLocation.Distric = "";
                    infoLocation.Address = "";
                    infoLocation.DeliveryCostAfter = -1;
                    infoLocation.DeliveryCostBefor = -1;
                    infoLocation.LocationId = 0;

                    return infoLocation;
                }
            }
            catch (Exception)
            {
                GetLocationInfoModel infoLocation = new GetLocationInfoModel();

                infoLocation.Country = "";
                infoLocation.City = "";
                infoLocation.Area = "";
                infoLocation.Distric = "";

                infoLocation.Address = "";
                infoLocation.DeliveryCostAfter = -1;
                infoLocation.DeliveryCostBefor = -1;
                infoLocation.LocationId = 0;

                return infoLocation;
            }

        }

        private bool IsvalidLatLong(string latit, string longit)
        {
            double lon, lat;
            bool IsDoubleLat, IsDoubleLong, isValidLat, isValidLong;

            IsDoubleLat = double.TryParse(latit, out lat);
            IsDoubleLong = double.TryParse(longit, out lon);

            if (IsDoubleLat && IsDoubleLong)
            {
                if (lat < -90 || lat > 90)
                {
                    isValidLat = false;
                }
                else
                {
                    isValidLat = true;
                }
                if (lon < -180 || lon > 180)
                {
                    isValidLong = false;
                }
                else
                {
                    isValidLong = true;
                }
            }
            else
            {
                isValidLong = false;
                isValidLat = false;
            }

            return isValidLong && isValidLat;
        }

        private async Task<TenantModel> GetTenantById(int? id)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }

        private Contact GetContactByPhoneNumber(string phoneNumber, int tenantId)
        {
            try
            {
                var result = new Contact();
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GetContactByPhoneNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TenantId", tenantId);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                result.TenantId = reader.GetInt32(reader.GetOrdinal("TenantId"));
                                result.DisplayName = reader["DisplayName"] as string;
                                result.PhoneNumber = reader["DisplayName"] as string;
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private GetLocationInfoModel getLocationInfoPerKiloMeter(int tenantID, string query)
        {
            try
            {
                GetLocationInfoModel getLocationInfoModel = new GetLocationInfoModel();

                if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
                {
                    double latitude = double.Parse(query.Split(",")[0]);
                    double longitude = double.Parse(query.Split(",")[1]);
                    var rez = GetLocation(query);

                    getLocationInfoModel.Country = rez.Country.Replace("'", "").Trim();
                    getLocationInfoModel.City = rez.City.Replace("'", "").Trim();
                    getLocationInfoModel.Area = rez.Area.Replace("'", "").Trim();
                    getLocationInfoModel.Distric = rez.Distric.Replace("'", "").Trim();
                    string Route = rez.Route.Replace("'", "").Trim();

                    getLocationInfoModel.DeliveryCostAfter = -1;
                    getLocationInfoModel.DeliveryCostBefor = -1;
                    getLocationInfoModel.LocationId = 0;
                    getLocationInfoModel.Address = Route + " - " + getLocationInfoModel.Distric + " - " + getLocationInfoModel.Area + " - " + getLocationInfoModel.City + " - " + getLocationInfoModel.Country;
                    double distance;
                    AreaDto AreaDto = getNearbyArea(tenantID, latitude, longitude, null, 0, out distance);

                    if (!AreaDto.IsAvailableBranch)
                    {
                        getLocationInfoModel = new GetLocationInfoModel();
                        return getLocationInfoModel;
                    }
                    if (AreaDto.Id > 0 && distance != -1)
                    {
                        DeliveryCostDto deliveryCostDto = _iDeliveryCostAppService.GetDeliveryCostByAreaId(tenantID, AreaDto.Id);

                        if (deliveryCostDto != null)
                        {
                            decimal value = -1;
                            if (deliveryCostDto.lstDeliveryCostDetailsDto != null)
                            {
                                distance = distance / 1000.00; // convert a meter to kilo-meter
                                DeliveryCostDetailsDto deliveryCostDetailsDto = new DeliveryCostDetailsDto();
                                foreach (var item in deliveryCostDto.lstDeliveryCostDetailsDto)
                                {
                                    if (deliveryCostDetailsDto.To <= item.To)
                                        deliveryCostDetailsDto = item;

                                    if ((double)item.From <= distance && distance <= (double)item.To)
                                    {
                                        value = item.Value;
                                        break;
                                    }
                                }
                                if (value == -1)
                                {
                                    value = (Math.Ceiling((decimal)distance - deliveryCostDetailsDto.To) * deliveryCostDto.AboveValue) + deliveryCostDetailsDto.Value;
                                }
                            }
                            getLocationInfoModel.DeliveryCostAfter = value;
                            getLocationInfoModel.DeliveryCostBefor = value;
                        }
                    }
                    getLocationInfoModel.LocationId = (int)AreaDto.Id;
                    return getLocationInfoModel;
                }
                else
                {
                    return getLocationInfoModel;
                }
            }
            catch (Exception)
            {
                return new GetLocationInfoModel()
                {
                    DeliveryCostAfter = -1,
                    DeliveryCostBefor = -1,
                    LocationId = 0
                };
            }
        }

        private AreaDto getNearbyArea(int tenantID, double eLatitude, double eLongitude, string city, long areaId, out double distance)
        {
            distance = -1;

            bool isInAmman = !string.IsNullOrEmpty(city) && city.Trim().ToLower().Equals("amman");
            AreaDto areaDto = new AreaDto();
            var areas = _iAreasAppService.GetAllAreas(tenantID, true);
            List<AreaDto> lstAreaDto = new List<AreaDto>();

            if (areas != null)
            {
                foreach (var item in areas)
                {
                    if (checkIsInService2(item.SettingJson))
                    {
                        lstAreaDto.Add(item);
                    }
                }
            }

            if (!isInAmman && tenantID == 42)// Macdonalds
            {
                if (lstAreaDto != null)
                    lstAreaDto = lstAreaDto.Where(x => x.Id == areaId).ToList();
            }

            if (lstAreaDto != null)
            {
                foreach (var item in lstAreaDto)
                {
                    var sCoord = new GeoCoordinate(item.Latitude.Value, item.Longitude.Value);
                    var eCoord = new GeoCoordinate(eLatitude, eLongitude);
                    var currentDistance = sCoord.GetDistanceTo(eCoord);

                    if ((distance == -1 && !isInAmman) || (isInAmman && currentDistance < 5000 && distance == -1))
                    {
                        areaDto = new AreaDto();
                        areaDto.Id = item.Id;
                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
                        areaDto.AreaName = item.AreaName;
                        areaDto.AreaCoordinate = item.AreaCoordinate;
                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;

                        areaDto.IsAvailableBranch = item.IsAvailableBranch;
                        distance = currentDistance;

                    }
                    if ((distance > currentDistance && !isInAmman) || (isInAmman && currentDistance < 5000 && distance > currentDistance))
                    {
                        areaDto = new AreaDto();
                        areaDto.Id = item.Id;
                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
                        areaDto.AreaName = item.AreaName;
                        areaDto.AreaCoordinate = item.AreaCoordinate;
                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;
                        areaDto.IsAvailableBranch = item.IsAvailableBranch;
                        distance = currentDistance;

                    }
                }
            }
            return areaDto;
        }

        private locationAddressModel GetLocation(string query)
        {
            try
            {
                var client = new HttpClient();
                string Key = _configuration["GoogleMapsKey:KeyMap"];
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={query}&key=" + Key;
                var response = client.GetAsync(url).Result;

                var result = response.Content.ReadAsStringAsync().Result;
                var resultO = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleMapModel>(result);

                locationAddressModel locationAddressModel = new locationAddressModel();
                var rez = resultO;

                foreach (var item in rez.results)
                {
                    var route = item.types.Where(x => x == "street_address").FirstOrDefault();
                    if (route != null)
                    {
                        locationAddressModel.Route = item.formatted_address.Split(",")[0];
                    }
                    else
                    {
                        route = item.types.Where(x => x == "route").FirstOrDefault();
                        if (route != null)
                        {
                            locationAddressModel.Route = item.formatted_address.Split(",")[0];
                        }
                    }

                    //Distric
                    var neighborhood = item.types.Where(x => x == "neighborhood").FirstOrDefault();
                    if (neighborhood != null)
                    {
                        locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;
                    }
                    else
                    {
                        neighborhood = item.types.Where(x => x == "administrative_area_level_2").FirstOrDefault();
                        if (neighborhood != null && locationAddressModel.Distric == null)
                        {
                            locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;
                        }
                    }

                    //Area
                    var sublocality_level_1 = item.types.Where(x => x == "sublocality_level_1").FirstOrDefault();
                    if (sublocality_level_1 != null)
                    {
                        locationAddressModel.Area = item.address_components.FirstOrDefault().long_name;
                    }

                    //city
                    var locality = item.types.Where(x => x == "locality").FirstOrDefault();
                    if (locality != null)
                    {
                        locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];
                    }
                    else
                    {
                        locality = item.types.Where(x => x == "administrative_area_level_1").FirstOrDefault();
                        if (locality != null)
                        {
                            if (item.address_components.FirstOrDefault().long_name.Split(",")[0] == "Jerash Governorate")
                            {
                                locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];
                            }
                        }
                    }

                    //Country
                    var country = item.types.Where(x => x == "country").FirstOrDefault();
                    if (country != null)
                    {
                        locationAddressModel.Country = item.address_components.FirstOrDefault().long_name;
                    }
                }

                if (locationAddressModel.Route == null)
                    locationAddressModel.Route = "";
                if (locationAddressModel.Distric == null)
                    locationAddressModel.Distric = "";
                if (locationAddressModel.Area == null)
                    locationAddressModel.Area = "";
                if (locationAddressModel.City == null)
                    locationAddressModel.City = "";
                if (locationAddressModel.Country == null)
                    locationAddressModel.Country = "";

                return locationAddressModel;

            }
            catch (Exception)
            {
                locationAddressModel locationAddressModel = new locationAddressModel();

                if (locationAddressModel.Route == null)
                    locationAddressModel.Route = "";
                if (locationAddressModel.Distric == null)
                    locationAddressModel.Distric = "";
                if (locationAddressModel.Area == null)
                    locationAddressModel.Area = "";
                if (locationAddressModel.City == null)
                    locationAddressModel.City = "";
                if (locationAddressModel.Country == null)
                    locationAddressModel.Country = "";

                return locationAddressModel;
            }
        }

        private List<LocationInfoModel> GetAllLocationInfoModel()
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Locations] ";

                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                DataSet dataSet = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dataSet);

                List<LocationInfoModel> locationInfoModel = new List<LocationInfoModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        locationInfoModel.Add(new LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                            ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                        });
                    }
                    catch
                    {
                        locationInfoModel.Add(new LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                        });
                    }
                }

                conn.Close();
                da.Dispose();

                return locationInfoModel;
            }
            catch
            {
                return null;
            }
        }

        private List<Area> GetAreas(int TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where TenantID=" + TenantID;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            DataSet dataSet = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dataSet);

            List<Area> branches = new List<Area>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                try
                {
                    branches.Add(new Area
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                        AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString(),
                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString()

                    });
                }
                catch
                {
                    branches.Add(new Area
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                    });
                }
            }
            conn.Close();
            da.Dispose();

            return branches;
        }

        private void OrderOfferFun(int tenantID, bool isOrderOffer, decimal OrderTotal, GetLocationInfoModel infoLocation, string ci, string ar, string dis, decimal costDistric)
        {
            if (isOrderOffer)
            {
                var orderEffor = GetOrderOffer(tenantID);

                if (infoLocation.LocationAreaName == null)
                    infoLocation.LocationAreaName = "";

                var item = orderEffor.Where(x => (x.BranchesIds.Contains(infoLocation.LocationId.ToString()) && x.isPersentageDiscount == false));//.FirstOrDefault(); x.Area.Contains(ci) || x.Area.Contains(ar) || x.Area.Contains(dis) &&

                foreach (var areaEffor in item)
                {
                    if (areaEffor != null)
                    {
                        if (!areaEffor.isPersentageDiscount)
                        {
                            if (OrderTotal >= areaEffor.FeesStart && OrderTotal <= areaEffor.FeesEnd)
                            {
                                var DateNow = Convert.ToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                var DateStart = Convert.ToDateTime(areaEffor.OrderOfferDateStart.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                var DateEnd = Convert.ToDateTime(areaEffor.OrderOfferDateEnd.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

                                if (DateStart <= DateNow && DateEnd >= DateNow)
                                {
                                    var timeNow = Convert.ToDateTime(DateTime.UtcNow.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture));
                                    var timeStart = Convert.ToDateTime(areaEffor.OrderOfferStart.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));
                                    var timeEnd = Convert.ToDateTime(areaEffor.OrderOfferEnd.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));

                                    if ((timeStart <= timeNow && timeNow <= timeEnd))
                                    {
                                        infoLocation.DeliveryCostAfter = costDistric;
                                        infoLocation.DeliveryCostBefor = areaEffor.NewFees;
                                        infoLocation.isOrderOfferCost = true;

                                        break;
                                    }
                                    else
                                    {
                                        infoLocation.DeliveryCostAfter = costDistric;
                                        infoLocation.DeliveryCostBefor = 0;
                                        infoLocation.isOrderOfferCost = false;
                                    }
                                }
                                else
                                {
                                    infoLocation.DeliveryCostAfter = costDistric;
                                    infoLocation.DeliveryCostBefor = 0;
                                    infoLocation.isOrderOfferCost = false;
                                }
                            }
                            else
                            {
                                infoLocation.DeliveryCostAfter = costDistric;
                                infoLocation.DeliveryCostBefor = 0;
                                infoLocation.isOrderOfferCost = false;
                            }
                        }
                        else
                        {
                            infoLocation.DeliveryCostAfter = costDistric;
                            infoLocation.DeliveryCostBefor = 0;
                            infoLocation.isOrderOfferCost = false;
                        }
                    }
                    else
                    {
                        infoLocation.DeliveryCostAfter = costDistric;
                        infoLocation.DeliveryCostBefor = 0;
                        infoLocation.isOrderOfferCost = false;
                    }
                }
            }
            else
            {
                infoLocation.DeliveryCostAfter = costDistric;
                infoLocation.DeliveryCostBefor = 0;
                infoLocation.isOrderOfferCost = false;
            }
        }

        private bool checkIsInService2(string workingHourSetting)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(workingHourSetting))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var workModel = System.Text.Json.JsonSerializer.Deserialize<MessagingPortal.Configuration.Tenants.Dto.WorkModel>(workingHourSetting, options);

                DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
                DayOfWeek wk = currentDateTime.DayOfWeek;
                TimeSpan timeOfDay = currentDateTime.TimeOfDay;

                switch (wk)
                {
                    case DayOfWeek.Saturday:
                        if (workModel.IsWorkActiveSat)
                        {
                            var StartDateSat = getValidValue(workModel.StartDateSat);
                            var EndDateSat = getValidValue(workModel.EndDateSat);

                            var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
                            var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

                            if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Sunday:
                        if (workModel.IsWorkActiveSun)
                        {
                            var StartDate = getValidValue(workModel.StartDateSun);
                            var EndDate = getValidValue(workModel.EndDateSun);

                            var StartDateSP = getValidValue(workModel.StartDateSunSP);
                            var EndDateSP = getValidValue(workModel.EndDateSunSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Monday:

                        if (workModel.IsWorkActiveMon)
                        {
                            var StartDate = getValidValue(workModel.StartDateMon);
                            var EndDate = getValidValue(workModel.EndDateMon);

                            var StartDateSP = getValidValue(workModel.StartDateMonSP);
                            var EndDateSP = getValidValue(workModel.EndDateMonSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Tuesday:
                        if (workModel.IsWorkActiveTues)
                        {
                            var StartDate = getValidValue(workModel.StartDateTues);
                            var EndDate = getValidValue(workModel.EndDateTues);

                            var StartDateSP = getValidValue(workModel.StartDateTuesSP);
                            var EndDateSP = getValidValue(workModel.EndDateTuesSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        if (workModel.IsWorkActiveWed)
                        {
                            var StartDate = getValidValue(workModel.StartDateWed);
                            var EndDate = getValidValue(workModel.EndDateWed);

                            var StartDateSP = getValidValue(workModel.StartDateWedSP);
                            var EndDateSP = getValidValue(workModel.EndDateWedSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        if (workModel.IsWorkActiveThurs)
                        {
                            var StartDate = getValidValue(workModel.StartDateThurs);
                            var EndDate = getValidValue(workModel.EndDateThurs);

                            var StartDateSP = getValidValue(workModel.StartDateThursSP);
                            var EndDateSP = getValidValue(workModel.EndDateThursSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Friday:
                        if (workModel.IsWorkActiveFri)
                        {
                            var StartDate = getValidValue(workModel.StartDateFri);
                            var EndDate = getValidValue(workModel.EndDateFri);

                            var StartDateSP = getValidValue(workModel.StartDateFriSP);
                            var EndDateSP = getValidValue(workModel.EndDateFriSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;

        }

        private List<OrderOffers.OrderOffer> GetOrderOffer(int TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[OrderOffer] where TenantID=" + TenantID;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            DataSet dataSet = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dataSet);

            List<OrderOffers.OrderOffer> order = new List<OrderOffers.OrderOffer>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                if (!IsDeleted)
                {
                    try
                    {
                        order.Add(new OrderOffers.OrderOffer
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
                            Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
                            FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
                            FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
                            NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
                            TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
                            OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
                            OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
                            OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            isPersentageDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isPersentageDiscount"].ToString()),
                            isBranchDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isBranchDiscount"].ToString()),

                            BranchesName = dataSet.Tables[0].Rows[i]["BranchesName"].ToString(),
                            BranchesIds = dataSet.Tables[0].Rows[i]["BranchesIds"].ToString(),
                        });
                    }
                    catch
                    {
                        order.Add(new OrderOffers.OrderOffer
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
                            Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
                            FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
                            FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
                            NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
                            TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
                            OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
                            OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
                            OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            isPersentageDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isPersentageDiscount"].ToString()),
                            isBranchDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isBranchDiscount"].ToString()),
                        });
                    }
                }
            }

            conn.Close();
            da.Dispose();

            return order;
        }

        private DateTime getValidValue(dynamic value)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                result = DateTime.Parse(value.ToString());
                return result;
            }
            catch (Exception)
            {
                return result;
                throw;
            }
        }


        #endregion








    }
}
