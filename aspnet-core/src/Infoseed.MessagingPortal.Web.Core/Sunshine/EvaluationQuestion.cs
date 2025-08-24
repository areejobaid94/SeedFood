using Framework.Data;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.WhatsAppDialog;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Sunshine
{
    public class EvaluationQuestion
    {
        private TelemetryClient _telemetry;
        private IHubContext<TeamInboxHub> _hub;
        IDBService _dbService;
        private readonly IDocumentClient _IDocumentClient;

        public EvaluationQuestion(
            TelemetryClient telemetry,
             IDBService dbService,
            IHubContext<TeamInboxHub> hub
            , IDocumentClient iDocumentClient
            )
        {
            _telemetry = telemetry;
            _dbService = dbService;
            _hub = hub;
            _IDocumentClient = iDocumentClient;

        }

        public async Task<string> evaluationQuestion()
        {

            var orderList = GetOrderList().Where(x=>x.orderStatus== OrderStatusEunm.Done || x.orderStatus == OrderStatusEunm.Pre_Order).ToList();

            foreach(var order in orderList)
            {


                if(order.OrderLocal=="" || order.OrderLocal==null)
                {
                    order.OrderLocal = "ar";
                }
           
                var tenant = GetTenantById(order.TenantId).Result;

                if(tenant.IsEvaluation)
                {
                    var contact = GetCustomer(order.ContactId);
                    var user = GetCustomerAzuer(contact.UserId);
                    var time = DateTime.Now;







                    if (order.SelectDay == null)
                    {

                        var timeAdd = time.AddHours(AppSettingsModel.AddHour);

                        TimeSpan timeSpan = timeAdd - order.CreationTime;
                        int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);

                        if (totalMinutes >= tenant.EvaluationTime && totalMinutes <= tenant.EvaluationTime+5 && !order.IsEvaluation)
                        {

                            var DeleteBotChat = DeleteConversationD360(user.userId).Result;
                            DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
                            var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(user.D360Key, tenant.DirectLineSecret, user.userId, tenant.botId).Result;
                            var Bot = directLineConnector.StartBotConversationD360E(contact.UserId, order.ContactId.ToString(), micosoftConversationID.MicrosoftBotId, "EvaluationQuestion", tenant.DirectLineSecret, tenant.botId, contact.PhoneNumber, order.OrderNumber.ToString() + "," + tenant.TenantId + "," + tenant.EvaluationText + "," + order.OrderLocal, user.displayName, tenant.PhoneNumber, tenant.isOrderOffer.ToString(), null, null).Result;



                            SendWhatsAppD360Model sendWhatsAppD360Model = ListInt(Bot.FirstOrDefault(), contact.PhoneNumber);

                            try
                            {
                                var result = await WhatsAppDialogConnector.PostMsgToSmooch(user.D360Key, sendWhatsAppD360Model, _telemetry);

                                if (result == HttpStatusCode.Created)
                                {
                                    Content message = contentM(Bot.FirstOrDefault(), tenant.botId);
                                    updateOrder(order.Id);
                                    //update Bot massage in cosmoDB 
                                    var CustomerChat = _dbService.UpdateCustomerChatD360(contact.TenantId, message, user.userId, user.ConversationId);
                                    user.CreateDate = CustomerChat.CreateDate;
                                    user.customerChat = CustomerChat;
                                    await _hub.Clients.All.SendAsync("brodCastEndUserMessage", user);

                                }
                            }
                            catch
                            {


                            }
                          

                        }
                    }
                    //else
                    //{
                    //    /// if pre order
                       
                    //    var St = order.SelectDay;

                    //    int pFrom = St.IndexOf("(") + "(".Length;
                    //    int pTo = St.LastIndexOf(")");

                    //    var result = St.Substring(pFrom, pTo - pFrom);
                    //    var cc = result.Split("/");
                    //    var timeNow = cc[1] + "/" + cc[0] + "/" + time.Year;

                    //    DateTime DateEvaluation = Convert.ToDateTime(timeNow);



                    //    DateTime resultDateEvaluation = new DateTime(DateEvaluation.Year, DateEvaluation.Month, DateEvaluation.Day);
                    //    DateTime resultDateNow= new DateTime(time.Year, time.Month, time.Day);

                    //    if (resultDateEvaluation == resultDateNow)
                    //    {
                    //        DateTime dateTime = DateTime.ParseExact(order.SelectTime, "h:mm tt", CultureInfo.InvariantCulture);

                    //        var timeAdd = time.AddHours(3);

                    //        TimeSpan timeSpan = timeAdd - dateTime;
                    //        int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);

                    //        if (totalMinutes >= 120 && !order.IsEvaluation)
                    //        {

                    //            var DeleteBotChat = DeleteConversationD360(user.userId).Result;
                    //            DirectLineConnector directLineConnector = new DirectLineConnector();
                    //            var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(user.D360Key, tenant.DirectLineSecret, user.userId, tenant.botId).Result;
                    //            var Bot = directLineConnector.StartBotConversationD360(contact.UserId, order.ContactId.ToString(), micosoftConversationID.MicrosoftBotId, "EvaluationQuestion", tenant.DirectLineSecret, tenant.botId, contact.PhoneNumber, order.OrderNumber.ToString() + "," + tenant.TenantId + "," + tenant.EvaluationText, user.displayName, null, null).Result;



                    //            SendWhatsAppD360Model sendWhatsAppD360Model = ListInt(Bot.FirstOrDefault(), contact.PhoneNumber);


                    //            var resultt = await WhatsAppDialogConnector.PostMsgToSmooch(user.D360Key, sendWhatsAppD360Model, _telemetry);

                    //            if (resultt == HttpStatusCode.Created)
                    //            {
                    //                Content message = contentM(Bot.FirstOrDefault(), tenant.botId);
                    //                updateOrder(order.Id);
                    //                //update Bot massage in cosmoDB 
                    //                var CustomerChat = _dbService.UpdateCustomerChatD360(contact.TenantId, message, user.userId, user.SunshineConversationId);
                    //                user.CreateDate = CustomerChat.CreateDate;
                    //                user.customerChat = CustomerChat;
                    //                await _hub.Clients.All.SendAsync("brodCastEndUserMessage", user);

                    //            }

                    //        }

                    //    }


                    //}



                }
             
            }


            return null;
        }

        private SendWhatsAppD360Model  ListInt(Microsoft.Bot.Connector.DirectLine.Activity msgBot ,  string PhoneNumber)
        {

            List<SendWhatsAppD360Model.Section> sections = new List<SendWhatsAppD360Model.Section>();
            List<SendWhatsAppD360Model.Row> rows = new List<SendWhatsAppD360Model.Row>();
            foreach (var button in msgBot.SuggestedActions.Actions)
            {
                rows.Add(new SendWhatsAppD360Model.Row
                {
                    id = button.Title,
                    title = button.Title

                });


            }

            sections.Add(new SendWhatsAppD360Model.Section
            {
                title = "Rate",
                rows = rows.ToArray()
            });

            SendWhatsAppD360Model sendWhatsAppD360Model = new SendWhatsAppD360Model
            {
                to = PhoneNumber,
                type = "interactive",
                interactive = new SendWhatsAppD360Model.Interactive
                {
                    type = "list",
                    // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                    body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                    footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                    action = new SendWhatsAppD360Model.Action
                    {
                        button = "Rate List",
                        sections = sections.ToArray()

                    }



                }

            };

            return sendWhatsAppD360Model;
        }


        private Content contentM(Microsoft.Bot.Connector.DirectLine.Activity msgBot, string botId)
        {
            string tMessageToSend = string.Empty;
            List<CardAction> tOutActions = new List<CardAction>();
            int tOrder = 1;
            var optionlst = new Dictionary<int, string>();
            if (msgBot.SuggestedActions != null && msgBot.SuggestedActions.Actions.Count > 0)
            {
                tOutActions.AddRange(msgBot.SuggestedActions.Actions);
            }

            foreach (var hc in tOutActions)
            {
                tMessageToSend += tOrder.ToString() + "- " + hc.Title + "\r\n";
                optionlst.Add(tOrder, hc.Title);
                tOrder++;
            }

            Content message = new Content
            {
                text = msgBot.Text + "\r\n" + tMessageToSend,
                type = "text",
                agentName = botId,
                agentId = "1000000"

            };

            return message;

        }
        private async Task<string> DeleteConversation(string sunshineConversationId)
        {


            string result = string.Empty;

            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.SunshineConversationId == sunshineConversationId).Result;

            if (objConversation != null)
            { // delete contact caht 

                var queryString = "SELECT * FROM c WHERE c.ItemType= 3 and  c.SunshineConversationId= '" + sunshineConversationId + "'";
                await conversationChat.DeleteChatItem(queryString);

            }


            return result;
        }
        private async Task<string> DeleteConversationD360(string userId)
        {


            string result = string.Empty;

            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.userId == userId).Result;

            if (objConversation != null)
            { // delete contact caht 

                var queryString = "SELECT * FROM c WHERE c.ItemType= 3 and  c.userId= '" + userId + "'";
                await conversationChat.DeleteChatItem(queryString);

            }


            return result;
        }

        private async Task<TenantModel> GetTenantById(int? id)
        {
           
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }

        private  List<Order> GetOrderList()
        {

            //var x = GetContactId("962779746365", "28");


            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where IsEvaluation = 0";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Order> order = new List<Order>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                try
                {
                    order.Add(new Order
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                        Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                        ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                        CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                        OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                        orderStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), dataSet.Tables[0].Rows[i]["orderStatus"].ToString(), true),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        IsEvaluation = bool.Parse(dataSet.Tables[0].Rows[i]["IsEvaluation"].ToString()),
                         OrderLocal= dataSet.Tables[0].Rows[i]["OrderLocal"].ToString()
                    });
                }
                catch
                {

                    //order.Add(new Order
                    //{
                    //    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    //    OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                    //    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    //    ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                    //    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                    //    OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                    //    orderStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), dataSet.Tables[0].Rows[i]["orderStatus"].ToString(), true),
                    //    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    //    IsEvaluation = bool.Parse(dataSet.Tables[0].Rows[i]["IsEvaluation"].ToString()),
                    //});
                }
               

            }

            conn.Close();
            da.Dispose();

            return order;

        }

        private  Contact GetCustomer(int? ContactId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Contacts] where Id=" + ContactId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Contact contact = new Contact();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                contact = new Contact
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString()
                };
            }

            conn.Close();
            da.Dispose();

            return contact;
        }

        private  CustomerModel GetCustomerAzuer(string UserId)
        {
            string result = string.Empty;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && UserId != null && a.userId == UserId);
            if (customerResult.IsCompletedSuccessfully)
            {
                return customerResult.Result;

            }


            return null;
        }
        private  void updateOrder(long Id)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            { 
                command.CommandText = "UPDATE Orders SET  IsEvaluation = @IsEva  Where Id = @Id";
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@IsEva", true);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
