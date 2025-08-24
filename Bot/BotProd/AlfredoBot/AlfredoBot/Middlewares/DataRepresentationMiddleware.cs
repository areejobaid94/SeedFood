using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using info_seed_bot.Extensions;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace info_seed_bot.Middlewares
{
    public class DataRepresentationMiddleware  : IMiddleware
    {
        private static Dictionary<string, Dictionary<int, string>> conversationOptions = new Dictionary<string, Dictionary<int, string>>();
        private static Dictionary<string, DateTime> conversationOptionsLastActivity = new Dictionary<string, DateTime>();

        public Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken)
        {
            turnContext.Activity.Locale = string.IsNullOrWhiteSpace(turnContext.Activity.Locale) ? "en-us" : turnContext.Activity.Locale;
            if (turnContext.Activity.Type == ActivityTypes.Message &&
                ((turnContext.IsTelegramChannel() && turnContext.Activity.Text == "/start") ||
                  turnContext.Activity.Text == "resetbot"))
            {
                turnContext.Activity.Code = EndOfConversationCodes.CompletedSuccessfully;
                turnContext.Activity.Text = "hi";
                var msg = turnContext.Activity.CreateReply();
                msg.AsEndOfConversationActivity();
                turnContext.SendActivityAsync(msg, cancellationToken);
            }

            if (CheckIfInputValidForOptions(turnContext, out int intNumber) && intNumber != -1)
            {
                turnContext.Activity.Text = conversationOptions[turnContext.Activity.Conversation.Id][intNumber];
            }

            turnContext.OnSendActivities(HandleSendActivities);
            turnContext.OnUpdateActivity(HandleUpdateActivity);
            turnContext.OnDeleteActivity(HandleDeleteActivity);
            return next(cancellationToken);
        }

        private static bool CheckIfInputValidForOptions(ITurnContext turnContext, out int intNumber)
        {
            intNumber = -1;
            return turnContext.Activity.Type == ActivityTypes.Message &&
                            conversationOptions.ContainsKey(turnContext.Activity.Conversation.Id) &&
                            int.TryParse(turnContext.Activity.Text, out intNumber) && intNumber < 100
                            && conversationOptions[turnContext.Activity.Conversation.Id].ContainsKey(intNumber);
        }

        private static void ValidateOptionsHistory(string conversationId)
        {
            if (!conversationOptionsLastActivity.ContainsKey(conversationId))
            {
                conversationOptionsLastActivity.Add(conversationId, DateTime.Now);
            }
            else
            {
                conversationOptionsLastActivity[conversationId] = DateTime.Now;
            }

            if (conversationOptionsLastActivity.Count > 0
                && conversationOptionsLastActivity.Any(x => (DateTime.Now - x.Value).Hours > 1))
            {
                var list = new List<string>();
                foreach (var item in conversationOptionsLastActivity.Where(x => (DateTime.Now - x.Value).Hours > 1).Select(x => x.Key).ToList())
                {
                    conversationOptions.Remove(item);
                    list.Add(item);
                }

                conversationOptionsLastActivity = conversationOptionsLastActivity.Where(x => !list.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);
                list.Clear();
            }
        }

        private Task<ResourceResponse[]> HandleSendActivities(ITurnContext turnContext, List<Activity> activities, Func<Task<ResourceResponse[]>> next)
        {
            if (activities != null && activities.Count > 0)
            {
                if (conversationOptions.ContainsKey(turnContext.Activity.Conversation.Id) &&
                    activities.Any(x => x.Type == ActivityTypes.Message))
                {
                    conversationOptions.Remove(turnContext.Activity.Conversation.Id);
                }

                var optionlst = new Dictionary<int, string>();

                foreach (var activity in activities)
                {
                    if (activity.Type == ActivityTypes.Message)
                    {
                        if (turnContext.IsFacebookChannel())
                        {
                            // we need to change the adaptive card buttons to suggested actions in case facebook or telegram
                            // (note: the max suggested actions number is 11 and the max character for text in each suggested action is 20)

                            if (string.IsNullOrEmpty(activity.Text))
                            {
                                activity.Text = null;
                            }

                            var tOutText = string.Empty;
                            List<CardAction> tOutActions = null;
                            List<Attachment> tAttachmentsToRemove = null;
                            if (HandleActivity(activity, out tOutText, out tOutActions, out tAttachmentsToRemove))
                            {
                                tAttachmentsToRemove.ForEach((att) =>
                                {
                                    activity.Attachments.Remove(att);
                                });

                                if (activity.SuggestedActions != null && activity.SuggestedActions.Actions != null && activity.SuggestedActions.Actions.Count > 0)
                                {
                                    tOutActions.AddRange(activity.SuggestedActions.Actions);
                                }

                                //if (turnContext.IsFacebookChannel())
                                //{
                                //    foreach (var item in tOutActions)
                                //    {
                                //        item.DisplayText = item.Text;
                                //        item.Type = "messageBack";

                                //        var listOfIgnoredTexts = new List<string> { "Back", "Main Menu", "Load More", "Cancel" };
                                //        if (!listOfIgnoredTexts.Contains(item.DisplayText))
                                //        {
                                //            tOutText += "\n" + "- **" + item.DisplayText + "**";
                                //        }
                                //    }
                                //}

                                activity.Text = tOutText;

                                activity.SuggestedActions = new SuggestedActions()
                                {
                                    Actions = tOutActions
                                };
                            }
                        }
                        else if (turnContext.IsWhatsappChannel())
                        {
                            string tMessageToSend = string.Empty;
                            int tOrder = 1;

                            if (string.IsNullOrEmpty(activity.Text))
                            {
                                activity.Text = null;
                            }

                            var tOutText = string.Empty;
                            List<CardAction> tOutActions = null;
                            List<Attachment> tAttachmentsToRemove = null;

                            if (HandleActivity(activity, out tOutText, out tOutActions, out tAttachmentsToRemove))
                            {
                                //tAttachmentsToRemove.ForEach((att) =>
                                //{
                                //    activity.Attachments.Remove(att);
                                //});

                                if (activity.SuggestedActions != null && activity.SuggestedActions.Actions != null && activity.SuggestedActions.Actions.Count > 0)
                                {
                                    tOutActions.AddRange(activity.SuggestedActions.Actions);
                                }

                                //int count = 1;
                                foreach (var hc in tOutActions)
                                {
                                    hc.Value=hc.Value.ToString().Trim();
                                    tMessageToSend += tOrder.ToString() + "- " + hc.Title + "\r\n";
                                    optionlst.Add(tOrder, hc.Title);
                                    tOrder++;
                                }

                                if (activity.SuggestedActions == null && tOutActions.Count() > 0)
                                {
                                    activity.SuggestedActions = new SuggestedActions
                                    {
                                        Actions = tOutActions,
                                    };
                                }

                                activity.Text = tOutText.Replace("\\n\\r", "\r\n").Replace("\r\n\r\n\r\n\r\n\r\n\r\n", string.Empty); //+ activity.Summary; // "\r\n" + tMessageToSend + "\r\n" + activity.Summary;
                            }
                        }
                    }
                }

                if (optionlst.Count > 0)
                {
                    conversationOptions.Add(turnContext.Activity.Conversation.Id, optionlst);
                    ValidateOptionsHistory(turnContext.Activity.Conversation.Id);
                }
            }

            return next();
        }

        private bool HandleActivity(Activity activity, out string text, out List<CardAction> cardActions, out List<Attachment> attachmentsToRemove)
        {
            text = string.IsNullOrEmpty(activity.Text) ? string.Empty : activity.Text + "\r\n";
            cardActions = new List<CardAction>();
            attachmentsToRemove = new List<Attachment>();

            if (activity.Attachments != null && activity.Attachments.Count > 0)
            {
                foreach (var attachment in activity.Attachments)
                {
                    bool isRemovable;
                    string tAttOutText;
                    List<CardAction> tAttOutActions;

                    if (HandleAttachement(attachment, out tAttOutText, out tAttOutActions, out isRemovable))
                    {
                        if (isRemovable)
                        {
                            attachmentsToRemove.Add(attachment);
                        }

                        text += tAttOutText + "\r\n";
                        if (tAttOutActions != null && tAttOutActions.Count > 0)
                        {
                            cardActions.AddRange(tAttOutActions);
                        }
                    }
                }
            }

            return true;
        }

        private bool HandleAttachement(Attachment attachment, out string text, out List<CardAction> cardActions, out bool removable)
        {
            removable = true;
            text = string.Empty;
            cardActions = new List<CardAction>();

            if (attachment.ContentType == "application/vnd.microsoft.card.adaptive")
            {
                if (attachment.Content.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    var tRet = false;
                    var tContentObj = (Newtonsoft.Json.Linq.JObject)attachment.Content;
                    var tBody = tContentObj["body"];
                    string tOutBodyText;
                    List<CardAction> tOutBodyActions;
                    if (HandleAttachementBody(tBody, out tOutBodyText, out tOutBodyActions, out removable))
                    {
                        text += tOutBodyText + "\r\n";
                        if (tOutBodyActions != null && tOutBodyActions.Count > 0)
                        {
                            cardActions.AddRange(tOutBodyActions);
                        }

                        tRet = true;
                    }

                    var tActionsObj = ((Newtonsoft.Json.Linq.JObject)attachment.Content)["actions"];
                    string tOutActionsText;
                    List<CardAction> tOutActionsActions;
                    if (HandleAttachementActions(tActionsObj, out tOutActionsText, out tOutActionsActions))
                    {
                        text += tOutActionsText + "\r\n";
                        if (tOutActionsActions != null && tOutActionsActions.Count > 0)
                        {
                            cardActions.AddRange(tOutActionsActions);
                        }

                        tRet = true;
                    }

                    return tRet;
                }
            }
            else if (attachment.ContentType == "application/pdf")
            {
                cardActions.Add(new CardAction()
                {
                    Type = ActionTypes.DownloadFile,
                    Text = attachment.ContentUrl as string,
                    Title = attachment.ContentUrl as string,
                    Value = attachment.ContentUrl as string,
                    DisplayText = attachment.ContentUrl as string
                });
                text += attachment.ContentUrl;
                return true;
            }
            else if (attachment.ContentType == "application/vnd.microsoft.card.hero")
            {
                var heroCard = attachment.Content as HeroCard;
                var suggestedActions = new List<CardAction>();
                heroCard.Buttons.ToList().ForEach(hc =>
                {
                    suggestedActions.Add(new CardAction()
                    {
                        Type = ActionTypes.ImBack,
                        Text = hc.Text,
                        Title = hc.Title,
                        Value = hc.Value,
                        DisplayText = hc.DisplayText
                    });
                });

                text += heroCard.Text;
                cardActions = suggestedActions;
                return true;
            }

            return false;
        }

        private bool HandleAttachementBody(Newtonsoft.Json.Linq.JToken body, out string text, out List<CardAction> cardActions, out bool removable)
        {
            var tRet = false;
            text = string.Empty;
            removable = true;
            cardActions = new List<CardAction>();
            if (body != null)
            {
                foreach (var ibodyObj in body)
                {
                    if (ibodyObj.SelectToken("parseOnRepresentation") != null)
                    {
                        removable = (bool)((Newtonsoft.Json.Linq.JValue)ibodyObj["parseOnRepresentation"]).Value;
                    }

                    var type = ((Newtonsoft.Json.Linq.JValue)ibodyObj["type"]).Value as string;
                    string tOutText;
                    List<CardAction> tOutCardActions;
                    if (HandleAttachmentBodyItem(ibodyObj, out tOutText, out tOutCardActions))
                    {
                        text += tOutText + "\r\n";
                        if (tOutCardActions != null && tOutCardActions.Count > 0)
                        {
                            cardActions.AddRange(tOutCardActions);
                        }

                        tRet = true;
                    }
                }
            }

            return tRet;
        }

        private bool HandleAttachmentBodyItem(Newtonsoft.Json.Linq.JToken bodyItem, out string text, out List<CardAction> cardActions)
        {
            // TODO : Add other types of inputs
            text = string.Empty;
            cardActions = new List<CardAction>();
            var type = (((Newtonsoft.Json.Linq.JValue)bodyItem["type"]).Value as string).ToLower();
            if (type == "textblock")
            {
                text += bodyItem["text"] + "\r\n";
                return true;
            }

            if (type == "input.toggle")
            {
                var titleString = (string)bodyItem["title"];
                text += titleString + "\r\n";

                cardActions.Add(new CardAction()
                {
                    Type = ActionTypes.MessageBack,
                    Text = "Agree",
                    Title = "Agree",
                    DisplayText = "Agree"
                });
                cardActions.Add(new CardAction()
                {
                    Type = ActionTypes.MessageBack,
                    Text = "Disagree",
                    Title = "Disagree",
                    DisplayText = "Disagree"
                });
                return true;
            }

            if (type == "input.choiceset")
            {
                var tJObj = (Newtonsoft.Json.Linq.JObject)bodyItem;
                if (tJObj["choices"] != null &&
                    tJObj["placeholder"] != null)
                {
                    text += (((Newtonsoft.Json.Linq.JValue)tJObj["placeholder"]).Value as string) + "\r\n";

                    var choices = tJObj["choices"];

                    foreach (var iChoice in choices)
                    {
                        cardActions.Add(new CardAction()
                        {
                            Type = ActionTypes.ImBack,
                            Text = ((Newtonsoft.Json.Linq.JValue)iChoice["title"]).Value as string,
                            Title = ((Newtonsoft.Json.Linq.JValue)iChoice["title"]).Value as string,
                            DisplayText = ((Newtonsoft.Json.Linq.JValue)iChoice["title"]).Value as string,
                            Value = ((Newtonsoft.Json.Linq.JValue)iChoice["title"]).Value as string
                        });
                    }
                }

                return true;
            }
            else if (type == "container")
            {
                var items = bodyItem["items"];
                foreach (var iItem in items)
                {
                    string tOutText;
                    List<CardAction> tOutActions;
                    if (HandleAttachmentBodyItem(iItem, out tOutText, out tOutActions))
                    {
                        text += tOutText + "\r\n";
                        if (tOutActions != null && tOutActions.Count > 0)
                        {
                            cardActions.AddRange(tOutActions);
                        }
                    }
                }

                return true;
            }
            else if (type == "colum")
            {
                var items = bodyItem["columns"];
                foreach (var iItem in items)
                {
                    string tOutText;
                    List<CardAction> tOutActions;
                    if (HandleAttachmentBodyItem(iItem, out tOutText, out tOutActions))
                    {
                        text += tOutText + "\r\n";
                        if (tOutActions != null && tOutActions.Count > 0)
                        {
                            cardActions.AddRange(tOutActions);
                        }
                    }
                }

                return true;
            }
            else if (type == "columnset")
            {
                var items = bodyItem["columns"];
                foreach (var iItem in items)
                {
                    foreach (var columnItem in iItem["items"])
                    {
                        text += columnItem["text"] + "\r\n";
                    }
                }

                return true;
            }

            if (type == "textblock1")
            {
                text += bodyItem["text"] + "\r\n";
                return true;
            }

            return false;
        }

        private bool HandleAttachementActions(Newtonsoft.Json.Linq.JToken actions, out string text, out List<CardAction> cardActions)
        {
            var tRet = false;
            text = string.Empty;
            cardActions = new List<CardAction>();

            if (actions != null)
            {
                foreach (var tObj in actions)
                {
                    var type = (((Newtonsoft.Json.Linq.JValue)tObj["type"]).Value as string).ToLower();
                    if (type == "action.submit")
                    {
                        if (tObj.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                        {
                            var tJObj = (Newtonsoft.Json.Linq.JObject)tObj;
                            if (tObj["data"] != null &&
                                tObj["title"] != null)
                            {
                                string data = string.Empty;

                                if (tObj["data"].GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                                {
                                    data = ((Newtonsoft.Json.Linq.JValue)tObj["data"]["action"]).Value as string;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(data))
                                    {
                                        data = ((Newtonsoft.Json.Linq.JValue)tObj["data"]).Value as string;
                                    }
                                }

                                cardActions.Add(new CardAction()
                                {
                                    Type = ActionTypes.ImBack,
                                    Text = ((Newtonsoft.Json.Linq.JValue)tObj["title"]).Value as string,
                                    Title = ((Newtonsoft.Json.Linq.JValue)tObj["title"]).Value as string,
                                    Value = data,
                                    DisplayText = ((Newtonsoft.Json.Linq.JValue)tObj["title"]).Value as string
                                });
                            }
                        }

                        tRet = true;
                    }
                    else if (type == "action.toggle")
                    {
                        cardActions.Add(new CardAction()
                        {
                            Type = ActionTypes.MessageBack,
                            Text = "Agree",
                            Title = "Agree",
                            DisplayText = "Agree"
                        });
                        cardActions.Add(new CardAction()
                        {
                            Type = ActionTypes.ImBack,
                            Text = "Disagree",
                            Title = "Disagree",
                            DisplayText = "Disagree"
                        });
                        tRet = true;
                    }
                    else if (type == "action.openurl")
                    {
                        text = string.Concat(text, "\n\r", ((Newtonsoft.Json.Linq.JValue)tObj["title"])?.Value as string, "\n\r", ((Newtonsoft.Json.Linq.JValue)tObj["url"])?.Value as string);
                        tRet = true;
                    }
                }
            }

            return tRet;
        }

        private Task<ResourceResponse> HandleUpdateActivity(ITurnContext turnContext, Activity activity, Func<Task<ResourceResponse>> next)
        {
            return next();
        }

        private Task HandleDeleteActivity(ITurnContext turnContext, ConversationReference reference, Func<Task> next)
        {
            return next();
        }
    }
}
