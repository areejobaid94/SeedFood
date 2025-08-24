using Abp.Application.Services.Dto;
using Abp.Notifications;
using Abp.Runtime.Session;
using DocumentFormat.OpenXml.Bibliography;
using Framework.Data;
using Hangfire.Storage;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.FacebookDTO.DTO;
using Infoseed.MessagingPortal.Genders;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.LiveChat;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.Azure.Documents;
using Nancy.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Sunshine
{

    public class CosmosDBService : IDBService
    {
        private readonly IDocumentClient _IDocumentClient;
        private readonly IGeneralAppService _IGeneralAppService;


        public CosmosDBService()
        {



        }


        public CosmosDBService(IDocumentClient iDocumentClient,
            IGeneralAppService iGeneralAppService = null)
        {

            _IDocumentClient = iDocumentClient;
            _IGeneralAppService = iGeneralAppService;

        }
       // public CosmosDBService( IDocumentClient iDocumentClient)
       // {

       //     _IDocumentClient = iDocumentClient;

       //}
        //update Customer  
        public CustomerModel CheckCustomerD360(WebHookD360Model model, int? TenantId, string D360Key)
        {
            string result = string.Empty;
            string userId = TenantId + "_" + model.messages.FirstOrDefault().from;
            string displayName = model.contacts.FirstOrDefault().profile.name;
            string phoneNumber = model.messages.FirstOrDefault().from;
            string type = model.messages.FirstOrDefault().type;
            string mediaUrl = model.messages.FirstOrDefault().mediaUrl;

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;

                if (customer == null)
                {
                    var cont = new ContactDto()
                    {
                        UserId = userId,
                        DisplayName = displayName,
                        AvatarUrl = "",
                        CreatorUserId = 1,
                        Description = "",
                        EmailAddress = "",
                        IsDeleted = false,
                        CreationTime = DateTime.Now,
                        IsLockedByAgent = false,
                        IsConversationExpired = false,
                        IsBlock = false,
                        IsOpen = false,
                        LockedByAgentName = "",
                        PhoneNumber = phoneNumber,
                        Website = "",
                        TenantId = TenantId,
                        DeletionTime = null,
                        DeleterUserId = null,
                        loyalityPoint=0,
                        TotalOrder=0,
                        TakeAwayOrder=0,
                        DeliveryOrder=0, 
                        ConversationsCount=1
                        //,ConversationId = model.statuses != null ? model.statuses.FirstOrDefault().conversation.id : null


                    };

                    //create customer in DB
                    //var Idcont= InsertContact(cont);
                    var idcont = _IGeneralAppService.CreateContact(cont);
                    if (string.IsNullOrEmpty(idcont.Group))
                    {
                        idcont.Group="0";
                        idcont.GroupName="";
                    }
                    var CustomerModel = new CustomerModel()
                    {
                        ConversationsCount = 0,
                        DeliveryOrder =0,
                        TakeAwayOrder=0,
                        TotalOrder=0,
                        loyalityPoint=0,
                        ContactID= idcont.Id.ToString(),
                        userId = userId,
                        displayName = displayName,
                        avatarUrl = "",
                        type = type,
                        CreateDate = DateTime.Now,
                        IsLockedByAgent = false,
                        IsBlock = false,
                        IsConversationExpired = false,
                        CustomerChatStatusID = (int)CustomerChatStatus.Active,
                        CustomerStatusID = (int)CustomerStatus.Active,
                        LastMessageData = DateTime.Now,
                        IsNew = true,
                        TenantId = TenantId,
                        phoneNumber = phoneNumber,
                        UnreadMessagesCount = 1,
                        IsNewContact = true,
                        D360Key= D360Key,
                        GroupId=long.Parse(idcont.Group),
                        GroupName=idcont.GroupName
                        //  ConversationId = model.statuses.FirstOrDefault().conversation.id
                    };
                    var customerChat2 = UpdateCustomerChatD360(model, TenantId, 1);

                    CustomerModel.CreateDate = customerChat2.CreateDate;
                    CustomerModel.customerChat = customerChat2;

                    //create customer in azuer
                    var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;

                  

                    return CustomerModel;
                }
                else
                {

                    customer.IsNewContact = false;
                    customer.D360Key = D360Key;
                    customer.IsConversationExpired = false;
                 //   customer.ConversationId = model.statuses.FirstOrDefault().conversation.id;

                    TimeSpan timeSpan = DateTime.Now - customer.LastMessageData.Value;
                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                    if (totalHours >= 24 && customer.customerChat.sender==MessageSenderType.Customer)
                    {
                        customer.ConversationsCount = customer.ConversationsCount+1;
                    }
                  


                    customer.LastMessageData = DateTime.Now;


                    if (customer.IsOpen)
                    {
                        customer.UnreadMessagesCount = 0;
                    }
                    else
                    {
                        customer.UnreadMessagesCount++;
                    }

                   
                    //if is blocked 
                    if (customer.IsBlock)
                    {
                        CustomerChat customerCh = new CustomerChat
                        {
                            notificationsText = "The Customer Blocked By : Admin",
                            userId = userId,
                            sender = MessageSenderType.TeamInbox,
                            type = "notification",
                            CreateDate = DateTime.Now,
                            text = "The Customer Blocked By : Admin",
                            UnreadMessagesCount = 1,
                            TenantId = TenantId,
                            status = (int)Messagestatus.New,
                            mediaUrl = mediaUrl
                        };

                        if (customer.customerChat.text == "The Customer Blocked By : Admin")
                        {
                            customer.CreateDate = customerCh.CreateDate;
                            customer.customerChat = customerCh;
                            return customer;
                        }

                        var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
                        var customerChat2 = UpdateCustomerWithChat(customerCh);
                        customer.customerChat = customerChat2;
                        return customer;

                    }
                    else
                    {
                        var customerChat = UpdateCustomerChatD360(model, TenantId, customer.UnreadMessagesCount);
                        customer.customerChat = customerChat;
                        var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;

                        return customer;
                    }

                }

            }

            return null;

        }
       
        public CustomerModel CheckIsNewCustomerWithBotD360(WebHookD360Model model, string botID, int? TenantId,string D360Key)
        {
            //int count = 1;
            string result = string.Empty;
            string userId = TenantId + "_" + model.messages.FirstOrDefault().from;
            string displayName = model.contacts.FirstOrDefault().profile.name;
            string phoneNumber = model.messages.FirstOrDefault().from;
            string type = model.messages.FirstOrDefault().type;
            string mediaUrl = model.messages.FirstOrDefault().mediaUrl;

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem  && a.userId == userId );//&& a.TenantId== TenantId
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer == null)
                {

                    var cont = new ContactDto()
                    {
                        loyalityPoint = 0,
                        TotalOrder = 0,
                        TakeAwayOrder = 0,
                        DeliveryOrder = 0,
                        UserId = userId,
                        DisplayName = displayName,
                        AvatarUrl = "",
                        CreatorUserId = 1,
                        Description = "",
                        EmailAddress = "",
                        IsDeleted = false,
                        CreationTime = DateTime.Now,
                        IsLockedByAgent = false,
                        IsConversationExpired = false,
                        IsBlock = false,
                        IsOpen = false,
                        LockedByAgentName = "",
                        PhoneNumber = phoneNumber,
                        SunshineAppID = "",// model.app._id,
                        //ConversationId = model.statuses.FirstOrDefault().conversation.id,
                        Website = "",
                        TenantId = TenantId,
                        DeletionTime = null,
                        DeleterUserId = null,
                         ConversationsCount=1,
                        // ConversationId = model.statuses.FirstOrDefault().conversation.id


                    };

                   //var idcont = InsertContact(cont);
                    var idcont = _IGeneralAppService.CreateContact(cont);
                    if (string.IsNullOrEmpty(idcont.Group))
                    {
                        idcont.Group="0";
                        idcont.GroupName="";
                    }

                    var CustomerModel = new CustomerModel()
                    {
                        ConversationsCount = 0,
                        ContactID = idcont.Id.ToString(),
                        IsComplaint = false,
                        userId = userId,
                        displayName = displayName,
                        avatarUrl = "",
                        type = type,
                        D360Key = D360Key,
                        CreateDate = DateTime.Now,
                        IsLockedByAgent = false,
                        LockedByAgentName = botID,
                        IsOpen = false,
                        agentId = 100000,
                        IsBlock = false,
                        IsConversationExpired = false,
                        CustomerChatStatusID = (int)CustomerChatStatus.Active,
                        CustomerStatusID = (int)CustomerStatus.Active,
                        LastMessageData = DateTime.Now,
                        IsNew = true,
                        TenantId = TenantId,
                        phoneNumber = phoneNumber,
                        UnreadMessagesCount = 1,
                        IsNewContact = true,
                        IsBotChat = true,
                        IsBotCloseChat = false,
                        loyalityPoint = 0,
                        TotalOrder = 0,
                        TakeAwayOrder = 0,
                        DeliveryOrder = 0,

                        GroupId=long.Parse(idcont.Group),
                        GroupName=idcont.GroupName
                        // ConversationId = model.statuses.FirstOrDefault().conversation.id
                    };

                    var customerChat2 = UpdateCustomerChatD360(model, TenantId, 1);
                    CustomerModel.customerChat = customerChat2;

                    var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;
         
                    return CustomerModel;

                }
                else
                {
                    
                    customer. IsNewContact = false;
                    customer.IsNew = false;
                   // customer.IsOpen = false;
                    customer.UnreadMessagesCount = 1;
                    customer.IsConversationExpired = false;
                    //  customer.ConversationId = model.statuses.FirstOrDefault().conversation.id;

                    if (customer.expiration_timestamp!=0)
                    {
                        var diff = customer.expiration_timestamp-customer.creation_timestamp;


                        var offsetcreation = DateTimeOffset.FromUnixTimeSeconds(customer.creation_timestamp);
                        DateTime creationDate = offsetcreation.UtcDateTime;

                        var offsetexpiration = DateTimeOffset.FromUnixTimeSeconds(customer.expiration_timestamp);
                        DateTime expirationDate = offsetexpiration.UtcDateTime;


                        TimeSpan timediff = expirationDate - creationDate;
                        int totalHoursforuser = (int)(timediff.TotalHours);


                        if (DateTime.UtcNow<=expirationDate)
                        {
                            //customer.IsConversationExpired = false;
                        }
                        else
                        {
                            //customer.ConversationsCount = customer.ConversationsCount + 1;
                            //customer.IsConversationExpired = true;
                        }


                    }
                    else
                    {
                        TimeSpan timeSpan = DateTime.Now - customer.LastConversationStartDateTime.Value;
                        int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                        if (totalHours <= 24)
                        {
                            //customer.IsConversationExpired = false;
                        }
                        else
                        {
                            // item.IsOpen = false;
                            //customer.IsConversationExpired = true;
                           // customer.ConversationsCount = customer.ConversationsCount + 1;
                        }

                    }

                    //customer
                    // customer.ConversationId = model.statuses.FirstOrDefault().conversation.id;
                    customer.LastMessageData = DateTime.Now;
                    if (customer.IsBlock)
                    {
                        CustomerChat customerCh = new CustomerChat
                        {
                            notificationsText = "The Customer Blocked By : Admin",
                            userId = userId,
                            sender = MessageSenderType.TeamInbox,
                            type = "notification",
                            CreateDate = DateTime.Now,
                            text = "The Customer Blocked By : Admin",
                            UnreadMessagesCount = 1,
                            TenantId = TenantId,
                            status = (int)Messagestatus.New,
                            mediaUrl = mediaUrl
                        };

                        if (customer.customerChat.text == "The Customer Blocked By : Admin")
                        {
                            customer.customerChat = customerCh;
                            return customer;
                        }

                        var customerChat2 = UpdateCustomerWithChat(customerCh);
                        customer.customerChat = customerChat2;

                        var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
                       
                        return customer;
                    }
                    else
                    {

                        var customerChat = UpdateCustomerChatD360(model, TenantId, 1);
                        customer.customerChat = customerChat;

                        var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
                        return customer;
                     
                    }

                }

            }
            else
            {
                return null;
            }

        }


        public CustomerModel CreateNewCustomer(string from,string name,string type, string botID, int TenantId, string D360Key)
        {
            //int count = 1;
           
            string userId = TenantId + "_" + from;
            string displayName = name;
            string phoneNumber = from;
    
        

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);


            var cont = new ContactDto()
                    {
                        loyalityPoint = 0,
                        TotalOrder = 0,
                        TakeAwayOrder = 0,
                        DeliveryOrder = 0,
                        UserId = userId,
                        DisplayName = displayName,
                        AvatarUrl = "avatar3",
                        CreatorUserId = 1,
                        Description = "",
                        EmailAddress = "",
                        IsDeleted = false,
                        CreationTime = DateTime.Now,
                        IsLockedByAgent = false,
                        IsConversationExpired = false,
                        IsBlock = false,
                        IsOpen = false,
                        LockedByAgentName = "",
                        PhoneNumber = phoneNumber,
                        SunshineAppID = "",// model.app._id,
                        //ConversationId = model.statuses.FirstOrDefault().conversation.id,
                        Website = "",
                        TenantId = TenantId,
                        DeletionTime = null,
                        DeleterUserId = null,
                        ConversationsCount = 1,
                        // ConversationId = model.statuses.FirstOrDefault().conversation.id
                        channel="Whatsapp"

            };

                    //var idcont = InsertContact(cont);
                      var idcont = _IGeneralAppService.CreateContact(cont);
            if (string.IsNullOrEmpty(idcont.Group))
            {
                idcont.Group="0";
                idcont.GroupName="";
            }
            var UserParmeter = new Dictionary<string, string>();
            UserParmeter.Add("Name", displayName);
            UserParmeter.Add("PhoneNumber", phoneNumber);
            UserParmeter.Add("Location", "No Location");
            UserParmeter.Add("ContactID", idcont.Id.ToString());
            UserParmeter.Add("TenantId", TenantId.ToString());

            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId=D360Key,
                ConversationsCount = 0,
                ContactID = idcont.Id.ToString(),
                IsComplaint = false,
                userId = userId,
                displayName = displayName,
                avatarUrl = "avatar3",
                type = type,
                D360Key = D360Key,
                CreateDate = DateTime.Now,
                IsLockedByAgent = false,
                LockedByAgentName = botID,
                IsOpen = false,
                agentId = 100000,
                IsBlock = false,
                IsConversationExpired = false,
                CustomerChatStatusID = (int)CustomerChatStatus.Active,
                CustomerStatusID = (int)CustomerStatus.Active,
                LastMessageData = DateTime.Now,
                IsNew = true,
                TenantId = TenantId,
                phoneNumber = phoneNumber,
                UnreadMessagesCount = 1,
                IsNewContact = true,
                IsBotChat = true,
                IsBotCloseChat = false,
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                customerChat = new CustomerChat()
                {
                    CreateDate = DateTime.Now,
                },
                creation_timestamp= 0,
                expiration_timestamp= 0,
                // ConversationId = model.statuses.FirstOrDefault().conversation.id
                CustomerStepModel=new CustomerStepModel() { ChatStepId=-1, ChatStepPervoiusId=0, IsLiveChat=false ,UserParmeter=UserParmeter },
                OneTimeQuestionIds=new Dictionary<string, string>(),

                GroupId=long.Parse(idcont.Group),
                GroupName=idcont.GroupName,
                 IsHumanhandover=false,
                channel="whatsapp"

            };

                    

                    var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;
            //CustomerModel._self = Result.Uri;
            //CustomerModel._rid = Result.ID;





            var itemsCollection2 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;
            return CustomerModel;

        }
        public CustomerModel CreateNewCustomerFacebook(string from, string name, string type, string botID, int TenantId, string D360Key,string avatarUrl="", string Gender = "")
        {
            string userId = TenantId + "_" + from;  // Facebook sender_id as the unique user ID
            string displayName = name;
            string messengerId = from; // Facebook user ID (sender_id)






            if (string.IsNullOrEmpty(displayName))
            {
                displayName=messengerId;
            }

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = userId,
                DisplayName = displayName,
                AvatarUrl = "",
                CreatorUserId = 1,
                Description = "",
                EmailAddress = "",
                IsDeleted = false,
                CreationTime = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired = false,
                IsBlock = false,
                IsOpen = false,
                LockedByAgentName = "",
                PhoneNumber = messengerId, // Facebook does not use phone numbers in the same way as WhatsApp
                SunshineAppID = "",
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1,
                 channel="facebook"
            };

            // Create the contact (for Facebook)
            var idcont = _IGeneralAppService.CreateContact(cont);
            if (string.IsNullOrEmpty(idcont.Group))
            {
                idcont.Group = "0";
                idcont.GroupName = "";
            }

            var UserParmeter = new Dictionary<string, string>();
            UserParmeter.Add("Name", displayName);
            UserParmeter.Add("Location", "No Location");
            UserParmeter.Add("ContactID", idcont.Id.ToString());
            UserParmeter.Add("TenantId", TenantId.ToString());

            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId = D360Key,
                ConversationsCount = 0,
                ContactID = idcont.Id.ToString(),
                IsComplaint = false,
                userId = userId,
                displayName = displayName,
                avatarUrl = avatarUrl,
                type = type,
                D360Key = D360Key,
                CreateDate = DateTime.Now,
                IsLockedByAgent = false,
                LockedByAgentName = botID,
                IsOpen = false,
                agentId = 100000,
                IsBlock = false,
                IsConversationExpired = false,
                CustomerChatStatusID = (int)CustomerChatStatus.Active,
                CustomerStatusID = (int)CustomerStatus.Active,
                LastMessageData = DateTime.Now,
                IsNew = true,
                TenantId = TenantId,
                phoneNumber = messengerId, // Not applicable for Facebook, unless you want to store it
                UnreadMessagesCount = 1,
                IsNewContact = true,
                IsBotChat = true,
                IsBotCloseChat = false,
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                customerChat = new CustomerChat()
                {
                    CreateDate = DateTime.Now,
                },
                creation_timestamp =  int.Parse( DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                expiration_timestamp =   int.Parse(DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString()),
                CustomerStepModel = new CustomerStepModel()
                {
                    ChatStepId = -1,
                    ChatStepPervoiusId = 0,
                    IsLiveChat = false,
                    UserParmeter = UserParmeter
                },
                OneTimeQuestionIds = new Dictionary<string, string>(),
                GroupId = long.Parse(idcont.Group),
                GroupName = idcont.GroupName,
                IsHumanhandover = false,
                channel="facebook",
                gender=Gender
            };
            CustomerModel.IsConversationExpired=false;
            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;

            // Retrieve the customer information from Cosmos DB
            var itemsCollection2 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;
            return CustomerModel;
        }
        public CustomerModel CreateNewCustomerInstagram(string from, string name, string type, string botID, int TenantId, string D360Key, InstagramUserInfoModel instagramUserInfoModel)
        {
            string userId = TenantId + "_" + from;  // Facebook sender_id as the unique user ID
            string displayName = name;
            string InstagramId = from; // Facebook user ID (sender_id)

            if (string.IsNullOrEmpty(displayName))
            {
                displayName = InstagramId;
            }

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = userId,
                DisplayName = displayName,
                AvatarUrl = instagramUserInfoModel.profile_pic,
                CreatorUserId = 1,
                Description = "",
                EmailAddress = "",
                IsDeleted = false,
                CreationTime = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired = false,
                IsBlock = false,
                IsOpen = false,
                LockedByAgentName = "",
                PhoneNumber = InstagramId, 
                SunshineAppID = "",
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1,
                channel = "instagram"
            };

            var idcont = _IGeneralAppService.CreateContact(cont);
            if (string.IsNullOrEmpty(idcont.Group))
            {
                idcont.Group = "0";
                idcont.GroupName = "";
            }

            var UserParmeter = new Dictionary<string, string>();
            UserParmeter.Add("Name", displayName);
            UserParmeter.Add("Location", "No Location");
            UserParmeter.Add("ContactID", idcont.Id.ToString());
            UserParmeter.Add("TenantId", TenantId.ToString());

            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId = D360Key,
                ConversationsCount = 0,
                ContactID = idcont.Id.ToString(),
                IsComplaint = false,
                userId = userId,
                displayName = displayName,
                avatarUrl = instagramUserInfoModel.profile_pic,
                type = type,
                D360Key = D360Key,
                CreateDate = DateTime.Now,
                IsLockedByAgent = false,
                LockedByAgentName = botID,
                IsOpen = false,
                agentId = 100000,
                IsBlock = false,
                IsConversationExpired = false,
                CustomerChatStatusID = (int)CustomerChatStatus.Active,
                CustomerStatusID = (int)CustomerStatus.Active,
                LastMessageData = DateTime.Now,
                IsNew = true,
                TenantId = TenantId,
                phoneNumber = InstagramId, 
                UnreadMessagesCount = 1,
                IsNewContact = true,
                IsBotChat = true,
                IsBotCloseChat = false,
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                customerChat = new CustomerChat()
                {
                    CreateDate = DateTime.Now,
                },
                creation_timestamp = int.Parse(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                expiration_timestamp = int.Parse(DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString()),
                CustomerStepModel = new CustomerStepModel()
                {
                    ChatStepId = -1,
                    ChatStepPervoiusId = 0,
                    IsLiveChat = false,
                    UserParmeter = UserParmeter
                },
                OneTimeQuestionIds = new Dictionary<string, string>(),
                GroupId = long.Parse(idcont.Group),
                GroupName = idcont.GroupName,
                IsHumanhandover = false,
                channel = "instagram",
                instagramUserInfoModel = instagramUserInfoModel
            };
            CustomerModel.IsConversationExpired = false;
            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;

            // Retrieve the customer information from Cosmos DB
            var itemsCollection2 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;
            return CustomerModel;
        }

        public CustomerModel CreateNewCustomerFacebook2(
                       string psid, // Messenger PSID (Page-Scoped ID)
                       string name,
                       string type,
                       string botID,
                       int TenantId,
                       string fbPageToken) // Facebook Page Access Token
        {
            string userId = TenantId + "_" + psid;
            string displayName = name;

            // Get user profile from Facebook Graph API (Optional)
            string avatarUrl = GetFacebookUserProfile(psid, fbPageToken);

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = userId,
                DisplayName = displayName,
                AvatarUrl = avatarUrl,
                CreatorUserId = 1,
                Description = "",
                EmailAddress = "",
                IsDeleted = false,
                CreationTime = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired = false,
                IsBlock = false,
                IsOpen = false,
                LockedByAgentName = "",
                PhoneNumber = psid, // Using PSID instead of phone number
                SunshineAppID = "",
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1
            };

            var idcont = _IGeneralAppService.CreateContact(cont);

            if (string.IsNullOrEmpty(idcont.Group))
            {
                idcont.Group = "0";
                idcont.GroupName = "";
            }

            var UserParmeter = new Dictionary<string, string>
    {
        { "Name", displayName },
        { "PSID", psid }, // Store PSID instead of phone number
        { "Location", "No Location" },
        { "ContactID", idcont.Id.ToString() },
        { "TenantId", TenantId.ToString() }
    };

            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId = fbPageToken, // Store Page Token if needed
                ConversationsCount = 0,
                ContactID = idcont.Id.ToString(),
                IsComplaint = false,
                userId = userId,
                displayName = displayName,
                avatarUrl = avatarUrl,
                type = type,
                D360Key = fbPageToken, // Messenger uses a Page Token instead of WhatsApp D360Key
                CreateDate = DateTime.Now,
                IsLockedByAgent = false,
                LockedByAgentName = botID,
                IsOpen = false,
                agentId = 100000,
                IsBlock = false,
                IsConversationExpired = false,
                CustomerChatStatusID = (int)CustomerChatStatus.Active,
                CustomerStatusID = (int)CustomerStatus.Active,
                LastMessageData = DateTime.Now,
                IsNew = true,
                TenantId = TenantId,
                phoneNumber = psid, // Store PSID
                UnreadMessagesCount = 1,
                IsNewContact = true,
                IsBotChat = true,
                IsBotCloseChat = false,
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                customerChat = new CustomerChat() { CreateDate = DateTime.Now },
                creation_timestamp = 0,
                expiration_timestamp = 0,
                CustomerStepModel = new CustomerStepModel()
                {
                    ChatStepId = -1,
                    ChatStepPervoiusId = 0,
                    IsLiveChat = false,
                    UserParmeter = UserParmeter
                },
                OneTimeQuestionIds = new Dictionary<string, string>(),
                GroupId = long.Parse(idcont.Group),
                GroupName = idcont.GroupName,
                IsHumanhandover = false,
            };

            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;

            var itemsCollection2 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;
            return CustomerModel;
        }

        public CustomerModel CreateNewCustomerMessanger(
                string psid, // Messenger PSID (Page-Scoped ID)
                string name,
                string type,
                string botID,
                int TenantId,
                string fbPageToken) // Facebook Page Access Token
        {
            string userId = TenantId + "_" + psid;
            string displayName = name;

            // Get user profile from Facebook Graph API (Optional)
            string avatarUrl = GetFacebookUserProfile(psid, fbPageToken);

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = userId,
                DisplayName = displayName,
                AvatarUrl = avatarUrl,
                CreatorUserId = 1,
                Description = "",
                EmailAddress = "",
                IsDeleted = false,
                CreationTime = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired = false,
                IsBlock = false,
                IsOpen = false,
                LockedByAgentName = "",
                PhoneNumber = psid, // Using PSID instead of phone number
                SunshineAppID = "",
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1
            };

            var idcont = _IGeneralAppService.CreateContact(cont);

            if (string.IsNullOrEmpty(idcont.Group))
            {
                idcont.Group = "0";
                idcont.GroupName = "";
            }

            var UserParmeter = new Dictionary<string, string>
    {
        { "Name", displayName },
        { "PSID", psid }, // Store PSID instead of phone number
        { "Location", "No Location" },
        { "ContactID", idcont.Id.ToString() },
        { "TenantId", TenantId.ToString() }
    };

            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId = fbPageToken, // Store Page Token if needed
                ConversationsCount = 0,
                ContactID = idcont.Id.ToString(),
                IsComplaint = false,
                userId = userId,
                displayName = displayName,
                avatarUrl = avatarUrl,
                type = type,
                D360Key = fbPageToken, // Messenger uses a Page Token instead of WhatsApp D360Key
                CreateDate = DateTime.Now,
                IsLockedByAgent = false,
                LockedByAgentName = botID,
                IsOpen = false,
                agentId = 100000,
                IsBlock = false,
                IsConversationExpired = false,
                CustomerChatStatusID = (int)CustomerChatStatus.Active,
                CustomerStatusID = (int)CustomerStatus.Active,
                LastMessageData = DateTime.Now,
                IsNew = true,
                TenantId = TenantId,
                phoneNumber = psid, // Store PSID
                UnreadMessagesCount = 1,
                IsNewContact = true,
                IsBotChat = true,
                IsBotCloseChat = false,
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                customerChat = new CustomerChat() { CreateDate = DateTime.Now },
                creation_timestamp = 0,
                expiration_timestamp = 0,
                CustomerStepModel = new CustomerStepModel()
                {
                    ChatStepId = -1,
                    ChatStepPervoiusId = 0,
                    IsLiveChat = false,
                    UserParmeter = UserParmeter
                },
                OneTimeQuestionIds = new Dictionary<string, string>(),
                GroupId = long.Parse(idcont.Group),
                GroupName = idcont.GroupName,
                IsHumanhandover = false,
            };

            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;

            var itemsCollection2 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;
            return CustomerModel;
        }
        public string GetFacebookUserProfile(string psid, string fbPageToken)
        {
            string apiUrl = $"https://graph.facebook.com/{psid}?fields=first_name,last_name,profile_pic&access_token={fbPageToken}";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    dynamic userData = JsonConvert.DeserializeObject(jsonResult);
                    return userData.profile_pic; // Return profile picture URL
                }
            }
            return "";
        }


        public ContactDto GetCustomerfromDB(string from, string name, string type, string botID, int TenantId, string D360Key)
        {


            string userId = TenantId + "_" + from;
            string displayName = name;
            string phoneNumber = from;





            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = userId,
                DisplayName = displayName,
                AvatarUrl = "",
                CreatorUserId = 1,
                Description = "",
                EmailAddress = "",
                IsDeleted = false,
                CreationTime = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired = false,
                IsBlock = false,
                IsOpen = false,
                LockedByAgentName = "",
                PhoneNumber = phoneNumber,
                SunshineAppID = "",// model.app._id,
                                   //ConversationId = model.statuses.FirstOrDefault().conversation.id,
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1,
                // ConversationId = model.statuses.FirstOrDefault().conversation.id


            };
            var idcont = _IGeneralAppService.CreateContact(cont);
  
      
            return idcont;







        }

        public async Task<TenantModel> GetTenantByKey(string phoneNumber, string key)
        {
            var result = await GetTenantByKeyD360(key);
            return result;
        }
        public async Task<TenantModel> GetTenantByFacebookId(string facebookId, string key)
        {
            var result = await GetTenantByFacebookId(key);
            return result;
        }
        public async Task<TenantModel> GetTenantByInstagramId(string instagramId, string key)
        {
            var result = await GetTenantByInstagramId(key);
            return result;
        }
        public async Task<TenantModel> GetTenantByKey2(string key)
        {
            var result = await GetTenantByKeyD360(key);
            return result;
        }
        public async Task<TenantModel> GetTenantInfoById(int key)
        {
            var result = await GetTenantByIdD360(key);
            return result;
        }

        public async Task<TenantModel> GetTenantInfoByPhoneNumber(string phoneNumber)

        {
            var result = await GetTenantByPhoneNumber(phoneNumber);
            return result;
        }


        public async Task<TenantModel> GetTenantByAppId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.SmoochAppID == id);
            return tenant;
        }
        public async Task<TenantModel> GetTenantByKeyD360( string key)
        {
            if ( string.IsNullOrEmpty(key))
                return null;
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection,_IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.D360Key == key);
            return tenant;
        }

        public async Task<TenantModel> GetTenantByFacebookId(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.FacebookPageId == key);
            return tenant;
        }
        public async Task<TenantModel> GetTenantByInstagramId(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.InstagramId == key);
            return tenant;
        }
        public async Task<TenantModel> GetTenantByIdD360( int key)
        {
            
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection,_IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == key);
            return tenant;
        }

        public async Task<TenantModel> GetTenantByMerchantId(string MerchantId)
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.MerchantID == MerchantId);
            return tenant;
        }
        public async Task<TenantModel> GetTenantByPhoneNumber(string phoneNumber)
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.PhoneNumber == phoneNumber);
            return tenant;
        }
        public async Task<CustomerModel> GetCustomer(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);

            var customer = await itemsCollection.GetItemAsync(a =>a.ItemType==InfoSeedContainerItemTypes.CustomerItem && a.userId==userId);
            return customer;
        }
        public async Task<CustomerModel> GetCustomerByContactId(string contactId)
        {
            if (string.IsNullOrEmpty(contactId))
                return null;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);

            var customer = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.ContactID == contactId);
            return customer;
        }
        public async Task<CustomerModel> GetCustomerWithTenantId(string userId, int? tenantId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);

            var customer = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId &&a.TenantId== tenantId);
            return customer;
        }
        public async Task<List<CustomerModel>> GetCustomersAsync(int? tenantId,int pageNumber, int pageSize, string searchTerm = default(string))
        {     
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customers = await itemsCollection.GetItemsRAsync(a=> a.TenantId == tenantId && a.ItemType== InfoSeedContainerItemTypes.CustomerItem&&  a.CreateDate!=null && a.customerChat != null &&
               ((!string.IsNullOrEmpty(searchTerm) ? a.displayName.ToLower().Contains(searchTerm) :a.displayName != null)
              ||(!string.IsNullOrEmpty(searchTerm) ? a.userId.ToLower().Contains(searchTerm) :(a.userId != null || a.userId == null)))
            , null,pageSize,pageNumber,a=> a.customerChat.CreateDate);

            var rez = customers.Item1.ToList();


            foreach (var cus in rez)
            {

                //For if the date value is the default {0001-01-01T00:00:00} , it must be equal to null
                if (cus.CloseTime.HasValue && cus.CloseTime.Value.Year < 1400)
                {
                    cus.CloseTime = null;
                }
                if (cus.lastNotificationsData.HasValue && cus.lastNotificationsData.Value.Year < 1400)
                {
                    cus.lastNotificationsData = null;
                }
                if (cus.CreateDate.HasValue && cus.CreateDate.Value.Year < 1400)
                {
                    cus.CreateDate = null;
                }
                if (cus.ModifyDate.HasValue && cus.ModifyDate.Value.Year < 1400)
                {
                    cus.ModifyDate = null;
                }
                if (cus.LastMessageData.HasValue && cus.LastMessageData.Value.Year < 1400)
                {
                    cus.LastMessageData = null;
                }
                if (cus.LastConversationStartDateTime.HasValue && cus.LastConversationStartDateTime.Value.Year < 1400)
                {
                    cus.LastConversationStartDateTime = null;
                }
                if (cus.requestedLiveChatTime.HasValue && cus.requestedLiveChatTime.Value.Year < 1400)
                {
                    cus.requestedLiveChatTime = null;
                }
                if (cus.OpenTime.HasValue && cus.OpenTime.Value.Year < 1400)
                {
                    cus.OpenTime = null;
                }
                
                if (cus.CloseTimeTicket.HasValue && cus.CloseTimeTicket.Value.Year < 1400)
                {
                    cus.CloseTimeTicket = null;
                }
                if (cus.OpenTimeTicket.HasValue && cus.OpenTimeTicket.Value.Year < 1400)
                {
                    cus.OpenTimeTicket = null;
                }
                if (cus.customerChat != null)
                {
                    if (cus.customerChat.lastNotificationsData.HasValue && cus.customerChat.lastNotificationsData.Value.Year < 1400)
                    {
                        cus.customerChat.lastNotificationsData = null;
                    }
                    if (cus.customerChat.CreateDate.HasValue && cus.customerChat.CreateDate.Value.Year < 1400)
                    {
                        cus.customerChat.CreateDate = null;
                    }
                }
                if (cus.CustomerStepModel != null)
                {
                    if (cus.CustomerStepModel.OrderCreationTime.HasValue && cus.CustomerStepModel.OrderCreationTime.Value.Year < 1400)
                    {
                        cus.CustomerStepModel.OrderCreationTime = null;
                    }
                }

                

                cus.IsSelectedPage = true;
                if (cus.creation_timestamp==0 || cus.expiration_timestamp==0)
                {
                    
                    if (cus.ConversationId!=null)
                    {
                        try
                        {
                            var model1 = getConversationSessions(cus.TenantId.Value, cus.phoneNumber, cus.ConversationId);

                            if (model1 != null && model1.expiration_timestamp!=0 &&model1.creation_timestamp!=0)
                            {
                                cus.expiration_timestamp=model1.expiration_timestamp;
                                cus.creation_timestamp=model1.creation_timestamp;

                            }
                            else
                            {
                                TimeSpan timeSpan = DateTime.Now - cus.LastConversationStartDateTime.Value;
                                int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                                if (totalHours <= 24)
                                {
                                    cus.IsConversationExpired = false;
                                }
                                else
                                {
                                    // item.IsOpen = false;
                                    cus.IsConversationExpired = true;
                                }
                            }
                        }
                        catch
                        {
                            cus.expiration_timestamp=0;
                            cus.creation_timestamp=0;

                           
                        }
                       
                    }


                }
                else
                {

                    var diff = cus.expiration_timestamp-cus.creation_timestamp;


                    var offsetcreation = DateTimeOffset.FromUnixTimeSeconds(cus.creation_timestamp);
                    DateTime creationDate = offsetcreation.UtcDateTime;

                    var offsetexpiration = DateTimeOffset.FromUnixTimeSeconds(cus.expiration_timestamp);
                    DateTime expirationDate = offsetexpiration.UtcDateTime;


                    TimeSpan timediff = expirationDate - creationDate;
                    int totalHoursforuser = (int)(timediff.TotalHours);


                    if (DateTime.UtcNow<=expirationDate)
                    {
                        cus.IsConversationExpired = false;
                    }
                    else
                    {

                        cus.IsConversationExpired = true;
                    }
                }

                var Result = itemsCollection.UpdateItemAsync(cus._self, cus).Result;

            }
            
            return rez;
        }

        public async Task<List<CustomerModel>> CustomersGetAllAsync(int? tenantId, int pageNumber, int pageSize, string searchTerm = default(string) ,int searchId = 0, int chatFilterID = 0, int agentId = 0, string userId = "")
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            List<CustomerModel> customerList = new List<CustomerModel>();

            var ChatItemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
            List<CustomerChat> customerChat = new List<CustomerChat>();
            var sixtyHoursAgo = DateTime.UtcNow.AddHours(-46);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm=searchTerm.ToLower().Trim();
                switch (searchId)
                {
                    case 1:
                        {
                            var customers = await itemsCollection.GetItemsRAsync(a =>
                                a.TenantId == tenantId
                                && a.ItemType == InfoSeedContainerItemTypes.CustomerItem
                                && a.CreateDate != null
                                && a.customerChat != null 
                                &&
                                (
                                    (!string.IsNullOrEmpty(searchTerm) ? a.displayName.ToLower().Contains(searchTerm) : a.displayName != null)
                                    || (!string.IsNullOrEmpty(searchTerm) ? a.userId.ToLower().Contains(searchTerm) : (a.userId != null || a.userId == null))
                                )
                                && (chatFilterID == 0 || (a.IsOpen && chatFilterID == 1) || (a.IsConversationExpired && chatFilterID == 2) || (!a.IsOpen && chatFilterID == 3))
                            , null, pageSize, pageNumber, a => a.LastMessageData);

                            customerList = customers.Item1.ToList();
                        }
                        break;
                    case 2:
                        {
                            var chatConversation = await ChatItemsCollection.GetItemsRAsync(a =>
                                a.TenantId == tenantId
                                && a.ItemType == InfoSeedContainerItemTypes.ConversationItem 
                                && a.sender == MessageSenderType.Customer
                                && a.userId != null
                                &&
                                (!string.IsNullOrEmpty(searchTerm) ? a.text.ToLower().Contains(searchTerm) : a.text != null)
                            , null, pageSize, pageNumber, x => x.CreateDate);

                            customerChat = chatConversation.Item1.ToList();
                        }
                    break;
                    case 3:
                        {
                            var customers = await itemsCollection.GetItemsRAsync(a =>
                                a.TenantId == tenantId
                                && a.ItemType == InfoSeedContainerItemTypes.CustomerItem
                                && a.CreateDate != null
                                && a.customerChat != null && a.customerChat.userId != null
                                &&
                                (
                                    (!string.IsNullOrEmpty(searchTerm) ? a.displayName.ToLower().Contains(searchTerm) : a.displayName != null)
                                    || (!string.IsNullOrEmpty(searchTerm) ? a.userId.ToLower().Contains(searchTerm) : (a.userId != null || a.userId == null))
                                )
                                && (chatFilterID == 0 || (a.IsOpen && chatFilterID == 1) || (a.IsConversationExpired && chatFilterID == 2) || (!a.IsOpen && chatFilterID == 3))
                            , null, pageSize, pageNumber, a => a.customerChat.CreateDate);

                            customerList = customers.Item1.ToList();

                            //var chatConversation = await ChatItemsCollection.GetItemsRAsync(a =>
                            //    a.TenantId == tenantId
                            //    && a.ItemType == InfoSeedContainerItemTypes.ConversationItem
                            //    && a.userId != null
                            //    && a.sender == MessageSenderType.Customer
                            //    &&
                            //    (!string.IsNullOrEmpty(searchTerm) ? a.text.ToLower().Contains(searchTerm) : a.text != null)
                            //, null, pageSize, pageNumber, x => x.CreateDate);

                            //customerChat = chatConversation.Item1.ToList();
                           // customerChat = chatConversation.Item1.DistinctBy(x => x.userId).ToList();
                            
                        }
                    break;

                    case 4:
                        {

                            var customers = await itemsCollection.GetItemsRAsync(a =>
                    a.TenantId == tenantId
                    && a.ItemType == InfoSeedContainerItemTypes.CustomerItem
                    && a.CreateDate != null
                    && a.customerChat != null && a.customerChat.CreateDate != null && a.customerChat.userId != null
                    && a.displayName != null
                    && a.LastMessageData.HasValue && a.LastMessageData.Value >= DateTime.UtcNow.AddDays(-5)
                    && (chatFilterID == 0 || (a.IsOpen && chatFilterID == 1) || (a.IsConversationExpired && chatFilterID == 2) || (!a.IsOpen && chatFilterID == 3))
                , null, pageSize, pageNumber, a => a.customerChat.CreateDate);
                            customerList = customers.Item1.ToList();

                            try
                            {
                                List<LiveChatModel> liveChatList = new List<LiveChatModel>();
                                int totalCountOut = 0;

                                var SP_Name = Constants.LiveChat.SP_GetLiveChatData;

                                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                        {
                           new System.Data.SqlClient.SqlParameter("@StartDate", sixtyHoursAgo),
                           new System.Data.SqlClient.SqlParameter("@TenantId", tenantId)
                        };

                                liveChatList = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                                                  DataReaderMapper.ConvertLiveChatDto, AppSettingsModel.ConnectionStrings).ToList();

                                var listLiveChat = new PagedResultDto<LiveChatModel>(liveChatList.Count, liveChatList);

                                var liveChatUserIds = new HashSet<string>(listLiveChat.Items
                                    .Where(lc => !string.IsNullOrEmpty(lc.UserId))
                                    .Select(lc => lc.UserId));

                                return customerList.Where(customer =>
                                    customer.customerChat != null &&
                                    !string.IsNullOrEmpty(customer.customerChat.userId) &&
                                    !liveChatUserIds.Contains(customer.customerChat.userId)
                                ).ToList();


                                var x = 10;

                                break;

                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    case 5:
                        {

                            if (chatFilterID==5)
                            {

                                var customers = await itemsCollection.GetItemsRAsync(a =>
                                   a.TenantId == tenantId
                                   && a.ItemType == InfoSeedContainerItemTypes.CustomerItem
                                   && a.CreateDate != null
                                   && a.customerChat != null && a.customerChat.userId != null
                                   &&
                                   (
                                       (!string.IsNullOrEmpty(searchTerm) ? a.displayName.ToLower().Contains(searchTerm) : a.displayName != null)
                                       || (!string.IsNullOrEmpty(searchTerm) ? a.userId.ToLower().Contains(searchTerm) : (a.userId != null || a.userId == null))
                                   )
                               , null, pageSize, pageNumber, a => a.customerChat.CreateDate);

                                customerList = customers.Item1.ToList();

                            }
                            else
                            {

                                pageSize=int.MaxValue;
                                //DateTime lastThreeMonths = DateTime.UtcNow.AddDays(-7);

                                var chatConversation = await ChatItemsCollection.GetItemsRAsync(
                                    a => a.TenantId == tenantId
                                         && a.ItemType == InfoSeedContainerItemTypes.ConversationItem
                                         && a.userId != null
                                         && a.sender == MessageSenderType.Customer
                                         // && a.CreateDate >= lastThreeMonths 
                                         && (
                                             string.IsNullOrEmpty(searchTerm)
                                             ? a.text != null
                                             : a.text.ToLower().Contains(searchTerm)
                                         ),
                                    null, pageSize, pageNumber, x => x.CreateDate);

                                customerChat = chatConversation.Item1.ToList();

                            }

                            break;

                        }
                    case 6:
                        {

                            pageSize=int.MaxValue;
                            //DateTime lastThreeMonths = DateTime.UtcNow.AddDays(-7);

                            var chatConversation = await ChatItemsCollection.GetItemsRAsync(
                                a => a.TenantId == tenantId
                                     && a.ItemType == InfoSeedContainerItemTypes.ConversationItem
                                     && a.userId != null
                                     && a.sender == MessageSenderType.TeamInbox
                                     && a.type =="note"
                                     //&& a.agentId ==agentId.ToString()
                                     && a.userId ==userId
                                     // && a.CreateDate >= lastThreeMonths 
                                     && (
                                         string.IsNullOrEmpty(searchTerm)
                                         ? a.text != null
                                         : a.text.ToLower().Contains(searchTerm)
                                     ),
                                null, pageSize, pageNumber, x => x.CreateDate);

                            customerChat = chatConversation.Item1.ToList();


                        }
                        break;
                }
            }
            else
            {
                if (chatFilterID == 4)
                {
                   var customers2 = await itemsCollection.GetItemsRAsync(a =>
                  a.TenantId == tenantId
                  && a.ItemType == InfoSeedContainerItemTypes.CustomerItem
                  && a.CreateDate != null
                  && a.customerChat != null && a.customerChat.CreateDate != null && a.customerChat.userId != null
                  && a.displayName != null
                  && a.LastMessageData.HasValue && a.LastMessageData.Value >= DateTime.UtcNow.AddDays(-2)
                  && (chatFilterID == 0 || (a.IsOpen && chatFilterID == 1) || (a.IsConversationExpired && chatFilterID == 2) || (chatFilterID == 4) || (!a.IsOpen && chatFilterID == 3))
              , null, pageSize, pageNumber, a => a.customerChat.CreateDate);

                    customerList = customers2.Item1.ToList();
                    try
                    {
                        List<LiveChatModel> liveChatList = new List<LiveChatModel>();
                        int totalCountOut = 0;

                        var SP_Name = Constants.LiveChat.SP_GetLiveChatData;

                        var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                          {
                             new System.Data.SqlClient.SqlParameter("@StartDate", sixtyHoursAgo),
                             new System.Data.SqlClient.SqlParameter("@TenantId", tenantId)
                          };

                        liveChatList = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                                          DataReaderMapper.ConvertLiveChatDto, AppSettingsModel.ConnectionStrings).ToList();

                        var listLiveChat = new PagedResultDto<LiveChatModel>(liveChatList.Count, liveChatList);

                        var liveChatUserIds = new HashSet<string>(listLiveChat.Items
                            .Where(lc => !string.IsNullOrEmpty(lc.UserId))
                            .Select(lc => lc.UserId));

                        return customerList.Where(customer =>
                            customer.customerChat != null &&
                            !string.IsNullOrEmpty(customer.customerChat.userId) &&
                            !liveChatUserIds.Contains(customer.customerChat.userId)
                        ).ToList();


                        var x = 10;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                var   customers = await itemsCollection.GetItemsRAsync(a =>
              a.TenantId == tenantId
              && a.ItemType == InfoSeedContainerItemTypes.CustomerItem
              && a.CreateDate != null
              && a.customerChat != null && a.customerChat.CreateDate != null && a.customerChat.userId != null
              && a.displayName != null
              && (chatFilterID == 0 || (a.IsOpen && chatFilterID == 1) || (a.IsConversationExpired && chatFilterID == 2) || (!a.IsOpen && chatFilterID == 3) || chatFilterID == 4)
          , null, pageSize, pageNumber, a => a.customerChat.CreateDate);
                customerList = customers.Item1.ToList();


       
                customerList = customers.Item1.ToList();
            }


            if (customerChat.Count != 0)
            {
                //pageNumber = 0;
                //pageSize = 10000;
                foreach (var cus in customerChat)
                {

                    var customers = await itemsCollection.GetItemsRAsync(a =>
                        a.TenantId == tenantId
                        && a.ItemType == InfoSeedContainerItemTypes.CustomerItem
                        && a.userId == cus.userId
                        && (chatFilterID == 0 || (a.IsOpen && chatFilterID == 1) || (a.IsConversationExpired && chatFilterID == 2) || (!a.IsOpen && chatFilterID == 3))
                    , null, pageSize, pageNumber, a => a.LastMessageData);

                    var customerModels = customers.Item1.FirstOrDefault();
                    if (customerModels != null)
                    {
                        customerModels.listCustomerChat.Add(cus);

                        // Accumulate data from each iteration
                        customerList.Add(customerModels);
                    }
                }
            }
            if (customerList.Count != 0)
            {
                foreach (var cus in customerList)
                {
                    if (cus.listCustomerChat.Count != 0)
                    {
                        cus.searchId = 2;
                    }
                    else
                    {
                        cus.searchId = 1;
                    }

                    #region For if the date value is the default {0001-01-01T00:00:00} , it must be equal to null
                    if (cus.CloseTime.HasValue && cus.CloseTime.Value.Year < 1400) { cus.CloseTime = null; }

                    if (cus.lastNotificationsData.HasValue && cus.lastNotificationsData.Value.Year < 1400) { cus.lastNotificationsData = null; }

                    if (cus.CreateDate.HasValue && cus.CreateDate.Value.Year < 1400) { cus.CreateDate = null; }

                    if (cus.ModifyDate.HasValue && cus.ModifyDate.Value.Year < 1400) { cus.ModifyDate = null; }

                    if (cus.LastMessageData.HasValue && cus.LastMessageData.Value.Year < 1400) { cus.LastMessageData = null; }

                    if (cus.LastConversationStartDateTime.HasValue && cus.LastConversationStartDateTime.Value.Year < 1400) { cus.LastConversationStartDateTime = null; }

                    if (cus.requestedLiveChatTime.HasValue && cus.requestedLiveChatTime.Value.Year < 1400) { cus.requestedLiveChatTime = null; }

                    if (cus.OpenTime.HasValue && cus.OpenTime.Value.Year < 1400) { cus.OpenTime = null; }

                    if (cus.CloseTimeTicket.HasValue && cus.CloseTimeTicket.Value.Year < 1400) { cus.CloseTimeTicket = null; }

                    if (cus.OpenTimeTicket.HasValue && cus.OpenTimeTicket.Value.Year < 1400) { cus.OpenTimeTicket = null; }

                    if (cus.customerChat != null)
                    {
                        if (cus.customerChat.lastNotificationsData.HasValue && cus.customerChat.lastNotificationsData.Value.Year < 1400) { cus.customerChat.lastNotificationsData = null; }

                        if (cus.customerChat.CreateDate.HasValue && cus.customerChat.CreateDate.Value.Year < 1400) { cus.customerChat.CreateDate = null; }
                    }

                    if (cus.CustomerStepModel != null)
                    {
                        if (cus.CustomerStepModel.OrderCreationTime.HasValue && cus.CustomerStepModel.OrderCreationTime.Value.Year < 1400) { cus.CustomerStepModel.OrderCreationTime = null; }
                    }
                    #endregion

                    cus.IsSelectedPage = true; 
                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    // Check if the expiration time has passed
                    if (currentTimestamp > cus.expiration_timestamp)
                    {
                        try
                        {
                            var model = getConversationSessions(cus.TenantId.Value, cus.phoneNumber, "0");

                            if (model!=null)
                            {
                                cus.creation_timestamp = model.creation_timestamp;
                                cus.expiration_timestamp = model.expiration_timestamp;
                            }
                            else
                            {


                                cus.creation_timestamp = 0;
                                cus.expiration_timestamp = 0;

                            }
                        }
                        catch
                        {
                            cus.creation_timestamp = 0;
                            cus.expiration_timestamp = 0;

                        }
   
                       
                    }
                    if (cus.creation_timestamp == 0 || cus.expiration_timestamp == 0)
                    {
                        if (cus.ConversationId != null)
                        {
                            try
                            {
                                var model1 = getConversationSessions(cus.TenantId.Value, cus.phoneNumber, "0");

                                if (model1 != null && model1.expiration_timestamp != 0 && model1.creation_timestamp != 0)
                                {
                                    cus.expiration_timestamp = model1.expiration_timestamp;
                                    cus.creation_timestamp = model1.creation_timestamp;

                                }
                                else
                                {
                                    TimeSpan timeSpan = DateTime.Now - cus.LastConversationStartDateTime.Value;
                                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                                    if (totalHours <= 24)
                                    {
                                        cus.IsConversationExpired = false;
                                    }
                                    else
                                    {
                                        // item.IsOpen = false;
                                        cus.IsConversationExpired = true;
                                    }
                                }
                            }
                            catch
                            {
                                cus.expiration_timestamp = 0;
                                cus.creation_timestamp = 0;
                            }
                        }
                        else
                        {

                            cus.IsConversationExpired = true;
                        }
                    }
                    else
                    {
                        var diff = cus.expiration_timestamp - cus.creation_timestamp;

                        var offsetcreation = DateTimeOffset.FromUnixTimeSeconds(cus.creation_timestamp);
                        DateTime creationDate = offsetcreation.UtcDateTime;

                        var offsetexpiration = DateTimeOffset.FromUnixTimeSeconds(cus.expiration_timestamp);
                        DateTime expirationDate = offsetexpiration.UtcDateTime;

                        TimeSpan timediff = expirationDate - creationDate;
                        int totalHoursforuser = (int)(timediff.TotalHours);

                        if (DateTime.UtcNow <= expirationDate)
                        {
                            cus.IsConversationExpired = false;
                        }
                        else
                        {
                            cus.IsConversationExpired = true;
                        }
                    }

                    var list = cus.listCustomerChat;
                    cus.listCustomerChat = new List<CustomerChat>();
                    var Result = itemsCollection.UpdateItemAsync(cus._self, cus).Result;
                    cus.listCustomerChat = list;


                    if (cus.channel=="instagram")
                    {
                        if (cus.instagramUserInfoModel==null)
                        {

                            cus.instagramUserInfoModel=new InstagramUserInfoModel()
                            {
                                profile_pic="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/instagram.avif",
                                follower_count=0,
                                is_business_follow_user=false,
                                is_user_follow_business=false,
                                name=cus.displayName,
                                id="",
                                username=cus.displayName,

                            };
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(cus.instagramUserInfoModel.profile_pic))
                            {
                                cus.instagramUserInfoModel=new InstagramUserInfoModel()
                                {
                                    profile_pic="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/instagram.avif",
                                    follower_count=0,
                                    is_business_follow_user=false,
                                    is_user_follow_business=false,
                                    name=cus.displayName,
                                    id="",
                                    username=cus.displayName,

                                };
                            }
                            


                        }

                        var datenow = DateTime.UtcNow;
                        var createdDate = cus.customerChat.CreateDate.GetValueOrDefault();

                        if ((datenow - createdDate).TotalDays > 2)
                        {
                            cus.instagramUserInfoModel.profile_pic="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/instagram.avif";
                        }

                    }
                    else if (cus.channel=="facebook")
                    {
                        if (cus.facebookUserInfoModel==null)
                        {

                            cus.facebookUserInfoModel=new FacebookUserInfoModel()
                            {
                                profile_pic="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/facebook.png",
                                first_name=cus.displayName,
                                gender="male",
                                last_name=cus.displayName,

                            };
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(cus.facebookUserInfoModel.profile_pic))
                            {
                                cus.facebookUserInfoModel=new FacebookUserInfoModel()
                                {
                                    profile_pic="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/facebook.png",
                                    first_name=cus.displayName,
                                    gender="male",
                                    last_name=cus.displayName,

                                };

                            }
                        }
                    }
                    else
                    {
                        cus.avatarUrl="avatar3";

                    }
                }
            }
            return customerList;
        }
        public async Task<List<CustomerChat>> GetNoteChat(string userId, int pageNumber, int pageSize)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var chatConversation = await itemsCollection.GetItemsRAsync(a =>a.type=="note"&& a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == userId, null, pageSize, pageNumber, x => x.CreateDate);

            var rez = chatConversation.Item1.ToList();
            //foreach (var cus in rez)
            //{

            //    //For if the date value is the default {0001-01-01T00:00:00} , it must be equal to null
            //    if (cus.lastNotificationsData.HasValue && cus.lastNotificationsData.Value.Year < 1400)
            //    {
            //        cus.lastNotificationsData = null;
            //    }
            //    if (cus.CreateDate.HasValue && cus.CreateDate.Value.Year < 1400)
            //    {
            //        cus.CreateDate = null;
            //    }

            //}
            return rez;
        }
        public async Task<List<CustomerChat>> GetCustomersChat(string userId, int pageNumber, int pageSize)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var chatConversation = await itemsCollection.GetItemsRAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == userId, null, pageSize, pageNumber,x=>x.CreateDate);
         
            var rez = chatConversation.Item1.ToList();
            //foreach (var cus in rez)
            //{

            //    //For if the date value is the default {0001-01-01T00:00:00} , it must be equal to null
            //    if (cus.lastNotificationsData.HasValue && cus.lastNotificationsData.Value.Year < 1400)
            //    {
            //        cus.lastNotificationsData = null;
            //    }
            //    if (cus.CreateDate.HasValue && cus.CreateDate.Value.Year < 1400)
            //    {
            //        cus.CreateDate = null;
            //    }
                
            //}
            return rez;
        }

        public CustomerChat UpdateCustomerChatD360(WebHookD360Model model,int? TenantId, int count)
        {
            try
            {


                CustomerChat CustomerChat = new CustomerChat();
                string userId = TenantId + "_" + model.messages.FirstOrDefault().from;
                string text = model.messages.FirstOrDefault().textD360;
                string type = model.messages.FirstOrDefault().typeD360;
                string mediaUrl = model.messages.FirstOrDefault().mediaUrl;
                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);

                if (type == "location")
                {

                    // Create your new conversation instance
                    CustomerChat = new CustomerChat()
                    {
                        TenantId = TenantId,
                        userId = userId,
                        text = "https://maps.google.com/?q=" + text,
                        type = "location",//type,
                        CreateDate = DateTime.Now,
                        status = (int)Messagestatus.New,
                        sender = MessageSenderType.Customer,
                        mediaUrl = mediaUrl,
                        UnreadMessagesCount = count
                    };
                }
                else
                {
                    // Create your new conversation instance
                    CustomerChat = new CustomerChat()
                    {
                        TenantId = TenantId,
                        userId = userId,
                        text = text,
                        type = type,
                        CreateDate = DateTime.Now,
                        status = (int)Messagestatus.New,
                        sender = MessageSenderType.Customer,
                        mediaUrl = mediaUrl,
                        UnreadMessagesCount = count
                    };

                }

                
               

                var Result = itemsCollection.CreateItemAsync(CustomerChat).Result;
                return CustomerChat;
            }
            catch (Exception )
            {

                throw;
            }

        }

        public CustomerChat UpdateCustomerChat(CustomerModel customer, string userId,string text,string type, int TenantId, int count, string mediaUrl, string agentName,string  agentId, MessageSenderType messageSenderType = MessageSenderType.Customer,string massageID="" , Content model = null, Referral referral = null)
        {
            try
            {


                CustomerChat CustomerChat = new CustomerChat();
                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);
                var itemsCollectionCustomer = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.CustomersCollection,_IDocumentClient);

                if (model!=null)
                {
                     CustomerChat = new CustomerChat()
                    {
                        messageId = massageID,
                        userId = userId,
                        SunshineConversationId = massageID,
                        text = model.text,
                        type = model.type,
                        fileName= model.fileName,
                        CreateDate = DateTime.Now,
                        status = (int)Messagestatus.New,
                        sender = MessageSenderType.TeamInbox,
                        mediaUrl = model.mediaUrl,
                        agentName = model.agentName,
                        agentId = model.agentId,
                        TenantId= TenantId,

                    };

                }
                else
                {
                    if (type == "location")
                    {

                        // Create your new conversation instance
                        CustomerChat = new CustomerChat()
                        {
                            messageId = massageID,
                            SunshineConversationId=massageID,
                            TenantId = TenantId,
                            userId = userId,
                            text = "https://maps.google.com/?q=" + text,
                            type = "location",//type,
                            CreateDate = DateTime.Now,
                            status = (int)Messagestatus.New,
                            sender = messageSenderType,
                            mediaUrl = mediaUrl,
                            UnreadMessagesCount = count,
                            agentName = agentName,
                            agentId = agentId,
                        };
                    }
                    else
                    {
                        // Create your new conversation instance
                        CustomerChat = new CustomerChat()
                        {
                            messageId = massageID,
                            SunshineConversationId=massageID,
                            TenantId = TenantId,
                            userId = userId,
                            text = text,
                            type = type,
                            CreateDate = DateTime.Now,
                            status = (int)Messagestatus.New,
                            sender = messageSenderType,
                            mediaUrl = mediaUrl,
                            UnreadMessagesCount = count,
                            agentName = agentName,
                            agentId = agentId,
                        };

                    }
                }
              




                customer.UnreadMessagesCount = 1;
                customer.IsConversationExpired = false;

                if (customer.expiration_timestamp!=0)
                {
                    var diff = customer.expiration_timestamp-customer.creation_timestamp;


                    var offsetcreation = DateTimeOffset.FromUnixTimeSeconds(customer.creation_timestamp);
                    DateTime creationDate = offsetcreation.UtcDateTime;

                    var offsetexpiration = DateTimeOffset.FromUnixTimeSeconds(customer.expiration_timestamp);
                    DateTime expirationDate = offsetexpiration.UtcDateTime;


                    TimeSpan timediff = expirationDate - creationDate;
                    int totalHoursforuser = (int)(timediff.TotalHours);


                    if (DateTime.UtcNow<=expirationDate)
                    {
                        customer.IsConversationExpired = false;
                    }
                    else
                    {
                      //  customer.ConversationsCount = customer.ConversationsCount + 1;
                        customer.IsConversationExpired = true;
                    }


                }
                else
                {

                    if (customer.LastConversationStartDateTime!=null)
                    {

                        TimeSpan timeSpan = DateTime.Now - customer.LastConversationStartDateTime.Value;
                        int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                        if (totalHours <= 24)
                        {
                            customer.IsConversationExpired = false;
                        }
                        else
                        {
                            // item.IsOpen = false;
                            customer.IsConversationExpired = true;
                            // customer.ConversationsCount = customer.ConversationsCount + 1;
                        }
                    }


                }

               

                customer.LastMessageData = DateTime.Now;
                if (customer.IsBlock)
                {
                    CustomerChat = new CustomerChat
                    {
                        messageId = massageID,
                        SunshineConversationId=massageID,
                        notificationsText = "The Customer Blocked By : Admin",
                        userId = userId,
                        sender = MessageSenderType.TeamInbox,
                        type = "notification",
                        CreateDate = DateTime.Now,
                        text = "The Customer Blocked By : Admin",
                        UnreadMessagesCount = 1,
                        TenantId = TenantId,
                        status = (int)Messagestatus.New,
                        mediaUrl = mediaUrl
                    };

                    if (customer.customerChat.text == "The Customer Blocked By : Admin")
                    {
                        customer.customerChat = CustomerChat;
                    }
                }

                if (customer.IsOpen && customer.IsConversationExpired)
                {
                    customer.IsOpen = false;
                    customer.IsLockedByAgent = false;
                    customer.IsConversationExpired = false;

                }
    
                if (customer.IsOpen && messageSenderType==MessageSenderType.Customer)
                {

                    try
                    {
                        customer.ConversationsCount = customer.ConversationsCount+1;
                    }
                    catch
                    {
                        customer.ConversationsCount = 1;
                    }
                    

                }
                else
                {
                    customer.ConversationsCount = 0;
                }

                if (customer.IsConversationExpired && messageSenderType == MessageSenderType.Customer)
                {
           
                    customer.IsConversationExpired = false;
                }


                if (referral != null)
                {
                    CustomerChat.source_url=referral.source_url;
                    CustomerChat.source_id=referral.source_id;
                    CustomerChat.source_type=referral.source_type;
                    CustomerChat.headline=referral.headline;
                    CustomerChat.media_type=referral.media_type;
                    CustomerChat.body=referral.body;
                    CustomerChat.image_url=referral.image_url;
                    CustomerChat.video_url=referral.video_url;
                    CustomerChat.thumbnail_url=referral.thumbnail_url;

                }

                if (customer.IsConversationExpired)
                {


                    customer.IsHumanhandover=false;


                }

                if (type=="note")
                {

                    customer.NumberNote=customer.NumberNote+1;
                }

                var Result = itemsCollection.CreateItemAsync(CustomerChat).Result;
                CustomerChat.id = Result.ID;
                CustomerChat._self = Result.Uri;
                customer.customerChat = CustomerChat;

                var objCustomer = itemsCollectionCustomer.UpdateItemAsync(customer._self, customer).Result;

                return CustomerChat;
            }
            catch (Exception )
            {

                throw;
            }

        }

        public CustomerChat UpdateCustomerChatStatusNew(string messageId ,int TenantId)
        {
            try
            {


                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.messageId == messageId && a.TenantId == TenantId);
                var Customercaht = customerResult.Result;

                if (Customercaht != null)
                {

                    Customercaht.status = 2;
                    var objCustomer = itemsCollection.UpdateItemAsync(Customercaht._self, Customercaht).Result;
                }

                return Customercaht;


            }
            catch(Exception ex)
            {
                return new CustomerChat();
            }
        }
        public CustomerModel PrePareCustomerChat(CustomerModel customer, string userId, string text, string type, int TenantId, int count, string mediaUrl, string agentName, string agentId, MessageSenderType messageSenderType = MessageSenderType.Customer, string massageID = "")
        {
            try
            {


                CustomerChat CustomerChat = new CustomerChat();
                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);
                var itemsCollectionCustomer = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.CustomersCollection,_IDocumentClient);

                if (type == "location")
                {

                    // Create your new conversation instance
                    CustomerChat = new CustomerChat()
                    {
                        messageId = massageID,
                        TenantId = TenantId,
                        userId = userId,
                        text = "https://maps.google.com/?q=" + text,
                        type = "location",//type,
                        CreateDate = DateTime.Now,
                        status = (int)Messagestatus.New,
                        sender = messageSenderType,
                        mediaUrl = mediaUrl,
                        UnreadMessagesCount = count,
                        agentName = agentName,
                        agentId = agentId,
                    };
                }
                else
                {
                    // Create your new conversation instance
                    CustomerChat = new CustomerChat()
                    {
                        messageId = massageID,
                        TenantId = TenantId,
                        userId = userId,
                        text = text,
                        type = type,
                        CreateDate = DateTime.Now,
                        status = (int)Messagestatus.New,
                        sender = messageSenderType,
                        mediaUrl = customer.customerChat.mediaUrl,
                        UnreadMessagesCount = count,
                        agentName = agentName,
                        agentId = agentId,
                    };

                }






                //customer.IsNewContact = false;
                //customer.IsNew = false;
                // customer.IsOpen = false;
                customer.UnreadMessagesCount = 1;
                customer.IsConversationExpired = false;
                //  customer.ConversationId = model.statuses.FirstOrDefault().conversation.id;

                if (customer.expiration_timestamp!=0)
                {
                    var diff = customer.expiration_timestamp-customer.creation_timestamp;


                    var offsetcreation = DateTimeOffset.FromUnixTimeSeconds(customer.creation_timestamp);
                    DateTime creationDate = offsetcreation.UtcDateTime;

                    var offsetexpiration = DateTimeOffset.FromUnixTimeSeconds(customer.expiration_timestamp);
                    DateTime expirationDate = offsetexpiration.UtcDateTime;


                    TimeSpan timediff = expirationDate - creationDate;
                    int totalHoursforuser = (int)(timediff.TotalHours);


                    if (DateTime.UtcNow<=expirationDate)
                    {
                        customer.IsConversationExpired = false;
                    }
                    else
                    {
                        //customer.ConversationsCount = customer.ConversationsCount + 1;
                        customer.IsConversationExpired = true;
                    }


                }
                else
                {
                    try
                    {
                        TimeSpan timeSpan = DateTime.Now - customer.LastConversationStartDateTime.Value;
                        int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                        if (totalHours <= 24)
                        {
                            customer.IsConversationExpired = false;
                        }
                        else
                        {
                            // item.IsOpen = false;
                            customer.IsConversationExpired = true;
                            //customer.ConversationsCount = customer.ConversationsCount + 1;
                        }
                    }
                    catch
                    {
                        customer.IsConversationExpired = true;

                    }
  

                }

                //customer
                // customer.ConversationId = model.statuses.FirstOrDefault().conversation.id;
                customer.LastMessageData = DateTime.Now;


                if (customer.IsOpen && customer.IsConversationExpired)
                {
                    customer.IsOpen = false;
                    customer.IsLockedByAgent = false;
                    customer.IsConversationExpired = false;

                }

                if (customer.IsOpen && messageSenderType==MessageSenderType.Customer)
                {

                    try
                    {
                        customer.ConversationsCount = customer.ConversationsCount+1;
                    }
                    catch
                    {
                        customer.ConversationsCount = 1;
                    }


                }
                else
                {
                    customer.ConversationsCount = 0;
                }

                if (customer.IsConversationExpired && messageSenderType == MessageSenderType.Customer)
                {

                    customer.IsConversationExpired = false;
                }


                if (customer.IsBlock)
                {
                    CustomerChat = new CustomerChat
                    {
                        notificationsText = "The Customer Blocked By : Admin",
                        userId = userId,
                        sender = MessageSenderType.TeamInbox,
                        type = "notification",
                        CreateDate = DateTime.Now,
                        text = "The Customer Blocked By : Admin",
                        UnreadMessagesCount = 1,
                        TenantId = TenantId,
                        status = (int)Messagestatus.New,
                        mediaUrl = mediaUrl
                    };

                    if (customer.customerChat.text == "The Customer Blocked By : Admin")
                    {
                        customer.customerChat = CustomerChat;
                    }
                }

                if (customer.IsOpen && customer.IsConversationExpired)
                {
                    customer.IsOpen = false;
                    customer.IsLockedByAgent = false;
                    customer.IsConversationExpired = false;

                }

                if (customer.IsConversationExpired && messageSenderType == MessageSenderType.Customer)
                {

                    customer.IsConversationExpired = false;
                }

             
                var Result = itemsCollection.CreateItemAsync(CustomerChat).Result;
                CustomerChat.id = Result.ID;
                CustomerChat._self = Result.Uri;
                customer.customerChat = CustomerChat;


                var objCustomer = itemsCollectionCustomer.UpdateItemAsync(customer._self, customer).Result;


                return customer;
            }
            catch (Exception )
            {

                throw;
            }

        }

        public CustomerChat UpdateCustomerStatus(CustomerModel customer)
        {
            try
            {


                CustomerChat CustomerChat = new CustomerChat();
                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var itemsCollectionCustomer = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.CustomersCollection, _IDocumentClient);
              
                var objCustomer = itemsCollectionCustomer.UpdateItemAsync(customer._self, customer).Result;

                return CustomerChat;
            }
            catch (Exception )
            {

                throw;
            }

        }


        public CustomerChat UpdateCustomerWithChat(CustomerChat model)
        {
            try
            {
                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);
                var Result = itemsCollection.CreateItemAsync(model).Result;
                return model;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public  CustomerChat UpdateCustomerChat(int? tenantId, Content model, string userId, string conversationID)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);
           
            // Create your new conversation instance
            var CustomerChat = new CustomerChat()
            {
                messageId = Guid.NewGuid().ToString(),
                userId = userId,
                SunshineConversationId = conversationID,
                text = model.text,
                type = model.type,
                fileName= model.fileName,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                agentName = model.agentName,
                agentId = model.agentId,
                 TenantId= tenantId,

            };
            var result= itemsCollection.CreateItemAsync(CustomerChat).Result;
            return CustomerChat;
        }

        public CustomerChat UpdateCustomerChatFacebook(int? tenantId, Content model, string userId, string conversationID)
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
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                agentName = model.agentName,
                agentId = model.agentId,
                TenantId = tenantId,

            };
            var result = itemsCollection.CreateItemAsync(CustomerChat).Result;

            return CustomerChat;
        }
        public CustomerChat UpdateCustomerChatD360(int? tenantId, Content model, string userId, string conversationID)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);

            // Create your new conversation instance
            var CustomerChat = new CustomerChat()
            {
                messageId = conversationID,
                userId = userId,
                SunshineConversationId = conversationID,
                text = model.text,
                type = model.type,
                fileName= model.fileName,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                agentName = model.agentName,
                agentId = model.agentId,
                TenantId= tenantId,

            };
            var result = itemsCollection.CreateItemAsync(CustomerChat).Result;

            return CustomerChat;
        }
        public CustomerChat UpdateCustomerChatFacebookAPI(FacebookContent model)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);

            // Ensure optional fields are initialized
            if (string.IsNullOrEmpty(model.FileName))
            {
                model.FileName = "";
            }
            if (string.IsNullOrEmpty(model.MediaUrl))
            {
                model.MediaUrl = "";
            }
            if (model.Buttons == null)
            {
                model.Buttons = new List<string>();
            }
            if (model.QuickReplies == null)
            {
                model.QuickReplies = new List<FacebookContent.QuickReply>();
            }

            // Create your new conversation instance
            var customerChat = new CustomerChat()
            {
                messageId = model.ConversationId,
                SunshineConversationId = model.ConversationId,
                TenantId = model.TenantId,
                userId = model.UserId,
                text = model.Text,
                type = model.Type,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = model.MediaUrl,
                UnreadMessagesCount = 0,
                agentName = model.AgentName,
                agentId = model.AgentId,
                fileName = model.FileName,
                IsButton = model.IsButton,
                Buttons = model.Buttons,
                // Assuming CustomerChat has properties for QuickReplies and IsQuickReply
                IsQuickReply = model.IsQuickReply,
                QuickReplies = model.QuickReplies.Select(qr => new QuickReply
                {
                    ContentType = qr.ContentType,
                    Title = qr.Title,
                    Payload = qr.Payload,
                    ImageUrl = qr.ImageUrl
                }).ToList()
            };

            var result = itemsCollection.CreateItemAsync(customerChat).Result;

            return customerChat;
        }
        public CustomerChat UpdateCustomerChatWhatsAppAPI(WhatsAppContent model)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);



            if (string.IsNullOrEmpty(model.fileName))
            {

                model.fileName="";
            }
            if (string.IsNullOrEmpty(model.mediaUrl))
            {

                model.mediaUrl="";
            }
            if (model.Buttons==null)
            {

                model.Buttons=new List<string>();
            }



            // Create your new conversation instance
            var CustomerChat = new CustomerChat()
            {
                messageId = model.conversationID,
                SunshineConversationId=model.conversationID,
                TenantId = model.tenantId,
                userId = model.userId,
                text = model.text,
                type = model.type,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                UnreadMessagesCount = 0,
                agentName =  model.agentName,
                agentId = model.agentId,

                //messageId = model.conversationID,
                //userId = model.userId,
                //SunshineConversationId = model.conversationID,
                //text = model.text,
                //type = model.type,
                //fileName = model.fileName,
                //CreateDate = DateTime.Now,
                //status = (int)Messagestatus.New,
                //sender = MessageSenderType.TeamInbox,
                //mediaUrl = model.mediaUrl,
                //agentName = model.agentName,
                //agentId = model.agentId,
                //TenantId = model.tenantId,
                // Buttons=model.Buttons,
                //  IsButton = model.IsButton,

            };

            var result = itemsCollection.CreateItemAsync(CustomerChat).Result;

            return CustomerChat;
        }

        public async Task<CustomerModel> UpdateNoteCounter(string contactId, int agentId, string agentName, int NumberNote)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);
            var customer = customerResult.Result;

            string result = string.Empty;

            if (customerResult.IsCompletedSuccessfully)
            {
                if (customer != null)
                {
                    //customer.lastNotificationsData = DateTime.Now;
                    customer.NumberNote = customer.NumberNote+1;
                    customer.TenantId = customer.TenantId;

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    return customer;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }


            return null;
        }
        public async Task<CustomerModel> UpdateNoteByAgent(string contactId, int agentId, string agentName,bool IsNote)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);
            var customer = customerResult.Result;

            string result = string.Empty;

            if (customerResult.IsCompletedSuccessfully)
            {
                if (customer != null)
                {
                    //customer.lastNotificationsData = DateTime.Now;
                    customer.IsNote = IsNote;
                    customer.TenantId = customer.TenantId;

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    return customer;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }


            return null;
        }


        public async Task<CustomerModel> OpenByAgent(string contactId, int agentId, string agentName, bool isLocked, string text)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);
            var customer = customerResult.Result;

            string result = string.Empty;
          
            var chat = UpdateChatNotifications2(text, contactId, customer);
            if (customerResult.IsCompletedSuccessfully)
            {
                if (customer != null)
                {
                    customer.lastNotificationsData = DateTime.Now;
                    customer.notificationsText = text;
                    customer.UnreadMessagesCount = 0;
                    customer.IsOpen = isLocked;
                    customer.agentId = agentId;
                    customer.IsLockedByAgent = isLocked;
                    customer.LockedByAgentName = agentName;
                    customer.customerChat = chat;
                    customer.IsBotCloseChat = false;
                    customer.IsBotChat = true;
                    customer.IsSupport = false;
                    customer.TenantId = customer.TenantId;

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    return customer;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
       
            
            return null;
        }

        public async Task<CustomerModel> UpdateLiveChat(CustomerModel customerModel)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.ContactID == customerModel.ContactID);
           
            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
      
                    await itemsCollection.UpdateItemAsync(customer._self, customerModel);
                    return customerModel;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return null;

        }
        public async Task<string> LockedAndUnlockedByAgent(string contactId, int agentId, string agentName, bool isLocked, UserNotification userNotification)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);
           var chat= UpdateChatNotifications(userNotification, contactId);
            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    customer.lastNotificationsData = userNotification.Notification.CreationTime;
                    customer.notificationID = userNotification.Id.ToString();
                    customer.notificationsText = userNotification.Notification.Data.Properties.FirstOrDefault().Value.ToString();
                    customer.IsOpen = isLocked;
                    customer.agentId = agentId;
                    customer.IsLockedByAgent = isLocked;
                    customer.LockedByAgentName = agentName;
                    customer.customerChat = chat;
                    customer.TenantId = customer.TenantId;

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    result = "Done";
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return result;



        }        
        public Dictionary<string, int> GetChatStatus()
        {
      
            Dictionary<string, int> dictionary = new Dictionary<string,int>();
            foreach (int enumValue in  Enum.GetValues(typeof(CustomerChatStatus)))
            {
                dictionary.Add(Enum.GetName(typeof(CustomerChatStatus), enumValue), enumValue);
            }
            return dictionary;
        }
        public async Task<string> UpdateCustomerInfo(UpdateCustomerModel customerModel)
        {

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == customerModel.UserId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {


                    var contact = GetContactsList(customerModel.UserId).Result.FirstOrDefault();
                    
                    if(contact!=null)
                    {
                        contact.IsConversationExpired = customer.IsConversationExpired;
                        contact.IsLockedByAgent = customer.IsLockedByAgent;
                        contact.LockedByAgentName = customer.LockedByAgentName;
                        contact.IsOpen = customer.IsOpen;
                        
                        contact.IsBlock = customer.IsBlock;
                        contact.DisplayName = customerModel.DisplayName;
                        contact.PhoneNumber = customerModel.PhoneNumber;
                        contact.EmailAddress = customerModel.EmailAddress;
                        contact.Description = customerModel.Description;
                        contact.Website = customerModel.Website;

                        //// update contact to database 
                        //UpdateContact(objContactDto);
                    }
                  
                    customer.displayName = customerModel.DisplayName;
                    customer.phoneNumber = customerModel.PhoneNumber;
                    customer.EmailAddress = customerModel.EmailAddress;
                    customer.Website = customerModel.Website;
                    customer.Description = customerModel.Description;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                  
                    result = "done";
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return result;


         
        }
        public async Task<CustomerModel> UpdateCustomerLocation(ContactDto objContactDto)
        {
           // Contact user = new Contact();
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == objContactDto.UserId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    //// update contact to database 
                    // UpdateContactInfo(objContactDto);
                    _IGeneralAppService.UpdateContactInfo(objContactDto);

                    customer.Description = objContactDto.Description;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);

                    return customer;
                }
            }
            else
            {
                return null;
            }

            return null;


        }
        //Close Chat
        public async Task<CustomerModel> UpdateCustomerStatus(string userId,string lockedByAgentName, bool IsOpen, string text)
        {

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);
            var customer = customerResult.Result;

            if (customer.IsLockedByAgent )//&& customer.LockedByAgentName == lockedByAgentName)
            {
                var chat = UpdateChatNotifications2(text, userId, customer);
                string result = string.Empty;
                if (customerResult.IsCompletedSuccessfully)
                {
                    if (customer != null)
                    {
                        customer.lastNotificationsData = DateTime.Now;                   
                        customer.notificationsText = text;
                        customer.LockedByAgentName = lockedByAgentName;
                        customer.IsLockedByAgent = IsOpen;
                        customer.IsOpen = IsOpen;
                        customer.customerChat = chat;
                        customer.IsBotCloseChat = false;
                        customer.TenantId = customer.TenantId;

                        if (!customer.IsOpen)
                        {
                            customer.IsHumanhandover=false;

                        }

                        if (customer.CustomerStepModel!=null)
                        {
                            customer.IsliveChat=false;
                        }
                        await itemsCollection.UpdateItemAsync(customer._self, customer);
                        return customer;
                    }
                }
                else
                {
                    result = "CustomerNotFound";
                }
                return null;

            }
            else
            {

                return null;

            }

        }
        public async Task<CustomerChat> GetLastMessage(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            var CustomerChatCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var chatConversation = await CustomerChatCollection.GetItemsAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == userId, null, int.MaxValue, 1);

            return chatConversation.Item1.LastOrDefault();
        }
        public async Task<CustomerChat> GetCustomersLastMessage(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);

            var message = await itemsCollection.GetItemOrderDescAsync(
                a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == userId,
                a => a.CreateDate);
            return message;
        }
      
        public async Task<CustomerModel> AssignTo(string contactId, int agentId, string agentName, bool isLocked, UserNotification userNotification)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);

            var chat= UpdateChatNotifications(userNotification, contactId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    customer.lastNotificationsData = userNotification.Notification.CreationTime;
                    customer.notificationID = userNotification.Id.ToString();
                    customer.notificationsText = userNotification.Notification.Data.Properties.FirstOrDefault().Value.ToString();
                    customer.IsOpen = !isLocked;
                    customer.agentId = agentId;
                    customer.IsLockedByAgent = !isLocked;
                    customer.LockedByAgentName = agentName;
                    customer.agentId = agentId;
                    customer.customerChat = chat;
                    customer.IsBotCloseChat = false;
                    customer.TenantId = customer.TenantId;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    
                    return customer;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return null;


        }
       
        public async Task<List<Contact>> GetContactsList(string UserId)
        {
            try
            {
                string connString = SettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Contacts] where UserId="+UserId;

                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<Contact> userModel = new List<Contact>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    userModel.Add(new Contact
                    {
                        UserId= Convert.ToString(dataSet.Tables[0].Rows[i]["UserId"]),
                        Id = int.Parse((dataSet.Tables[0].Rows[i]["Id"]).ToString()),
                        TenantId = int.Parse((dataSet.Tables[0].Rows[i]["TenantId"]).ToString()),
                        DisplayName = Convert.ToString(dataSet.Tables[0].Rows[i]["DisplayName"]),
                        PhoneNumber = Convert.ToString(dataSet.Tables[0].Rows[i]["PhoneNumber"]),
                        Description = Convert.ToString(dataSet.Tables[0].Rows[i]["Description"]),
                        EmailAddress = Convert.ToString(dataSet.Tables[0].Rows[i]["Description"]),
                    });
                }

                conn.Close();
                da.Dispose();
                return userModel;

            }
            catch
            {
                return null;

            }
           
        }
        public async Task<string> BlockCustomer(string contactId, int agentId, string agentName, bool isbloack)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    customer.isBlockCustomer = isbloack;
                    customer.agentId = agentId;
                    customer.blockByAgentName = agentName;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    result = "Done";
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return result;
        }
      
        public async Task<CustomerModel> UpdateIsSupport(string userId, bool IsOpen, string text, bool IsSupport)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);




       
            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                var chat = UpdateChatNotifications2(text, userId, customer);
              
                if (customer != null)
                {
                    customer.lastNotificationsData = DateTime.Now;
                    customer.notificationsText = text;
                    customer.IsLockedByAgent = IsOpen;
                    customer.IsOpen = IsOpen;
                    customer.IsSupport = IsSupport;
                    customer.customerChat = chat;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    return customer;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return null;

        }




        public async Task<CustomerModel> UpdateComplaintBot(string contactId, int agentId, bool IsComplaint)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {

                    customer.IsComplaint = IsComplaint;


                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    return customer;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return null;

        }







        public async Task<CustomerModel> UpdateComplaint(string contactId, int agentId, bool IsComplaint,string username)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);
            
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    customer.customerChat.type = "notification";
                    string lockedByAgentName = customer.customerChat.notificationsText;
                    if (customer.customerChat.notificationsText != null)
                    {
                        lockedByAgentName = customer.customerChat.notificationsText.Replace("Opened By", "");
                        lockedByAgentName = lockedByAgentName.Replace("Pinned On By", "");
                        lockedByAgentName = lockedByAgentName.Replace("Pinned Off By", "");
                    }

                    customer.IsComplaint = IsComplaint;

                    if (IsComplaint)
                    {
                        customer.customerChat.notificationsText = "Pinned On By " + username;
                        customer.customerChat.text = "Pinned On By " + username;
                    }
                    else
                    {
                        customer.customerChat.notificationsText = "Pinned Off By " + username;
                        customer.customerChat.text = "Pinned Off By " + username;
                    }
                    UpdateChatNotifications2(customer.customerChat.text, customer.userId, customer);

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    return customer;
                }
            }
            return null;

        }

        public async Task<CustomerModel> UpdateLiveChat(string contactId, int agentId, bool IsLiveChat, string Department1 = null, string Department2 = null)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contactId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    customer.IsliveChat = IsLiveChat;
                    customer.LiveChatStatus = 1;//pinding
                    customer.requestedLiveChatTime = DateTime.Now;

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    return customer;
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return null;

        }

        #region public contact in template
        public async Task<int> NickNameUpdateAsync(int tenantId, int contactID, string nickName = "")
        {
            try
            {
                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.ContactID == contactID.ToString());

                int result = 0;
                if (customerResult.IsCompletedSuccessfully)
                {
                    var customer = customerResult.Result;
                    if (customer != null)
                    {
                        var contact = UpdaateNickName(tenantId , contactID, nickName);

                        if (contact != 0)
                        {
                            customer.nickName = nickName;
                            await itemsCollection.UpdateItemAsync(customer._self, customer);
                            result = contactID;
                        }
                    }
                }
                return result;
            }
            catch
            {
                return 0;
            }
        }
        public List<ContactsTeamInboxs> ContactsTeamInbox(int tenantId, int pageNumber, int pageSize,string searchTerm = null)
        {
            try
            {
                List<ContactsTeamInboxs> contactsTeamInboxs = new List<ContactsTeamInboxs>();
                var SP_Name = Constants.Contacts.SP_ContactsTeamInpoxGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber", pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize) ,
                    new System.Data.SqlClient.SqlParameter("@searchTerm",searchTerm) ,
                };

                contactsTeamInboxs = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapContactGetAllTeamInpox, AppSettingsModel.ConnectionStrings).ToList();
                return contactsTeamInboxs;
            }
            catch
            {
                return null;
            }
        }
        private static ContactsTeamInboxs MapContactGetAllTeamInpox(IDataReader dataReader)
        {
            try
            {
                ContactsTeamInboxs model = new ContactsTeamInboxs();

                model.contactId = SqlDataHelper.GetValue<int>(dataReader, "Id");
                model.displayName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
                model.phoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                model.userId = SqlDataHelper.GetValue<string>(dataReader, "UserId");
                model.combinedValue = SqlDataHelper.GetValue<string>(dataReader, "CombinedPhoneNumberAndDisplayName");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsValidContact(string phoneNumber)
        {
            try
            {
                return isValidContact(phoneNumber);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region
        private int UpdaateNickName(int tenantId ,int contactID, string nickName = "")
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_NickNameUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@contactID", contactID) ,
                    new System.Data.SqlClient.SqlParameter("@nickName",nickName) ,
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
            }
            catch
            {
                return 0;
            }
        }
        #endregion
        #region private
        //private void UpdateContactInfo(ContactDto contact)
        //{
        //    try
        //    {
        //        _IContactsAppService.UpdateContactInfo(contact);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    //if (contact.PhoneNumber == null)
        //    //    contact.PhoneNumber = "";
        //    //if (contact.Description == null)
        //    //    contact.Description = "";
        //    //if (contact.EmailAddress == null)
        //    //    contact.EmailAddress = "";
        //    //if (contact.StreetName == null)
        //    //    contact.StreetName = "";
        //    //if (contact.BuildingNumber == null)
        //    //    contact.BuildingNumber = "";
        //    //if (contact.FloorNo == null)
        //    //    contact.FloorNo = "";
        //    //if (contact.ApartmentNumber == null)
        //    //    contact.ApartmentNumber = "";
        //    //if (contact.Website == null)
        //    //    contact.Website = "";
        //    //if (contact.DisplayName == null)
        //    //    contact.DisplayName = "";

        //    //string connString = SettingsModel.ConnectionStrings;
        //    //using (SqlConnection connection = new SqlConnection(connString))
        //    //    try
        //    //    {

        //    //        using (SqlCommand command = connection.CreateCommand())
        //    //        {

        //    //            command.CommandText = "UPDATE Contacts SET Website = @Website, DisplayName = @Dis, PhoneNumber = @Pho , EmailAddress = @Ema, Description = @Des , DeliveryOrder=@DeliveryOrder, TakeAwayOrder=@TakeAwayOrder, TotalOrder=@TotalOrder,loyalityPoint=@loyalityPoint ,StreetName=@StreetName,BuildingNumber=@BuildingNumber,FloorNo=@FloorNo,ApartmentNumber=@ApartmentNumber   Where Id = @Id";

        //    //            command.Parameters.AddWithValue("@Id", contact.Id);
        //    //            command.Parameters.AddWithValue("@Dis", contact.DisplayName);
        //    //            command.Parameters.AddWithValue("@Pho", contact.PhoneNumber);
        //    //            command.Parameters.AddWithValue("@Ema", contact.EmailAddress);
        //    //            command.Parameters.AddWithValue("@Des", contact.Description);
        //    //            command.Parameters.AddWithValue("@Website", contact.Website);

        //    //            command.Parameters.AddWithValue("@DeliveryOrder", contact.DeliveryOrder);
        //    //            command.Parameters.AddWithValue("@TakeAwayOrder", contact.TakeAwayOrder);
        //    //            command.Parameters.AddWithValue("@TotalOrder", contact.TotalOrder);
        //    //            command.Parameters.AddWithValue("@loyalityPoint", contact.loyalityPoint);

        //    //            command.Parameters.AddWithValue("@StreetName", contact.StreetName);
        //    //            command.Parameters.AddWithValue("@BuildingNumber", contact.BuildingNumber);
        //    //            command.Parameters.AddWithValue("@FloorNo", contact.FloorNo);
        //    //            command.Parameters.AddWithValue("@ApartmentNumber", contact.ApartmentNumber);

        //    //            connection.Open();
        //    //            command.ExecuteNonQuery();
        //    //            connection.Close();
        //    //        }
        //    //    }
        //    //    catch (Exception e)
        //    //    {


        //    //    }

        //}
        //private int  InsertContact(ContactDto contact)
        //{

        //    int modified = 0;
        //    string connString = AppSettingsModel.ConnectionStrings;
        //    //return orderCount.Count().ToString();
        //    using (SqlConnection connection = new SqlConnection(connString))
        //    using (SqlCommand command = connection.CreateCommand())
        //    {


        //        command.CommandText = "INSERT INTO Contacts (TenantId, AvatarUrl, DisplayName, PhoneNumber, SunshineAppID, IsLockedByAgent, LockedByAgentName, IsOpen,Website,EmailAddress,Description,ChatStatuseId,ContactStatuseId,CreationTime,CreatorUserId,DeleterUserId,DeletionTime,IsDeleted,LastModificationTime,LastModifierUserId,UserId,IsConversationExpired,IsBlock,ConversationsCount,ConversationId) " +
        //            " VALUES (@TenantId, @AvatarUrl, @DisplayName, @PhoneNumber, @SunshineAppID, @IsLockedByAgent, @LockedByAgentName, @IsOpen , @Website, @EmailAddress, @Description, @ChatStatuseId, @ContactStatuseId, @CreationTime, @CreatorUserId, @DeleterUserId, @DeletionTime, @IsDeleted, @LastModificationTime, @LastModifierUserId, @UserId, @IsConversationExpired, @IsBlock, @ConversationsCount,@ConversationId) ;SELECT SCOPE_IDENTITY();";

        //        command.Parameters.AddWithValue("@TenantId", contact.TenantId);
        //        command.Parameters.AddWithValue("@AvatarUrl", contact.AvatarUrl ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@DisplayName", contact.DisplayName);
        //        command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
        //        command.Parameters.AddWithValue("@SunshineAppID", contact.SunshineAppID);
        //        command.Parameters.AddWithValue("@IsLockedByAgent", contact.IsLockedByAgent);
        //        command.Parameters.AddWithValue("@LockedByAgentName", contact.LockedByAgentName ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@IsOpen", contact.IsOpen);
        //        command.Parameters.AddWithValue("@Website", contact.Website ?? Convert.DBNull);

        //        command.Parameters.AddWithValue("@EmailAddress", contact.EmailAddress ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@Description", contact.Description ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@ChatStatuseId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@ContactStatuseId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@CreationTime", contact.CreationTime);
        //        command.Parameters.AddWithValue("@CreatorUserId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@DeleterUserId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@DeletionTime", Convert.DBNull);
        //        command.Parameters.AddWithValue("@IsDeleted", contact.IsDeleted);

        //        command.Parameters.AddWithValue("@LastModificationTime", Convert.DBNull);
        //        command.Parameters.AddWithValue("@LastModifierUserId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@UserId", contact.UserId);
        //        command.Parameters.AddWithValue("@IsConversationExpired", contact.IsConversationExpired);
        //       // command.Parameters.AddWithValue("@ConversationId", contact.ConversationId);
        //        command.Parameters.AddWithValue("@IsBlock", contact.IsBlock);

        //        command.Parameters.AddWithValue("@ConversationsCount", contact.ConversationsCount);
        //        command.Parameters.AddWithValue("@ConversationId", !string.IsNullOrEmpty(contact.ConversationId)?contact.ConversationId:Convert.DBNull);

        //        connection.Open();
        //        modified = Convert.ToInt32(command.ExecuteScalar());
        //        if (connection.State == System.Data.ConnectionState.Open) connection.Close();


        //        return modified;

        //    }


        //}
        private CustomerChat UpdateChatNotifications(UserNotification userNotification, string contactId)
        {
            var itemsCollectionCh = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);

            CustomerChat customerCh = new CustomerChat
            {
                lastNotificationsData = userNotification.Notification.CreationTime,
                notificationsText = userNotification.Notification.Data.Properties.FirstOrDefault().Value.ToString(),
                notificationID = userNotification.Notification.Id.ToString(),
                userId = contactId,
                sender = MessageSenderType.TeamInbox,
                type = "notification",
                CreateDate = userNotification.Notification.CreationTime,
                text = userNotification.Notification.Data.Properties.FirstOrDefault().Value.ToString()

            };
            var Result = itemsCollectionCh.CreateItemAsync(customerCh).Result;

            return customerCh;
        }
        private CustomerChat UpdateChatNotifications2(string text, string contactId, CustomerModel customer)
        {
            var itemsCollectionCh = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);

            CustomerChat customerCh = new CustomerChat
            {
                lastNotificationsData = DateTime.Now,
                notificationsText = text,
                userId = customer.userId,
                status = 1,
                sender = MessageSenderType.TeamInbox,
                type = "notification",
                CreateDate = DateTime.Now,
                text = text,
                ItemType= InfoSeedContainerItemTypes.ConversationItem,
                TenantId=customer.TenantId,


            };
            var Result = itemsCollectionCh.CreateItemAsync(customerCh).Result;

            return customerCh;
        }

        private static ConversationSessionsModel getConversationSessions(int TenantId, string PhoneNumber, string ConversationID)
        {
            try
            {


                ConversationSessionsModel ConversationSessionsEntity = new ConversationSessionsModel();
                var SP_Name = "GetConversationSessions";    

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@ConversationID",ConversationID)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",PhoneNumber)
                };

                ConversationSessionsEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapConversationSessions, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                return ConversationSessionsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static ConversationSessionsModel MapConversationSessions(IDataReader dataReader)
        {
            try
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = SqlDataHelper.GetValue<int>(dataReader, "creation_timestamp");
                model.expiration_timestamp = SqlDataHelper.GetValue<int>(dataReader, "expiration_timestamp");
                // model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");

                return model;
            }
            catch (Exception)
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = 0;
                model.expiration_timestamp = 0;
                // model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");

                return model;
            }
        }

        private bool isValidContact(string phoneNumber)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_CheckPhoneNumberFormat;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@phoneNumber",phoneNumber)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToBoolean(OutputParameter.Value) : false;
            }
            catch
            {
                return false;
            }
        }


        #endregion


    }
}
