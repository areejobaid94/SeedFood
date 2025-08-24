
using InfoSeedAzureFunction.WhatsAppApi;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{
    public class EvaluationQuestion
    {
        private TelemetryClient _telemetry;
        private IHubContext<TeamInboxHub> _hub;
        public EvaluationQuestion(
            TelemetryClient telemetry,
            IHubContext<TeamInboxHub> hub)
        {
            _telemetry = telemetry;
            _hub = hub;
        }

        //public async Task<string> evaluationQuestion()
        //{


        //    var tenants = GetTenantList();

        //    foreach(var tenantDB in tenants)
        //    {
        //        var orderList = GetOrderList(tenantDB.TenantId.Value);

        //       var tenant=GetTenantById(tenantDB.TenantId.Value).Result;

        //        foreach (var order in orderList)
        //        {
        //            var user = GetCustomerAzuer(order.ContactId.ToString());
        //            var time = DateTime.Now;
        //            var timeAdd = time.AddHours(Constant.AddHour);

        //            TimeSpan timeSpan = timeAdd - order.ActionTime;
        //            int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);
        //            if (totalMinutes >= tenant.EvaluationTime && totalMinutes <= tenant.EvaluationTime+5  && tenant.IsBundleActive)
        //            {
        //               // var Bot = directLineConnector.StartBotConversationD360E(user.userId, order.ContactId.ToString(), micosoftConversationID.MicrosoftBotId, "EvaluationQuestion", tenant.DirectLineSecret, tenant.botId, user.phoneNumber, order.OrderNumber.ToString() + "," + tenant.TenantId + "," + tenant.EvaluationText + "," + order.OrderLocal, user.displayName, tenant.PhoneNumber, tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, null, tenant.BotTemplateId).Result;

        //                user.customerChat.text="EvaluationQuestion";
        //                user.CustomerStepModel.OrderNumber=order.OrderNumber;
        //                user.CustomerStepModel.OrderId=order.Id;
        //                user.CustomerStepModel.EvaluationQuestionText=tenant.EvaluationText;
        //                updateOrder(order.Id);
        //                SendToRestaurantsBot(user);
                      
        //            }

        //        }

        //    }

        //    return null;
        //}

        //private async Task SendToRestaurantsBot(CustomerModel jsonData)
        //{
        //    try
        //    {
        //        var constra = JsonConvert.SerializeObject(jsonData);
        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Post, Constants.BotApi +"api/RestaurantsChatBot/RestaurantsMessageHandler");
        //        request.Headers.Add("accept", "*/*");
        //        var content = new StringContent(constra
        //            , null, "application/json-patch+json");
        //        request.Content = content;
        //        var response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //        //Console.WriteLine(await response.Content.ReadAsStringAsync());
        //    }
        //    catch
        //    {

        //    }




        //}
        //private static List<TenantModel> GetTenantList()
        //{

        //    //var x = GetContactId("962779746365", "28");


        //    string connString = Constant.ConnectionString;
        //    string query = "select * from [dbo].[AbpTenants] where IsDeleted = 0 and botId = 'InfoBotRestaurantsSTG' and IsActive=1 and IsPaidInvoice <> 0 and IsEvaluation=1 and AccessToken <> '' and AccessToken is not null";
        //   // string query = "select * from [dbo].[AbpTenants] where IsDeleted = 0 and botId = 'InfoRestaurantsBot' and IsActive=1 and IsPaidInvoice <> 0 and IsEvaluation=1 and AccessToken <> '' and AccessToken is not null";


        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    List<TenantModel> Tenant = new List<TenantModel>();

        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {

        //        Tenant.Add(new TenantModel
        //        {
        //            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
        //            ZohoCustomerId = dataSet.Tables[0].Rows[i]["ZohoCustomerId"].ToString(),
        //            D360Key = dataSet.Tables[0].Rows[i]["D360Key"].ToString(),
        //            CautionDays = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CautionDays"]),
        //            WarningDays = Convert.ToInt32(dataSet.Tables[0].Rows[i]["WarningDays"]),
        //             AccessToken= dataSet.Tables[0].Rows[i]["AccessToken"].ToString(),
        //        });

        //    }

        //    conn.Close();
        //    da.Dispose();

        //    return Tenant;

        //}


        //private async Task<TenantModel> GetTenantById(int? id)
        //{
           
        //    var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);

        //    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.Tenant && a.TenantId == id);
        //    return tenant;
        //}

        //private  List<Order> GetOrderList(int TenantId)
        //{

        //    //var x = GetContactId("962779746365", "28");


        //    string connString =  Constant.ConnectionString;
        //    string query = "select * from [dbo].[Orders] where IsEvaluation = 0 and ( OrderStatus=2 OR OrderStatus=7 ) and TenantId="+TenantId+" and ActionTime >= GETDATE()- 3 and IsEvaluation=0";


        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();

        //    // create the DataSet 
        //    DataSet dataSet = new DataSet();

        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to your datatable
        //    da.Fill(dataSet);

        //    List<Order> order = new List<Order>();

        //    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //    {
        //        try
        //        {
        //            order.Add(new Order
        //            {
        //                Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
        //                OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
        //                Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
        //                ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
        //                CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
        //                OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
        //                orderStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), dataSet.Tables[0].Rows[i]["orderStatus"].ToString(), true),
        //                TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
        //                IsEvaluation = bool.Parse(dataSet.Tables[0].Rows[i]["IsEvaluation"].ToString()),
        //                 OrderLocal= dataSet.Tables[0].Rows[i]["OrderLocal"].ToString(),
        //                  ActionTime= Convert.ToDateTime(dataSet.Tables[0].Rows[i]["ActionTime"].ToString()),
        //            });
        //        }
        //        catch
        //        {

        //            //order.Add(new Order
        //            //{
        //            //    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
        //            //    OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
        //            //    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
        //            //    ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
        //            //    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
        //            //    OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
        //            //    orderStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), dataSet.Tables[0].Rows[i]["orderStatus"].ToString(), true),
        //            //    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
        //            //    IsEvaluation = bool.Parse(dataSet.Tables[0].Rows[i]["IsEvaluation"].ToString()),
        //            //});
        //        }
               

        //    }

        //    conn.Close();
        //    da.Dispose();

        //    return order;

        //}


        //private static CustomerModel GetCustomerAzuer(string contactID)
        //{
        //    int count = 1;
        //    string result = string.Empty;
        //    var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
        //    var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.CustomerItem && a.ContactID == contactID);
        //    if (customerResult.IsCompletedSuccessfully)
        //    {
        //        return customerResult.Result;

        //    }


        //    return null;
        //}
        //private  void updateOrder(long Id)
        //{

        //    string connString =  Constant.ConnectionString;
        //    using (SqlConnection connection = new SqlConnection(connString))
        //    using (SqlCommand command = connection.CreateCommand())
        //    { 
        //        command.CommandText = "UPDATE Orders SET  IsEvaluation = @IsEva  Where Id = @Id";
        //        command.Parameters.AddWithValue("@Id", Id);
        //        command.Parameters.AddWithValue("@IsEva", true);

        //        connection.Open();
        //        command.ExecuteNonQuery();
        //        connection.Close();
        //    }
        //}
    }
}
