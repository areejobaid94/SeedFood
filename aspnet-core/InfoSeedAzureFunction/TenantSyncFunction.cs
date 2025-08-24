using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using WebJobEntities;
using InfoSeedAzureFunction.Model;

namespace InfoSeedAzureFunction
{
    public static class TenantSyncFunction
    {
        private static AccessTokenModel accessTokenModel { get; set; }
        [FunctionName("TenantSyncFunction")]
        public static void Run([QueueTrigger("tenant-sync", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            // log.LogInformation($"C# Queue trigger function processed: {message}");
            TenantSyncMessage obj = JsonConvert.DeserializeObject<TenantSyncMessage>(message);
            Sync(Constants.ConnectionString, obj).Wait();
        }
        public static async Task Sync(string connectionString, TenantSyncMessage tenantMessage)
        {

            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Id",tenantMessage.TenantId),
            };


            IList<TenantModel> result = SqlDataHelper.ExecuteReader("[dbo].[TenantsGetById]", sqlParameters.ToArray(), MapTenantModel, connectionString);


            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
            //var tenant = itemsCollection.GetItemAsync(p => p.TenantId == item.TenantId && p.ItemType == ContainerItemTypes.Tenant).Result;

            foreach (var item in result)
            {
                var tenant = itemsCollection.GetItemAsync(p => p.TenantId == item.TenantId && p.ItemType == ContainerItemTypes.Tenant).Result;
                if (tenant == null)
                {


                    if (item.LiveChatWorkingHours == null)
                        item.LiveChatWorkingHours = new WorkModel();
                    await itemsCollection.CreateItemAsync(new TenantModel()
                    {
                        ItemType = ContainerItemTypes.Tenant,
                        SmoochAppID = item.SmoochAppID,
                        SmoochSecretKey = item.SmoochSecretKey,
                        SmoochAPIKeyID = item.SmoochAPIKeyID,
                        TenantId = item.TenantId,

                        DirectLineSecret = item.DirectLineSecret,
                        botId = item.botId,
                        IsBotActive = item.IsBotActive,
                        IsCancelOrder = item.IsCancelOrder,
                        CancelTime = item.CancelTime,

                        //StartDate = item.StartDate,
                        //EndDate = item.EndDate,

                        //WorkText = item.WorkText,
                        IsWorkActive = item.IsWorkActive,
                        IsLiveChatWorkActive = item.IsLiveChatWorkActive,

                        IsBellOn = item.IsBellOn,
                        IsBellContinues = item.IsBellContinues,
                        workModel = new WorkModel
                        {
                            EndDateFri = new DateTime(),
                            EndDateMon = new DateTime(),
                            EndDateSat = new DateTime(),
                            EndDateSun = new DateTime(),
                            EndDateThurs = new DateTime(),
                            EndDateTues = new DateTime(),
                            EndDateWed = new DateTime(),


                            EndDateFriSP = new DateTime(),
                            EndDateMonSP = new DateTime(),
                            EndDateSatSP = new DateTime(),
                            EndDateSunSP = new DateTime(),
                            EndDateThursSP = new DateTime(),
                            EndDateTuesSP = new DateTime(),
                            EndDateWedSP = new DateTime(),



                            IsWorkActiveFri = item.workModel.IsWorkActiveFri,
                            IsWorkActiveMon = item.workModel.IsWorkActiveMon,
                            IsWorkActiveSat = item.workModel.IsWorkActiveSat,
                            IsWorkActiveSun = item.workModel.IsWorkActiveSun,
                            IsWorkActiveThurs = item.workModel.IsWorkActiveThurs,
                            IsWorkActiveTues = item.workModel.IsWorkActiveTues,
                            IsWorkActiveWed = item.workModel.IsWorkActiveWed,

                            StartDateFri = new DateTime(),
                            StartDateMon = new DateTime(),
                            StartDateSat = new DateTime(),
                            StartDateSun = new DateTime(),
                            StartDateThurs = new DateTime(),
                            StartDateTues = new DateTime(),
                            StartDateWed = new DateTime(),


                            StartDateFriSP = new DateTime(),
                            StartDateMonSP = new DateTime(),
                            StartDateSatSP = new DateTime(),
                            StartDateSunSP = new DateTime(),
                            StartDateThursSP = new DateTime(),
                            StartDateTuesSP = new DateTime(),
                            StartDateWedSP = new DateTime(),



                            WorkTextFri = item.workModel.WorkTextFri,
                            WorkTextMon = item.workModel.WorkTextMon,
                            WorkTextSat = item.workModel.WorkTextSat,
                            WorkTextSun = item.workModel.WorkTextSun,
                            WorkTextThurs = item.workModel.WorkTextThurs,
                            WorkTextTues = item.workModel.WorkTextTues,
                            WorkTextWed = item.workModel.WorkTextWed,

                            HasSPSat = item.workModel.HasSPSat,
                            HasSPSun = item.workModel.HasSPSat,
                            HasSPMon = item.workModel.HasSPMon,
                            HasSPTues = item.workModel.HasSPTues,
                            HasSPWed = item.workModel.HasSPWed,
                            HasSPThurs = item.workModel.HasSPThurs,
                            HasSPFri = item.workModel.HasSPFri,



                        },

                        LiveChatWorkingHours = new WorkModel
                        {
                            EndDateFri = new DateTime(),
                            EndDateMon = new DateTime(),
                            EndDateSat = new DateTime(),
                            EndDateSun = new DateTime(),
                            EndDateThurs = new DateTime(),
                            EndDateTues = new DateTime(),
                            EndDateWed = new DateTime(),

                            IsWorkActiveFri = item.LiveChatWorkingHours.IsWorkActiveFri,
                            IsWorkActiveMon = item.LiveChatWorkingHours.IsWorkActiveMon,
                            IsWorkActiveSat = item.LiveChatWorkingHours.IsWorkActiveSat,
                            IsWorkActiveSun = item.LiveChatWorkingHours.IsWorkActiveSun,
                            IsWorkActiveThurs = item.LiveChatWorkingHours.IsWorkActiveThurs,
                            IsWorkActiveTues = item.LiveChatWorkingHours.IsWorkActiveTues,
                            IsWorkActiveWed = item.LiveChatWorkingHours.IsWorkActiveWed,

                            StartDateFri = new DateTime(),
                            StartDateMon = new DateTime(),
                            StartDateSat = new DateTime(),
                            StartDateSun = new DateTime(),
                            StartDateThurs = new DateTime(),
                            StartDateTues = new DateTime(),
                            StartDateWed = new DateTime(),

                            WorkTextFri = item.LiveChatWorkingHours.WorkTextFri,
                            WorkTextMon = item.LiveChatWorkingHours.WorkTextMon,
                            WorkTextSat = item.LiveChatWorkingHours.WorkTextSat,
                            WorkTextSun = item.LiveChatWorkingHours.WorkTextSun,
                            WorkTextThurs = item.LiveChatWorkingHours.WorkTextThurs,
                            WorkTextTues = item.LiveChatWorkingHours.WorkTextTues,
                            WorkTextWed = item.LiveChatWorkingHours.WorkTextWed

                        },

                        IsEvaluation = item.IsEvaluation,
                        EvaluationText = item.EvaluationText,
                        EvaluationTime = item.EvaluationTime,

                        IsLoyalityPoint = item.IsLoyalityPoint,
                        Points = item.Points,

                        D360Key = item.D360Key,
                        IsD360Dialog = item.IsD360Dialog,
                        isOrderOffer = item.isOrderOffer,
                        PhoneNumber = item.PhoneNumber,
                        Image = item.Image,
                        ImageBg = item.ImageBg
                        ,
                        IsBundleActive = true,
                        DeliveryCostTypeId = 0,
                        AccessToken = item.AccessToken,

                        WhatsAppAccountID = item.WhatsAppAccountID,
                        WhatsAppAppID = item.WhatsAppAppID,
                        BotTemplateId = item.BotTemplateId,
                        BIDailyLimit = item.BIDailyLimit,
                        CurrencyCode = item.CurrencyCode,
                        TimeZoneId = item.TimeZoneId,
                        IsPreOrder = item.IsPreOrder,
                        IsPickup = item.IsPickup,
                        IsDelivery = item.IsDelivery,
                        ReminderBookingHour = item.ReminderBookingHour,
                        BookingCapacity = item.BookingCapacity,
                        ZohoCustomerId = item.ZohoCustomerId,
                        IsCaution = item.IsCaution,
                        IsPaidInvoice = item.IsPaidInvoice,

                        CautionDays = item.CautionDays,
                        WarningDays = item.WarningDays,
                        UnAvailableBookingDates = item.UnAvailableBookingDates,
                        ConfirmRequestText = item.ConfirmRequestText,
                        RejectRequestText = item.RejectRequestText,

                        IsActiveMenuReminder = item.IsActiveMenuReminder,
                        MenuReminderMessage = item.MenuReminderMessage,
                        TimeReminder = item.TimeReminder   ,

                        IsMenuLinkFirst = item.IsMenuLinkFirst,
                        IsBotLanguageAr = item.IsBotLanguageAr,
                        IsBotLanguageEn = item.IsBotLanguageEn,
                         IsSelectPaymentMethod = item.IsSelectPaymentMethod,
                        IsInquiry = item.IsInquiry,
                        MerchantID = item.MerchantID,

                         FacebookPageId = item.FacebookPageId,
                          FacebookAccessToken = item.FacebookAccessToken,
                           InstagramId = item.InstagramId,
                            InstagramAccessToken = item.InstagramAccessToken

                    });

                }
                else
                {



                    tenant.ItemType = ContainerItemTypes.Tenant;
                    tenant.TenantId = item.TenantId;
                    tenant.DirectLineSecret = item.DirectLineSecret;
                    tenant.botId = item.botId;
                    tenant.PhoneNumber = item.PhoneNumber;
                    tenant.D360Key = item.D360Key;
                    tenant.Image = item.Image;
                    tenant.ImageBg = item.ImageBg;
                    tenant.IsBundleActive = tenant.IsBundleActive;
                    tenant.AccessToken = item.AccessToken;
                    tenant.WhatsAppAccountID = item.WhatsAppAccountID;
                    tenant.WhatsAppAppID = item.WhatsAppAppID;
                    tenant.BotTemplateId = item.BotTemplateId;
                    tenant.BIDailyLimit = item.BIDailyLimit;
                    tenant.IsBellOn = item.IsBellOn;
                    tenant.IsBellContinues = item.IsBellContinues;
                    tenant.CurrencyCode = item.CurrencyCode;
                    tenant.TimeZoneId = item.TimeZoneId;
                    tenant.IsPreOrder = item.IsPreOrder;
                    tenant.IsDelivery = item.IsDelivery;
                    tenant.IsPickup = item.IsPickup;
                    tenant.ZohoCustomerId = item.ZohoCustomerId;
                    tenant.ReminderBookingHour = item.ReminderBookingHour;
                    tenant.BookingCapacity = item.BookingCapacity;
                    tenant.CautionDays = item.CautionDays;
                    tenant.WarningDays = item.WarningDays;
                    tenant.UnAvailableBookingDates = item.UnAvailableBookingDates;

                    tenant.IsPaidInvoice = item.IsPaidInvoice;
                    tenant.IsCaution = item.IsCaution;
                    tenant.ConfirmRequestText = item.ConfirmRequestText;
                    tenant.RejectRequestText = item.RejectRequestText;
                    //try
                    //{
                    //    var model = CheckInvoicesFunction(tenant).Result;
                    //    tenant.IsPaidInvoice= model.IsPaidInvoice;
                    //    tenant.IsCaution=model.IsCaution;
                    //}
                    //catch
                    //{

                    //}
                    tenant.IsD360Dialog = item.IsD360Dialog;

                    tenant.MerchantID = item.MerchantID;

                    tenant.FacebookPageId = item.FacebookPageId;
                    tenant.FacebookAccessToken = item.FacebookAccessToken;
                    tenant.InstagramId = item.InstagramId;
                    tenant.InstagramAccessToken = item.InstagramAccessToken;

                    await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                }
            }
        }
        private static TenantModel MapTenantModel(IDataReader dataReader)
        {
            TenantModel catalogue = new TenantModel
            {
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                SmoochAppID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAppID"),
                SmoochSecretKey = SqlDataHelper.GetValue<string>(dataReader, "SmoochSecretKey"),
                SmoochAPIKeyID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAPIKeyID"),

                DirectLineSecret = SqlDataHelper.GetValue<string>(dataReader, "DirectLineSecret"),
                botId = SqlDataHelper.GetValue<string>(dataReader, "botId"),
                IsBotActive = SqlDataHelper.GetValue<bool>(dataReader, "IsBotActive"),

                IsCancelOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsCancelOrder"),
                CancelTime = SqlDataHelper.GetValue<int>(dataReader, "CancelTime"),

                //StartDate = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDate"),
                //EndDate = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDate"),
                //WorkText = SqlDataHelper.GetValue<string>(dataReader, "WorkText"),
                IsWorkActive = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActive"),
                IsBellOn = SqlDataHelper.GetValue<bool>(dataReader, "IsBellOn"),
                IsBellContinues = SqlDataHelper.GetValue<bool>(dataReader, "IsBellContinues"),
                workModel = new WorkModel
                {
                    IsWorkActiveSun = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSun"),
                    IsWorkActiveFri = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveFri"),
                    IsWorkActiveMon = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveMon"),
                    IsWorkActiveSat = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSat"),
                    IsWorkActiveThurs = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveThurs"),
                    IsWorkActiveTues = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveTues"),
                    IsWorkActiveWed = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveWed"),

                    WorkTextMon = SqlDataHelper.GetValue<string>(dataReader, "WorkTextMon"),
                    WorkTextSat = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSat"),
                    WorkTextSun = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSun"),
                    WorkTextThurs = SqlDataHelper.GetValue<string>(dataReader, "WorkTextThurs"),
                    WorkTextTues = SqlDataHelper.GetValue<string>(dataReader, "WorkTextTues"),
                    WorkTextWed = SqlDataHelper.GetValue<string>(dataReader, "WorkTextWed"),
                    WorkTextFri = SqlDataHelper.GetValue<string>(dataReader, "WorkTextFri"),

                    StartDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSat"),
                    StartDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateFri"),
                    StartDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateMon"),
                    StartDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSun"),
                    StartDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateThurs"),
                    StartDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateTues"),
                    StartDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateWed"),


                    EndDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateFri"),
                    EndDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateMon"),
                    EndDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSat"),
                    EndDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSun"),
                    EndDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateThurs"),
                    EndDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateTues"),
                    EndDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateWed"),

                    HasSPSat = SqlDataHelper.GetValue<bool>(dataReader, "HasSPSat"),
                    HasSPSun = SqlDataHelper.GetValue<bool>(dataReader, "HasSPSun"),
                    HasSPMon = SqlDataHelper.GetValue<bool>(dataReader, "HasSPMon"),
                    HasSPTues = SqlDataHelper.GetValue<bool>(dataReader, "HasSPTues"),
                    HasSPWed = SqlDataHelper.GetValue<bool>(dataReader, "HasSPWed"),
                    HasSPThurs = SqlDataHelper.GetValue<bool>(dataReader, "HasSPThurs"),
                    HasSPFri = SqlDataHelper.GetValue<bool>(dataReader, "HasSPFri"),


                },

                IsEvaluation = SqlDataHelper.GetValue<bool>(dataReader, "IsEvaluation"),
                EvaluationText = SqlDataHelper.GetValue<string>(dataReader, "EvaluationText"),
                EvaluationTime = SqlDataHelper.GetValue<int>(dataReader, "EvaluationTime"),
                D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key"),

                isOrderOffer = SqlDataHelper.GetValue<bool>(dataReader, "isOrderOffer"),
                IsD360Dialog = SqlDataHelper.GetValue<bool>(dataReader, "IsD360Dialog"),

                IsLoyalityPoint = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyalityPoint"),
                Points = SqlDataHelper.GetValue<int>(dataReader, "Points "),

                PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
                Image = SqlDataHelper.GetValue<string>(dataReader, "Image"),
                ImageBg = SqlDataHelper.GetValue<string>(dataReader, "ImageBg"),
                ZohoCustomerId = SqlDataHelper.GetValue<string>(dataReader, "ZohoCustomerId"),
                id = SqlDataHelper.GetValue<string>(dataReader, "id"),
                _rid = SqlDataHelper.GetValue<string>(dataReader, "_rid"),
                _self = SqlDataHelper.GetValue<string>(dataReader, "_self"),
                _etag = SqlDataHelper.GetValue<string>(dataReader, "_etag"),
                _attachments = SqlDataHelper.GetValue<string>(dataReader, "_attachments"),
                _ts = SqlDataHelper.GetValue<int>(dataReader, "_ts"),
                AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken"),
                WhatsAppAccountID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAccountID"),
                WhatsAppAppID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAppID"),
                BotTemplateId = SqlDataHelper.GetValue<int>(dataReader, "BotTemplateId"),
                BIDailyLimit = SqlDataHelper.GetValue<int>(dataReader, "BIDailyLimit"),
                CurrencyCode = SqlDataHelper.GetValue<string>(dataReader, "CurrencyCode"),
                TimeZoneId = SqlDataHelper.GetValue<string>(dataReader, "TimeZoneId"),
                IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder"),
                IsPickup = SqlDataHelper.GetValue<bool>(dataReader, "IsPickup"),
                IsDelivery = SqlDataHelper.GetValue<bool>(dataReader, "IsDelivery"),
                BookingCapacity = SqlDataHelper.GetValue<int>(dataReader, "BookingCapacity"),
                ReminderBookingHour = SqlDataHelper.GetValue<int>(dataReader, "ReminderBookingHour"),
                IsCaution = SqlDataHelper.GetValue<bool>(dataReader, "IsCaution"),
                IsPaidInvoice = SqlDataHelper.GetValue<bool>(dataReader, "IsPaidInvoice"),

                CautionDays = SqlDataHelper.GetValue<int>(dataReader, "CautionDays"),
                WarningDays = SqlDataHelper.GetValue<int>(dataReader, "WarningDays"),
                UnAvailableBookingDates = SqlDataHelper.GetValue<string>(dataReader, "UnAvailableBookingDates"),
                RejectRequestText = SqlDataHelper.GetValue<string>(dataReader, "RejectRequestText"),
                ConfirmRequestText = SqlDataHelper.GetValue<string>(dataReader, "ConfirmRequestText"),

                TimeReminder = SqlDataHelper.GetValue<int>(dataReader, "TimeReminder"),
                MenuReminderMessage = SqlDataHelper.GetValue<string>(dataReader, "MenuReminderMessage"),
                IsActiveMenuReminder = SqlDataHelper.GetValue<bool>(dataReader, "IsActiveMenuReminder"),

                IsInquiry = SqlDataHelper.GetValue<bool>(dataReader, "IsInquiry"),
                IsBotLanguageEn = SqlDataHelper.GetValue<bool>(dataReader, "IsBotLanguageEn"),
                IsBotLanguageAr = SqlDataHelper.GetValue<bool>(dataReader, "IsBotLanguageAr"),
                IsMenuLinkFirst = SqlDataHelper.GetValue<bool>(dataReader, "IsMenuLinkFirst"),
                IsSelectPaymentMethod = SqlDataHelper.GetValue<bool>(dataReader, "IsSelectPaymentMethod"),

                MerchantID= SqlDataHelper.GetValue<string>(dataReader, "MerchantID"),

                FacebookPageId= SqlDataHelper.GetValue<string>(dataReader, "FacebookPageId"),
                FacebookAccessToken= SqlDataHelper.GetValue<string>(dataReader, "FacebookAccessToken"),
                InstagramId= SqlDataHelper.GetValue<string>(dataReader, "InstagramId"),
                InstagramAccessToken= SqlDataHelper.GetValue<string>(dataReader, "InstagramAccessToken")

            };
            return catalogue;
        }







        public static async Task<TenantModel> CheckInvoicesFunction(TenantModel tenantCD)
        {
            try
            {

                accessTokenModel = getZohoAccessToken();
                if (tenantCD.ZohoCustomerId != "")
                {

                    try
                    {

                        var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);


                        var invous = GetAllInvoices(tenantCD.ZohoCustomerId);// status CAUTION!



                        if (invous != null)
                        {
                            var invCAUTION = invous.invoices.Where(x => x.status == "sent").ToList();//InvoicesGet(tenantCD.ZohoCustomerId, "sent");// status CAUTION!

                            if (invCAUTION.Count() > 0)
                            {

                                // var days = ExtractNumber(invCAUTION.invoices[0].due_days);
                                var InvBefore = invCAUTION.Where(x => ExtractNumber(x.due_days) <= tenantCD.CautionDays).ToArray();
                                if (InvBefore.Length > 0)
                                {
                                    addInvoicesHistoryAsync(InvBefore, tenantCD.TenantId, "CAUTION", tenantCD.D360Key);


                                    //tenantCD.IsPaidInvoice=true;
                                    tenantCD.IsCaution = true;
                                }
                                else
                                {

                                    UpdateInvoicesHistoryAsync(tenantCD.ZohoCustomerId, "CAUTION", false, tenantCD.D360Key);//on CAUTION


                                    // tenantCD.IsPaidInvoice=true;
                                    tenantCD.IsCaution = false;
                                }

                            }
                            else
                            {

                                UpdateInvoicesHistoryAsync(tenantCD.ZohoCustomerId, "CAUTION", false, tenantCD.D360Key);//on CAUTION


                                // tenantCD.IsPaidInvoice=true;
                                tenantCD.IsCaution = false;
                            }

                        }





                        var invWARNING = invous.invoices.Where(x => x.status == "overdue").ToList();//InvoicesGet(tenantCD.ZohoCustomerId, "overdue");// status WARNING
                        if (invWARNING.Count() > 0)
                        {
                            //  var days = ExtractNumber(invWARNING.invoices[0].due_days);
                            var bef = invWARNING.Where(x => ExtractNumber(x.due_days) >= tenantCD.WarningDays).ToArray();

                            if (bef.Length > 0)
                            {
                                addInvoicesHistoryAsync(bef, tenantCD.TenantId, "WARNING", tenantCD.D360Key);

                                tenantCD.IsPaidInvoice = false;
                                // tenantCD.IsCaution=true;
                            }
                            else
                            {
                                //addInvoicesHistoryAsync(bef, tenantCD.TenantId, "CAUTION", tenantCD.D360Key);

                                UpdateInvoicesHistoryAsync(tenantCD.ZohoCustomerId, "WARNING", true, tenantCD.D360Key);//on WARNING 

                                tenantCD.IsPaidInvoice = true;
                                tenantCD.IsCaution = true;
                            }

                        }
                        else
                        {
                            UpdateInvoicesHistoryAsync(tenantCD.ZohoCustomerId, "WARNING", true, tenantCD.D360Key);//on WARNING 

                            tenantCD.IsPaidInvoice = true;
                            // tenantCD.IsCaution=true;
                        }






                        //   await itemsCollection.UpdateItemAsync(tenantCD._self, tenantCD);



                        return tenantCD;
                    }
                    catch
                    {

                    }


                }

            }
            catch (Exception ex)
            {



            }

            return new TenantModel() { IsPaidInvoice = true, IsCaution = false };
        }


