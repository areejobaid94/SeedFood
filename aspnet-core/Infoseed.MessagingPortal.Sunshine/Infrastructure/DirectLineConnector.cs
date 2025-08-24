using Infoseed.MessagingPortal.Sunshine.Models;
using LiteDB;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Infrastructure
{
    public  class DirectLineConnector
    {
        private  string directLineSecret = ConfigurationManager.AppSettings["DirectLineSecret"];
        private  string botId = ConfigurationManager.AppSettings["BotId"];
        private  string fromUser = "DirectLineSampleClientUser";
      
        public  async Task StartBotConversation(string? conversationID,string userMsg, SunshineReqInfoModel msgInfo)
        {
            DirectLineClient client = new DirectLineClient(directLineSecret);
            Microsoft.Bot.Connector.DirectLine.Conversation conversation;
            if (conversationID != null)
            {
                conversation= await client.Conversations.ReconnectToConversationAsync(conversationID);
            }
            else
            {
                 conversation = await client.Conversations.StartConversationAsync();
            }

            Microsoft.Bot.Connector.DirectLine.Activity userMessage = new Microsoft.Bot.Connector.DirectLine.Activity
            {
                From = new ChannelAccount(fromUser),
                Text = userMsg,
                Type = ActivityTypes.Message
            };

            var response=  await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);

            await ReadBotMessagesAsync(client, conversation.ConversationId, msgInfo);


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
     
        private  async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId, SunshineReqInfoModel msgInfo)
        {
            string watermark = null;

          
                var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);
                watermark = activitySet?.Watermark;

                var activities = from x in activitySet.Activities
                                 where x.From.Id == botId
                                 select x;

                foreach (Microsoft.Bot.Connector.DirectLine.Activity activity in activities)
                {
                await SunshineConnector.postTextMsgToSmooch(msgInfo.appID, msgInfo.conversationID, activity.Text);
                
                    //Console.WriteLine(activity.Text);

                    if (activity.Attachments != null)
                    {
                        foreach (Attachment attachment in activity.Attachments)
                        {
                            switch (attachment.ContentType)
                            {
                                case "application/vnd.microsoft.card.hero":
                                var heroCards=attachmentToHeroCards(activity.Attachments);
                                await SunshineConnector.postListMsgToSmooch(msgInfo.appID, msgInfo.conversationID, heroCards);
                                //Send All the card as once and return
                                return;
                                //RenderHeroCard(attachment);
                                break;

                                case "image/png":
                                //Console.WriteLine($"Opening the requested image '{attachment.ContentUrl}'");
                                await SunshineConnector.postImageMsgToSmooch(msgInfo.appID, msgInfo.conversationID, activity.Text, attachment.ContentUrl);
                                //Process.Start(attachment.ContentUrl);
                                break;
                            }
                        }
                    }

                    
                

              
            }
        }

        private List<HeroCard> attachmentToHeroCards(IList<Attachment> Attachments)
        {
            List<HeroCard> heroCards = new List<HeroCard>();
            foreach(var attachment in Attachments)
            {
                var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());
                heroCards.Add(heroCard);
            }
            return heroCards;
        }
        private  void RenderHeroCard(Attachment attachment)
        {
            const int Width = 70;
            Func<string, string> contentLine = (content) => string.Format($"{{0, -{Width}}}", string.Format("{0," + ((Width + content.Length) / 2).ToString() + "}", content));

            var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());

            if (heroCard != null)
            {
                Console.WriteLine("/{0}", new string('*', Width + 1));
                Console.WriteLine("*{0}*", contentLine(heroCard.Title));
                Console.WriteLine("*{0}*", new string(' ', Width));
                Console.WriteLine("*{0}*", contentLine(heroCard.Text));
                Console.WriteLine("{0}/", new string('*', Width + 1));
            }
        }


        public async Task<string> CheckIsNewConversation( string sunshineConversationId)
        {
            string result = string.Empty;
            using (var db = new LiteDatabase(@"LiteDb\ConversationDb.db"))
            {
                var conversationChat = db.GetCollection<ConversationChatModel>("conversationChatModel");
                var objConversation = conversationChat.Find(x => x.SunshineConversationId == sunshineConversationId).FirstOrDefault();

                if (objConversation == null)
                {
                    //As Odai Request , for me this shoould be in two way
                    DirectLineClient client = new DirectLineClient(directLineSecret);
                    var conversation = await client.Conversations.StartConversationAsync();
                    var microsoftBotId = conversation.ConversationId;
                    // Create your new conversation instance
                    var conversationChatModel = new ConversationChatModel()
                    {
                        MicrosoftBotId = microsoftBotId,
                        SunshineConversationId = sunshineConversationId,
                        LastUpdate = DateTime.Now,
                    };
                    conversationChat.Insert(conversationChatModel);
                    result = microsoftBotId;
                }
                else
                {
                    result = objConversation.MicrosoftBotId;
                }
            }
            return result;
        }


    }
}
