using Framework.Data;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using LiteDB;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Sunshine
{
    public  class DirectLineConnector
    {
        private readonly IDocumentClient _IDocumentClient;

      public  DirectLineConnector(IDocumentClient iDocumentClient)
        {
            _IDocumentClient = iDocumentClient; ;
        }

        private TelemetryClient _telemetry;
        public async Task<List<Microsoft.Bot.Connector.DirectLine.Activity>> StartBotConversation(string conversationID, string userMsg,string directLineSecret, string botId, string phonenumber, string TenantId, string name, string watermark,IList<Microsoft.Bot.Connector.DirectLine.Attachment> attachments = null)
        {
            DirectLineClient client = new DirectLineClient(directLineSecret);
            Microsoft.Bot.Connector.DirectLine.Conversation conversation;
            if (conversationID != null)
            {
                conversation=  client.Conversations.ReconnectToConversationAsync(conversationID).Result;
            }
            else
            {
                 conversation =  client.Conversations.StartConversationAsync().Result;
            }

            Microsoft.Bot.Connector.DirectLine.Activity userMessage = new Microsoft.Bot.Connector.DirectLine.Activity
            { 
                From = new ChannelAccount(phonenumber+","+ name, TenantId),
                Text = userMsg,
                Type = ActivityTypes.Message,
                 Locale= name,
                Attachments = attachments
            };

            try
            {

                // send message to bot 
                var response = client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage).Result;
                this._telemetry.TrackTrace("send message to bot, the   ConversationId :"+ conversation.ConversationId + " and the message is :" + userMessage, SeverityLevel.Information);
            }
            catch(Exception ex)
            {

                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
            }
          


           
            //read bot response
            var botMSG =  ReadBotMessagesAsync(client, conversation.ConversationId, botId, watermark, conversationID).Result;

            return botMSG;               
        }
        public async Task<List<Microsoft.Bot.Connector.DirectLine.Activity>> StartBotConversationD360(string userID,string contcatID,string conversationID, string userMsg, string directLineSecret, string botId, string phonenumber, string TenantId, string name, string TenantPhoneNumber, string isOrderOffer, string watermark, IList<Microsoft.Bot.Connector.DirectLine.Attachment> attachments = null,int? BotTemplateId=1, string interactiveId="")
        {
            DirectLineClient client = new DirectLineClient(directLineSecret);
            Microsoft.Bot.Connector.DirectLine.Conversation conversation;
            if (conversationID != null)
            {
                conversation =  client.Conversations.ReconnectToConversationAsync(conversationID).Result;
            }
            else
            {
             
                conversation =  client.Conversations.StartConversationAsync().Result;
            }

            if (TenantPhoneNumber == null)
            {
                TenantPhoneNumber = "";
            }

            Microsoft.Bot.Connector.DirectLine.Activity userMessage = new Microsoft.Bot.Connector.DirectLine.Activity
            {
                From = new ChannelAccount(phonenumber.Trim(),name.Trim()),
                Text = userMsg,
                Type = ActivityTypes.Message,
                Locale =   contcatID.Trim()+ "," + TenantId.Trim() + "," + isOrderOffer.Trim() + "," + TenantPhoneNumber.Trim() +","+ TenantId.Trim()+"_"+ phonenumber.Trim()+","+BotTemplateId.ToString()+","+ interactiveId,
                Attachments = attachments
            };
            // send message to bot 
            var response =  client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage).Result;

            //read bot response
            var botMSG =  ReadBotMessagesD360Async(client, conversation.ConversationId, botId, watermark, conversationID, "").Result;

            return botMSG;
        }

        public async Task<InfoSeedAzureFunction.MicrosoftBotInfo> CheckIsNewConversation(string sunshineConversationId, string directLineSecret, string appID, string userId, string BotId)
        {
            var userid = userId;
            try
            {

                if (userId.Split("_")[0] == "32" || userId.Split("_")[0] == "31")
                {
                    userid = "22_" + userId.Split("_")[1];

                }
            }
            catch
            {


            }

            InfoSeedAzureFunction.MicrosoftBotInfo result = new InfoSeedAzureFunction.MicrosoftBotInfo();

            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.SunshineConversationId == sunshineConversationId).Result;

            if (objConversation == null)
            {
                //As Odai Request , for me this shoould be in two way
                DirectLineClient client = new DirectLineClient(directLineSecret);
                var conversation =  client.Conversations.StartConversationAsync().Result;
                var microsoftBotId = conversation.ConversationId;
                // Create your new conversation instance
                var x= conversationChat.CreateItemAsync(new ConversationChatModel()
                {
                    BotId = BotId,
                    MicrosoftBotId = microsoftBotId,
                    SunshineConversationId = sunshineConversationId,
                    LastUpdate = DateTime.Now,
                    ItemType = InfoSeedContainerItemTypes.ConversationBot,
                    appID = appID,
                    userId = userid,
                    watermark = null,
                }).Result;
                result.MicrosoftBotId = microsoftBotId;
                result.watermark = null;
            }
            else
            {
                result.MicrosoftBotId = objConversation.MicrosoftBotId;
                result.watermark = objConversation.watermark;

            }

            return result;
        }

        public async Task<InfoSeedAzureFunction.MicrosoftBotInfo> CheckIsNewConversationD360(string D360Key, string directLineSecret, string userId, string BotId)
        {
            var userid = userId;
            try
            {

                if (userId.Split("_")[0] == "32" || userId.Split("_")[0] == "31")
                {
                    userid = "22_" + userId.Split("_")[1];

                }
            }
            catch
            {


            }



            InfoSeedAzureFunction.MicrosoftBotInfo result = new InfoSeedAzureFunction.MicrosoftBotInfo();

            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.userId == userid).Result;

            if (objConversation == null)
            {
                //As Odai Request , for me this shoould be in two way
                DirectLineClient client = new DirectLineClient(directLineSecret);
                var conversation =  client.Conversations.StartConversationAsync().Result;
                var microsoftBotId = conversation.ConversationId;
                // Create your new conversation instance
                var x= conversationChat.CreateItemAsync(new ConversationChatModel()
                {
                    BotId = BotId,
                    MicrosoftBotId = microsoftBotId,
                    SunshineConversationId = "",
                    LastUpdate = DateTime.Now,
                    ItemType = InfoSeedContainerItemTypes.ConversationBot,
                    D360Key = D360Key,
                    appID = "",
                    userId = userid,
                    watermark = null,
                }).Result;
                result.MicrosoftBotId = microsoftBotId;
                result.watermark = null;
            }
            else
            {
                result.MicrosoftBotId = objConversation.MicrosoftBotId;
                result.watermark = objConversation.watermark;

            }

            return result;
        }

        private async Task<List<Microsoft.Bot.Connector.DirectLine.Activity>> ReadBotMessagesAsync(DirectLineClient client, string conversationId, string botId, string watermark, string MicrosoftBotId)
        {

            var activitySet =  client.Conversations.GetActivitiesAsync(conversationId, watermark).Result;
            watermark = activitySet?.Watermark;

            // update the watermark(number of step activities)
            UpdateConversation(MicrosoftBotId, watermark);

            var activities = from x in activitySet.Activities
                             where x.From.Id == botId
                             select x;

            return activities.ToList();
        }
        private async Task<List<Microsoft.Bot.Connector.DirectLine.Activity>> ReadBotMessagesD360Async(DirectLineClient client, string conversationId, string botId, string watermark, string MicrosoftBotId, string D360Key)
        {

            var activitySet =  client.Conversations.GetActivitiesAsync(conversationId, watermark).Result;
            watermark = activitySet?.Watermark;

            // update the watermark(number of step activities)
            UpdateConversation(MicrosoftBotId, watermark);

            var activities = from x in activitySet.Activities
                             where x.From.Id == botId
                             select x;

            return activities.ToList();
        }

        private async void UpdateConversation(string sunshineConversationId, string watermark)
        {
            try
            {
                InfoSeedAzureFunction.MicrosoftBotInfo result = new InfoSeedAzureFunction.MicrosoftBotInfo();

                var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.MicrosoftBotId == sunshineConversationId).Result;

                if (objConversation != null)
                {
                    objConversation.watermark = watermark;

                    var xx = conversationChat.UpdateItemAsync(objConversation._self, objConversation).Result;
                }
               

            }
            catch(Exception ex)
            {
                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
            }
            

        }


        public async Task<List<Microsoft.Bot.Connector.DirectLine.Activity>> StartBotConversationD360E(string userID, string contcatID, string conversationID, string userMsg, string directLineSecret, string botId, string phonenumber, string TenantId, string name, string TenantPhoneNumber, string isOrderOffer, string watermark, IList<Microsoft.Bot.Connector.DirectLine.Attachment> attachments = null)
        {
            DirectLineClient client = new DirectLineClient(directLineSecret);
            Microsoft.Bot.Connector.DirectLine.Conversation conversation;
            if (conversationID != null)
            {
                conversation = client.Conversations.ReconnectToConversationAsync(conversationID).Result;
            }
            else
            {

                conversation = client.Conversations.StartConversationAsync().Result;
            }

            if (TenantPhoneNumber == null)
            {
                TenantPhoneNumber = "";
            }

            var list = TenantId.Split(",");

            Microsoft.Bot.Connector.DirectLine.Activity userMessage = new Microsoft.Bot.Connector.DirectLine.Activity
            {
                From = new ChannelAccount(phonenumber.Trim(),name.Trim()),
                Text = userMsg,
                Type = ActivityTypes.Message,
                Locale = list[0].Trim()+","+ list[1].Trim()+"," + list[2].Trim()+"," + list[3].Trim() + "," + TenantId.Trim() + "_" + phonenumber.Trim() ,
                Attachments = attachments
            };
            // send message to bot 
            var response = client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage).Result;

            //read bot response
            var botMSG = ReadBotMessagesD360Async(client, conversation.ConversationId, botId, watermark, conversationID, "").Result;

            return botMSG;
        }

    }
}
