using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Web.Models;
using Framework.Data;
using GraphQL;
using Infoseed.MessagingPortal.Careem_Express;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Engine.Model;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using NUglify.JavaScript.Syntax;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Engine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareemAPIController : MessagingPortalControllerBase
    {
        private readonly IOrdersAppService _iOrdersAppService;
        private readonly ICacheManager _cacheManager;
        IDBService _dbService;
        private readonly IDocumentClient _IDocumentClient;




        public CareemAPIController(IOrdersAppService IOrdersAppService, ICacheManager cacheManager
            , IDocumentClient iDocumentClient, IDBService dbService)
        {
            _iOrdersAppService = IOrdersAppService;
            _cacheManager = cacheManager;
            _dbService = dbService;
            _IDocumentClient = iDocumentClient;

        }

        [HttpGet("webhook")]
        [DontWrapResult]
        public string Webhook(
       [FromQuery(Name = "hub.mode")] string mode,
       [FromQuery(Name = "hub.challenge")] string challenge,
       [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            string currentDattime = "InfoSeed-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            if (verify_token.Equals(currentDattime) && mode.Equals("subscribe"))
            {
                return challenge;
            }
            else
            {
                return null;
            }
        }

        [HttpPost("webhook")]
        public async void Webhook(dynamic jsonData)
        {
            try
            {
                var msg= string.Empty;

                var json = JsonConvert.SerializeObject(jsonData);
                CareemModel model = JsonConvert.DeserializeObject<CareemModel>(json);

                var orderRefrence = model.details.external_order_reference.Split(","); 
                var tenantId = Convert.ToInt32(orderRefrence[0]);
                var orderNumber = Convert.ToInt64(orderRefrence[1]);
                var customerPhoneNumber = orderRefrence[2];

                var deliveryId = model.details.delivery_id;
                var trackingDelivery = new TrackDeliveryResponse();

                try
                {
                    trackingDelivery = await TrackDelivery(deliveryId);
                }
                catch (UserFriendlyException ex)
                {
                    return; // stop further execution
                }



                Web.Models.Sunshine.TenantModel Tenant = new Web.Models.Sunshine.TenantModel();

                var objTenant = _cacheManager.GetCache("CacheTenant").Get(tenantId.ToString(), cache => cache);
                if (objTenant.Equals(tenantId.ToString()))
                {
                    Tenant = await _dbService.GetTenantInfoById(tenantId); 
                    _cacheManager.GetCache("CacheTenant").Set(tenantId.ToString(), Tenant);
                }
                else
                {
                    Tenant = (Web.Models.Sunshine.TenantModel)objTenant;
                }

                //insert log
                AddCareemLog(tenantId, json);

                //update order ETA
                if(model.details.captain_eta != null)
                {
                    var str = JsonConvert.SerializeObject(model.details.captain_eta);
                    _iOrdersAppService.updateOrderETA(orderNumber, tenantId, str);
                }

                //update zeedly status for the order
                //switch (model.event_type)
                //{
                //    case "DELIVERY_STARTED":
                //        var update1 = _iOrdersAppService.updateOrderZeedlyStatus(orderNumber, tenantId,(int)ZeedlyOrderStatus.OrderOnTheWay);
                //        msg = $"Your Order Status: Order on the way\n\nDelivery Captin info:\nname: {model.details.captain.name}\n" +
                //            $"phone number: {model.details.captain.phone_number}\n\nCaptain Location:{location}\n\n"+
                //            $"Esimated Time of Arrival:\ndistance: {model.details.captain_eta.distance / 1000.0}km\n"+
                //            $"duration: {model.details.captain_eta.duration / 60.0}mins ";
                //        break;

                //    case "DELIVERY_ENDED":
                //        var update2 = _iOrdersAppService.updateOrderZeedlyStatus(orderNumber, tenantId, (int)ZeedlyOrderStatus.Delivered);
                //        msg = $"Your Order Status: Delivered\n\nDelivery Captain info:\nname: {model.details.captain.name}\n" +
                //            $"phone number: {model.details.captain.phone_number}\n";
                //        break;

                //    case "CANCELED":
                //        var update3 = _iOrdersAppService.updateOrderZeedlyStatus(orderNumber, tenantId, (int)ZeedlyOrderStatus.NSNA);
                //        msg = $"Your Order Status: No show no answer\n\nDelivery Captain info:\nname: {model.details.captain.name}\n" +
                //            $"phone number: {model.details.captain.phone_number}\n";
                //        break;

                //    default:
                //        break;
                //}
                switch (model.event_type)
                {
                    case "DELIVERY_STARTED":
                        var update1 = _iOrdersAppService.updateOrderZeedlyStatus(orderNumber, tenantId, (int)ZeedlyOrderStatus.OrderOnTheWay);
                        msg = $"حالة طلبك: الطلب في الطريق\n\nمعلومات السائق:\nالاسم: {model.details.captain.name}\n" +
                              $"رقم الهاتف: {model.details.captain.phone_number}\n\nموقع السائق: {trackingDelivery.tracking_url}\n\n" +
                              $"الوقت المقدر للوصول:\nالمسافة: {model.details.captain_eta.distance / 1000.0} كم\n" +
                              $"المدة: {model.details.captain_eta.duration / 60.0} دقيقة";
                        break;

                    case "DELIVERY_ENDED":
                        var update2 = _iOrdersAppService.updateOrderZeedlyStatus(orderNumber, tenantId, (int)ZeedlyOrderStatus.Delivered);
                        msg = $"حالة طلبك: تم التوصيل\n\nمعلومات السائق:\nالاسم: {model.details.captain.name}\n" +
                              $"رقم الهاتف: {model.details.captain.phone_number}\n";
                        break;

                    case "CANCELED":
                        var update3 = _iOrdersAppService.updateOrderZeedlyStatus(orderNumber, tenantId, (int)ZeedlyOrderStatus.NSNA);
                        msg = $"حالة طلبك: لم يتم الرد\n\nمعلومات السائق:\nالاسم: {model.details.captain.name}\n" +
                              $"رقم الهاتف: {model.details.captain.phone_number}\n";
                        break;

                    default:
                        break;
                }


                //send update to the consumer
                if (!msg.IsNullOrEmpty())
                {
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.to = customerPhoneNumber.ToString();

                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                    postWhatsAppMessageModel.text.body = msg;

                    SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
                    {
                        text = msg,
                        type = "text",
                    };

                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken, Tenant.IsD360Dialog);
                    if (result)
                    {
                        //var userId = tenantId.ToString() + "_" + customerPhoneNumber.ToString();
                        var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                        var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && 
                                        a.phoneNumber.Contains(customerPhoneNumber) && a.TenantId == Tenant.TenantId);
                        var Customer = await customerResult;

                        var CustomerChat = UpdateCustomerChat(Customer.TenantId, message, Customer.userId, Customer.SunshineConversationId);
                        Customer.customerChat = CustomerChat;
                        SocketIOManager.SendContact(Customer, (int)Customer.TenantId);
                    }
                }
            }
            catch (Exception ex) { throw; }
        }


        #region private 

        private async Task<TrackDeliveryResponse> TrackDelivery(string deliveryId)
        {
            var delivery = new TrackDeliveryResponse();
            using (var client = new HttpClient())
            {
                var requestUrl = $"https://sagateway.careem-engineering.com/b2b/deliveries/{deliveryId}/tracking";

                var accessToken = string.Empty; //where to get it ??
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
        }

        private void AddCareemLog(int tenantId, string json)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.InsertCareemLog";

                        command.Parameters.AddWithValue("@TenantId", tenantId);
                        command.Parameters.AddWithValue("@EventJson", json);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ContactDto GetContactById(int tenantId, long contactId)
        {
            ContactDto contact = new ContactDto();

            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.GetContactById";

                    command.Parameters.AddWithValue("@TenantId", tenantId);
                    command.Parameters.AddWithValue("@ContactId", contactId);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contact = new ContactDto
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                DisplayName = reader["DisplayName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                            };
                        }
                    }
                }
            }
            catch
            {
                throw; 
            }

            return contact;
        }

        private CustomerChat UpdateCustomerChat(int? tenantId, SunshinePostMsgBotModel.Content model, string userId, string conversationID)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);

            // Create your new conversation instance
            var CustomerChat = new CustomerChat()
            {
                messageId = Guid.NewGuid().ToString(),
                userId = userId,
                SunshineConversationId = conversationID,
                text = model.text,
                type = model.type,
                fileName = model.fileName,
                CreateDate = DateTime.Now,
                status = 1,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                agentName = model.agentName,
                agentId = model.agentId,
                TenantId = tenantId,

            };
            var result = itemsCollection.CreateItemAsync(CustomerChat).Result;

            return CustomerChat;
        }



        #endregion





    }
}