        private static List<TenantModel> GetTenantList(int id)
        {

            //var x = GetContactId("962779746365", "28");


            string connString = Constant.ConnectionString;
            string query = "select * from [dbo].[AbpTenants] where IsDeleted = 0 and Id = " + id;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<TenantModel> order = new List<TenantModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                order.Add(new TenantModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    ZohoCustomerId = dataSet.Tables[0].Rows[i]["ZohoCustomerId"].ToString(),
                    D360Key = dataSet.Tables[0].Rows[i]["D360Key"].ToString(),
                });

            }

            conn.Close();
            da.Dispose();

            return order;

        }

        public static InvoicesModel InvoicesGet(string ZohoCustomerId, string status)
        {
            var client = new RestClient("https://invoice.zoho.com/api/v3/invoices?customer_id=" + ZohoCustomerId + "&&status=" + status);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessTokenModel.access_token);
            // request.AddHeader("Cookie", "BuildCookie_800001748=1; 0d082fb755=622080e4a2a522d402965665137144e9; JSESSIONID=B6196252D559333C3B0DB909391DCBA8; _zcsr_tmp=436c8351-0058-47a9-ba3d-a270a0d7b3ac; zbcscook=436c8351-0058-47a9-ba3d-a270a0d7b3ac");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new InvoicesModel();
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                RefreshToken();
                return InvoicesGet(ZohoCustomerId, status);
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rez = JsonConvert.DeserializeObject<InvoicesModel>(response.Content);

                rez.invoices = rez.invoices.Where(x => x.status != "draft").ToArray();

                return rez;
            }
            else
            {
                return new InvoicesModel();
            }

        }
        public static InvoicesModel GetAllInvoices(string ZohoCustomerId)
        {
            var client = new RestClient("https://invoice.zoho.com/api/v3/invoices?customer_id=" + ZohoCustomerId);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessTokenModel.access_token);
            //   request.AddHeader("Cookie", "BuildCookie_800001748=1; 0d082fb755=622080e4a2a522d402965665137144e9; JSESSIONID=B6196252D559333C3B0DB909391DCBA8; _zcsr_tmp=436c8351-0058-47a9-ba3d-a270a0d7b3ac; zbcscook=436c8351-0058-47a9-ba3d-a270a0d7b3ac");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new InvoicesModel();
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                RefreshToken();
                return GetAllInvoices(ZohoCustomerId);
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rez = JsonConvert.DeserializeObject<InvoicesModel>(response.Content);

                rez.invoices = rez.invoices.Where(x => x.status != "draft").ToArray();

                return rez;
            }
            else
            {
                return new InvoicesModel();
            }

        }
        private static AccessTokenModel getZohoAccessToken()
        {
            try
            {
                AccessTokenModel liveChatEntity = new AccessTokenModel();
                var SP_Name = "GetZohoAccessToken";

                var sqlParameters = new List<SqlParameter>
                {
                };

                liveChatEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapAccessToken, Constant.ConnectionString).FirstOrDefault();


                return liveChatEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static AccessTokenModel MapAccessToken(IDataReader dataReader)
        {
            try
            {
                AccessTokenModel model = new AccessTokenModel();
                model.access_token = SqlDataHelper.GetValue<string>(dataReader, "access_token");
                model.refresh_token = SqlDataHelper.GetValue<string>(dataReader, "refresh_token");
                model.client_secret = SqlDataHelper.GetValue<string>(dataReader, "client_secret");
                model.client_id = SqlDataHelper.GetValue<string>(dataReader, "client_id");
                model.redirect_uri = SqlDataHelper.GetValue<string>(dataReader, "redirect_uri");
                model.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void RefreshToken()
        {

            var client = new RestClient("https://accounts.zoho.com/oauth/v2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cookie", "_zcsr_tmp=6a67ec77-3364-4b51-b0e8-efa20cdea769; b266a5bf57=a711b6da0e6cbadb5e254290f114a026; iamcsr=6a67ec77-3364-4b51-b0e8-efa20cdea769");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("client_id", accessTokenModel.client_id);
            request.AddParameter("client_secret", accessTokenModel.client_secret);
            request.AddParameter("refresh_token", accessTokenModel.refresh_token);
            request.AddParameter("grant_type", "refresh_token");
            IRestResponse response = client.Execute(request);



            var rez = JsonConvert.DeserializeObject<AccessTokenModel>(response.Content);
            accessTokenModel.access_token = rez.access_token;
            UpdateZohoAccessToken();

        }
        private static void UpdateZohoAccessToken()
        {
            try
            {

                var SP_Name = "ZohoAccessTokenUpdate";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@access_token",accessTokenModel.access_token),
                     new System.Data.SqlClient.SqlParameter("@refresh_token",accessTokenModel.refresh_token),
                     new System.Data.SqlClient.SqlParameter("@expires_in",3600),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constant.ConnectionString);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static async Task addInvoicesHistoryAsync(InvoicesModel.Invoice[] item, int? tenantId, string Status, string D360Key)
        {
            try
            {
                var SP_Name = "InvoicesHistoryAdd";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Month",DateTime.UtcNow.Month)
                    ,new System.Data.SqlClient.SqlParameter("@Year",DateTime.UtcNow.Year)
                    ,new System.Data.SqlClient.SqlParameter("@Status",Status)
                     ,new System.Data.SqlClient.SqlParameter("@InvoicesJson",JsonConvert.SerializeObject(item))
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constant.ConnectionString);

                try
                {
                    var url = Constant.EngineAPIURL + "api/WhatsAppAPI/DeleteCache?phoneNumberId=" + D360Key + "&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges";
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync(url))
                        {
                            var resultrespons = await response.Content.ReadAsStringAsync();
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                            }
                        }
                    }

                }
                catch (Exception ex)
                {


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task UpdateInvoicesHistoryAsync(string customer_id, string Status, bool IsStatus, string D360Key)
        {
            try
            {
                var SP_Name = "InvoicesHistoryUpdate";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@ZohoCustomerId",customer_id)
                    ,new System.Data.SqlClient.SqlParameter("@Status",Status)
                     ,new System.Data.SqlClient.SqlParameter("@IsStatus",IsStatus)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constant.ConnectionString);

                try
                {
                    var url = Constant.EngineAPIURL + "api/WhatsAppAPI/DeleteCache?phoneNumberId=" + D360Key + "&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges";
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync(url))
                        {
                            var resultrespons = await response.Content.ReadAsStringAsync();
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                            }
                        }
                    }

                }
                catch (Exception ex)
                {


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static int ExtractNumber(string str1)
        {
            string str2 = string.Empty;
            int val = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                if (Char.IsDigit(str1[i]))
                    str2 += str1[i];
            }
            if (str2.Length > 0)
                val = int.Parse(str2);
            return val;
        }
    }
}
