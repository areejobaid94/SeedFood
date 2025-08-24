using Framework.Data;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal
{
    public class DirectLineBotConnector
    {  //private  string directLineSecret = "fTWdMb3_TWA.DwrVOOa4r4bimpwgi9u6Z00kq5GxAmfgbMvqawty7iY";
        //private  string botId = "infoseedBotRG";
        private string fromUser = "DirectLineSampleClientUser";
        private readonly IDocumentClient _IDocumentClient;


        public DirectLineBotConnector(

                     IDocumentClient iDocumentClient

         )


        {
                    _IDocumentClient = iDocumentClient;

        }
    public async Task<SunshinePostMsgBotModel.Content> StartBotConversation(string conversationID, string userMsg, SunshinePostMsgBotModel.Content msgInfo, string directLineSecret, string botId, string phonenumber, string TenantId, string Location, IList<Microsoft.Bot.Connector.DirectLine.Attachment> attachments = null)
        {
            DirectLineClient client = new DirectLineClient(directLineSecret);
           Conversation conversation;
            if (conversationID != null)
            {
                conversation = await client.Conversations.ReconnectToConversationAsync(conversationID);
            }
            else
            {
                conversation = await client.Conversations.StartConversationAsync();
            }


            Activity userMessage = new Activity
            {
                From = new ChannelAccount(phonenumber, TenantId),
                Text = userMsg,
                Type = ActivityTypes.Message,
                Attachments = attachments,
                InputHint = Location,
                 
            };
            //var eventMessage = Activity.CreateEventActivity();
            ////Wrong way round?!?
            //eventMessage.From = new ChannelAccount(phonenumber, TenantId);
            //eventMessage.Type = ActivityTypes.Event;
            //eventMessage.Value = "join";

            var response = client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage).Result;

            var botMSG = ReadBotMessagesAsync(client, conversation.ConversationId, msgInfo, botId).Result;



            return botMSG;
            //I think this is if the bot support auto reply when the use open the chat , which is not supported in whatsapp
            //new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversation.ConversationId)).Start();

            //Console.Write("Command> ");

            //while (true)
            //{
            //    string input = Console.ReadLine().Trim();

            //    if (input.ToLower() == "exit")
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        if (input.Length > 0)
            //        {
            //            Microsoft.Bot.Connector.DirectLine.Activity userMessage = new Microsoft.Bot.Connector.DirectLine.Activity
            //            {
            //                From = new ChannelAccount(fromUser),
            //                Text = input,
            //                Type = ActivityTypes.Message
            //            };

            //            await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
            //        }
            //    }
            //}

        }

        private async Task<SunshinePostMsgBotModel.Content> ReadBotMessagesAsync(DirectLineClient client, string conversationId, SunshinePostMsgBotModel.Content msgInfo, string botId)
        {
            string watermark = null;


            var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);
            watermark = activitySet?.Watermark;

            var activities = from x in activitySet.Activities
                             where x.From.Id == botId
                             select x;


            var activityLast = activities.Last();
            var list = activities.ToList();

            var beforMasg = "";

            if (activityLast.Text.Contains("EndBot"))
            {
                list.Remove(activityLast);

                var listLastE = list.Last();


                if (listLastE.Text.Contains("SeedBotPDF.pdf"))
                {
                    SunshinePostMsgBotModel.Content message2 = new SunshinePostMsgBotModel.Content()
                    {

                        mediaUrl = listLastE.Text.Replace("\r\n\r\n", ""),
                        type = "file",
                        agentName = botId,
                        agentId = "100",
                        altText = listLastE.Text,
                        fileName = "SeedBotPDF.pdf",


                    };
                    beforMasg = listLastE.Text;
                    //   var result2 = await SunshineConnector.PostMsgToSmooch(msgInfo.appID, msgInfo.conversationID, message2);

                }
                else
                {
                    SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content()
                    {

                        text = listLastE.Text,
                        type = "text",
                        agentName = botId,
                        agentId = "100"


                    };
                    beforMasg = listLastE.Text;
                    //   var result = await SunshineConnector.PostMsgToSmooch(msgInfo.appID, msgInfo.conversationID, message);

                }

            }
            else
            {
                SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content()
                {

                    text = activityLast.Text,
                    type = "text",
                    agentName = botId,
                    agentId = "100"


                };
                //  var result = await SunshineConnector.PostMsgToSmooch(msgInfo.appID, msgInfo.conversationID, message);

            }

            SunshinePostMsgBotModel.Content ContentM = new SunshinePostMsgBotModel.Content()
            {
                altText = beforMasg,
                text = activityLast.Text,
                type = "text",
                agentName = botId,
                agentId = "100"


            };
            //if (activityLast.Attachments != null)
            //{
            //    foreach (Attachment attachment in activityLast.Attachments)
            //    {
            //        switch (attachment.ContentType)
            //        {
            //            case "application/vnd.microsoft.card.hero":
            //                var heroCards = attachmentToHeroCards(activityLast.Attachments);
            //                await SunshineConnector.postListMsgToSmooch(msgInfo.appID, msgInfo.conversationID, heroCards);
            //                //Send All the card as once and return

            //                //RenderHeroCard(attachment);
            //                break;

            //            case "image/png":
            //                //Console.WriteLine($"Opening the requested image '{attachment.ContentUrl}'");
            //                await SunshineConnector.postImageMsgToSmooch(msgInfo.appID, msgInfo.conversationID, activityLast.Text, attachment.ContentUrl);
            //                //Process.Start(attachment.ContentUrl);
            //                break;
            //        }
            //    }
            //}



            return ContentM;
            //foreach (Microsoft.Bot.Connector.DirectLine.Activity activity in activities)
            //    {
            //    Content message = new Content() { 

            //         text = activity.Text,
            //          type="text",


            //    };


            //    await SunshineConnector.PostMsgToSmooch(msgInfo.appID, msgInfo.conversationID, message);

            //        //Console.WriteLine(activity.Text);

            //        if (activity.Attachments != null)
            //        {
            //            foreach (Attachment attachment in activity.Attachments)
            //            {
            //                switch (attachment.ContentType)
            //                {
            //                    case "application/vnd.microsoft.card.hero":
            //                    var heroCards=attachmentToHeroCards(activity.Attachments);
            //                    await SunshineConnector.postListMsgToSmooch(msgInfo.appID, msgInfo.conversationID, heroCards);
            //                    //Send All the card as once and return
            //                    return;
            //                    //RenderHeroCard(attachment);
            //                    break;

            //                    case "image/png":
            //                    //Console.WriteLine($"Opening the requested image '{attachment.ContentUrl}'");
            //                    await SunshineConnector.postImageMsgToSmooch(msgInfo.appID, msgInfo.conversationID, activity.Text, attachment.ContentUrl);
            //                    //Process.Start(attachment.ContentUrl);
            //                    break;
            //                }
            //            }
            //        }





            //}
        }

        //private List<HeroCard> attachmentToHeroCards(IList<Microsoft.Bot.Connector.DirectLine.Attachment> Attachments)
        //{
        //    List<HeroCard> heroCards = new List<HeroCard>();
        //    foreach (var attachment in Attachments)
        //    {
        //        var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());
        //        heroCards.Add(heroCard);
        //    }
        //    return heroCards;
        //}
        //private void RenderHeroCard(Microsoft.Bot.Connector.DirectLine.Attachment attachment)
        //{
        //    const int Width = 70;
        //    Func<string, string> contentLine = (content) => string.Format($"{{0, -{Width}}}", string.Format("{0," + ((Width + content.Length) / 2).ToString() + "}", content));

        //    var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());

        //    if (heroCard != null)
        //    {
        //        Console.WriteLine("/{0}", new string('*', Width + 1));
        //        Console.WriteLine("*{0}*", contentLine(heroCard.Title));
        //        Console.WriteLine("*{0}*", new string(' ', Width));
        //        Console.WriteLine("*{0}*", contentLine(heroCard.Text));
        //        Console.WriteLine("{0}/", new string('*', Width + 1));
        //    }
        //}


        public async Task<string> CheckIsNewConversation(string sunshineConversationId, string directLineSecret, string appID, string userId, string BotId)
        {


            string result = string.Empty;

            var conversationChat = new DocumentCosmoseDB<ConversationChatBotModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.SunshineConversationId == sunshineConversationId).Result;

            if (objConversation == null)
            {
                //As Odai Request , for me this shoould be in two way
                DirectLineClient client = new DirectLineClient(directLineSecret);
                var conversation = await client.Conversations.StartConversationAsync();
                var microsoftBotId = conversation.ConversationId;
                // Create your new conversation instance
                await conversationChat.CreateItemAsync(new ConversationChatBotModel()
                {
                    BotId = BotId,
                    MicrosoftBotId = microsoftBotId,
                    SunshineConversationId = sunshineConversationId,
                    LastUpdate = DateTime.Now,
                    ItemType = InfoSeedContainerItemTypes.ConversationBot,
                    appID = appID,
                    userId = userId
                });
                result = microsoftBotId;
            }
            else
            {
                result = objConversation.MicrosoftBotId;
            }

            return result;
        }

        public async Task<string> DeleteConversation(string sunshineConversationId)
        {


            string result = string.Empty;

            var conversationChat = new DocumentCosmoseDB<ConversationChatBotModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.SunshineConversationId == sunshineConversationId).Result;

            if (objConversation != null)
            { // delete contact caht 

                var queryString = "SELECT * FROM c WHERE c.ItemType= 3 and  c.SunshineConversationId= '" + sunshineConversationId + "'";
                await conversationChat.DeleteChatItem(queryString);

            }


            return result;
        }


    }
}
