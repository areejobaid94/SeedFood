using Infoseed.MessagingPortal.ChatStatuses;
using Infoseed.MessagingPortal.ContactStatuses;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Contacts.Exporting;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Framework.Data;
using Abp.Notifications;
using Infoseed.MessagingPortal.Notifications;
using System.Data.SqlClient;
using Infoseed.MessagingPortal.Orders;
using System.Data;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Abp.Extensions;
using Infoseed.MessagingPortal.General;
using DocumentFormat.OpenXml.Wordprocessing;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Infoseed.MessagingPortal.SocketIOClient;
using DocumentFormat.OpenXml.Bibliography;
using MailKit.Search;
using MongoDB.Driver;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using MongoDB.Bson;

namespace Infoseed.MessagingPortal.Contacts
{
    [AbpAuthorize(AppPermissions.Pages_Contacts)]
    public class ContactsAppService : MessagingPortalAppServiceBase, IContactsAppService
    {
        //private IUserNotificationManager _userNotificationManager;
        //private IAppNotifier _appNotifier;
        private readonly IExcelExporterAppService _excelExporterAppServicer;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IContactsExcelExporter _contactsExcelExporter;
        private readonly IRepository<ChatStatuse, int> _lookup_chatStatuseRepository;
        private readonly IRepository<ContactStatuse, int> _lookup_contactStatuseRepository;
        //private readonly IRepository<Order, long> _orderRepository;

        private readonly ContactsSyncService _contactsSyncService;
        private readonly IDocumentClient _IDocumentClient;


        public ContactsAppService(IRepository<Contact> contactRepository, IContactsExcelExporter contactsExcelExporter, IRepository<ChatStatuse,
            int> lookup_chatStatuseRepository, IRepository<ContactStatuse, int> lookup_contactStatuseRepository,
            IUserNotificationManager userNotificationManager,
            IAppNotifier appNotifier,
            IRepository<Order, long> orderRepository,
            IDocumentClient iDocumentClient,
            IExcelExporterAppService excelExporterAppService
            )

        {
            //_userNotificationManager = userNotificationManager;
            //_appNotifier = appNotifier;
            _contactRepository = contactRepository;
            _contactsExcelExporter = contactsExcelExporter;
            _lookup_chatStatuseRepository = lookup_chatStatuseRepository;
            _lookup_contactStatuseRepository = lookup_contactStatuseRepository;
            //_orderRepository = orderRepository;
            _IDocumentClient = iDocumentClient;

            _contactsSyncService = new ContactsSyncService(_contactRepository, _IDocumentClient);
            _excelExporterAppServicer = excelExporterAppService;

        }

        public ContactEntity GetContact(int pageNumber = 0, int pageSize = 50, string name = null, string phoneNumber = null, int? selectedStatus = null)
        {
            return getContact(pageNumber, pageSize, name, phoneNumber, selectedStatus);
        }
        [AbpAuthorize(AppPermissions.Pages_Contacts_Edit)]
        public async Task<ContactDto> BlockContact(int contactId, bool isBlock, string username)
        {
            try
            {
                ContactDto contact = getContactbyId(contactId);
                contact.IsBlock = isBlock;
                try
                {
                    if (contact.UserId == null || contact.UserId == "")
                    {
                        contact.UserId = contact.TenantId.ToString() + "_" + contact.PhoneNumber;
                    }
                    var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contact.UserId);

                    string result = string.Empty;
                    if (customerResult.IsCompletedSuccessfully)
                    {
                        var customer = customerResult.Result;
                        var message = "";
                        if (isBlock)
                        {
                            message = "User Blocked By : " + username;
                        }
                        else
                        {
                            message = "User Un Blocked By : " + username;
                        }

                        var chat = UpdateChatNotifications2(message, contactId.ToString(), customer);

                        if (customer != null)
                        {
                            customer.IsBlock = contact.IsBlock;
                            customer.ContactID = contact.Id.ToString();
                            customer.customerChat=chat;
                            await itemsCollection.UpdateItemAsync(customer._self, customer);

                            // update contact to database 
                            updateContactInfo(contact);
                            //await _contactRepository.UpdateAsync(contact);
                            SocketIOManager.SendChat(customer, customer.TenantId.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return contact;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task CreateOrEdit(ContactDto input)
        {
            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Contacts_Create)]
        protected virtual async Task Create(ContactDto input)
        {
            var contact = ObjectMapper.Map<Contact>(input);

            if (AbpSession.TenantId != null)
            {
                contact.TenantId = (int?)AbpSession.TenantId;
            }

            await _contactRepository.InsertAsync(contact);

            await _contactsSyncService.CreateNewCustomer(contact);


        }

        [AbpAuthorize(AppPermissions.Pages_Contacts_Edit)]
        protected virtual async Task Update(ContactDto input)
        {
            try
            {
                if (input.UserId == null || input.UserId == "")
                {
                    input.UserId = input.TenantId.ToString() + "_" + input.PhoneNumber;
                }
                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == input.UserId);

                string result = string.Empty;
                if (customerResult.IsCompletedSuccessfully)
                {
                    var customer = customerResult.Result;
                    if (customer != null)
                    {
                        customer.IsBlock = input.IsBlock;
                        customer.ContactID = input.Id.ToString();
                        customer.displayName = input.DisplayName;
                        customer.phoneNumber = input.PhoneNumber;
                        customer.EmailAddress = input.EmailAddress;
                        customer.Website = input.Website;
                        customer.Description = input.Description;
                        customer.userId = input.UserId;
                        await itemsCollection.UpdateItemAsync(customer._self, customer);

                        // update contact to database 
                        updateContactInfo(input);
                        //await _contactRepository.UpdateAsync(contact);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CustomerChat UpdateChatNotifications2(string text, string contactId, CustomerModel customer)
        {
            var itemsCollectionCh = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);

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
        [AbpAuthorize(AppPermissions.Pages_Contacts_Delete)]
        public async Task Delete(EntityDto input)
        {
            // var contact = GetContactId((long)input.Id);
            var contact = getContactbyId(input.Id);
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contact.UserId);
            var customer = new CustomerModel();
            try
            {
                customer = customerResult.Result;
            }
            catch
            {
                customer = null;

            }

            //delete order for user
            DeleteOrder(AbpSession.TenantId, contact.Id);

            try
            {
                // delete contact caht 
                var queryString = "SELECT * FROM c WHERE c.TenantId=" + contact.TenantId + " and c.userId='" + customer.userId + "'";
                await itemsCollection.DeleteChatItem(queryString);
                // delete teaminbox caht 
                var queryString2 = "SELECT * FROM c WHERE c.ItemType= 1 and c.userId='" + customer.userId + "'";
                await itemsCollection.DeleteChatItem(queryString2);

            }
            catch
            {


            }
            //delete contact from cosmoDB
            await itemsCollection.DeleteItem(contact.UserId, contact.TenantId);

            //delete contact from DB
            deleteContact(input.Id);



            try
            {
                //delete bot conversation
                var conversationChat = new DocumentCosmoseDB<ConversationChatBotModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.SunshineConversationId == customer.SunshineAppID).Result;
                if (objConversation != null)
                {

                    var queryString2 = "SELECT * FROM c WHERE c.ItemType= 3 and c.MicrosoftBotId='" + objConversation.MicrosoftBotId + "'";
                    await itemsCollection.DeleteChatItem(queryString2);
                }

            }
            catch
            {

            }

        }
        [HttpDelete]
        public async Task<Dictionary<string, dynamic>> ContactDelete(EntityDto input)
        {
            try
            {
                return await contactDelete(input);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        [HttpPut]
        public async Task<Dictionary<string, dynamic>> PhoneNumberUpdate(int contactId, string phoneNumber)
        {
            try
            {
                return await contactUpdate(contactId, phoneNumber);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        public async Task<bool> DeleteContactChat(EntityDto input)
        {
            var contact = await _contactRepository.FirstOrDefaultAsync((int)input.Id);
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contact.UserId);
            var customer = customerResult.Result;

            //if (customer != null)
            //{
            // delete contact caht 
            var queryString = "SELECT * FROM c WHERE c.TenantId=" +contact.TenantId + " and c.userId= '"+contact.UserId+"'";
            await itemsCollection.DeleteChatItem(queryString);
            // delete teaminbox caht 
            var queryString2 = "SELECT * FROM c WHERE c.ItemType= 1 and c.userId= '"+contact.UserId+"'";
            await itemsCollection.DeleteChatItem(queryString2);
            return true;
            // }
            //else
            //{
            //    return false;

            //}
        }
        public Task<FileDto> GetContactsToExcel(int pageNumber = 0, int pageSize = 50, string name = null, string phoneNumber = null, int? selectedStatus = null)
        {
            try
            {
                List<ContactDto> lstContacts = getContact(0, int.MaxValue, name, phoneNumber, selectedStatus).lstContacts;
                return Task.FromResult(_contactsExcelExporter.ExportToFile(lstContacts));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //var filteredContacts = _contactRepository.GetAll()
            //            .Include(e => e.ChatStatuseFk)
            //            .Include(e => e.ContactStatuseFk)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.AvatarUrl.Contains(input.Filter) || e.DisplayName.Contains(input.Filter) || e.PhoneNumber.Contains(input.Filter) || e.SunshineAppID.Contains(input.Filter) || e.LockedByAgentName.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.EmailAddress.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.UserId.Contains(input.Filter))
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneNumberFilter), e => e.PhoneNumber == input.PhoneNumberFilter)
            //            .WhereIf(input.IsLockedByAgentFilter.HasValue && input.IsLockedByAgentFilter > -1, e => (input.IsLockedByAgentFilter == 1 && e.IsLockedByAgent) || (input.IsLockedByAgentFilter == 0 && !e.IsLockedByAgent))
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.LockedByAgentNameFilter), e => e.LockedByAgentName == input.LockedByAgentNameFilter)
            //            .WhereIf(input.IsOpenFilter.HasValue && input.IsOpenFilter > -1, e => (input.IsOpenFilter == 1 && e.IsOpen) || (input.IsOpenFilter == 0 && !e.IsOpen))
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.EmailAddressFilter), e => e.EmailAddress == input.EmailAddressFilter)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.UserIdFilter), e => e.UserId == input.UserIdFilter)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.ChatStatuseChatStatusNameFilter), e => e.ChatStatuseFk != null && e.ChatStatuseFk.ChatStatusName == input.ChatStatuseChatStatusNameFilter)
            //            .WhereIf(!string.IsNullOrWhiteSpace(input.ContactStatuseContactStatusNameFilter), e => e.ContactStatuseFk != null && e.ContactStatuseFk.ContactStatusName == input.ContactStatuseContactStatusNameFilter);

            //var query = (from o in filteredContacts
            //             join o1 in _lookup_chatStatuseRepository.GetAll() on o.ChatStatuseId equals o1.Id into j1
            //             from s1 in j1.DefaultIfEmpty()

            //             join o2 in _lookup_contactStatuseRepository.GetAll() on o.ContactStatuseId equals o2.Id into j2
            //             from s2 in j2.DefaultIfEmpty()

            //             select new GetContactForViewDto()
            //             {
            //                 Contact = new ContactDto
            //                 {
            //                     DisplayName = o.DisplayName,
            //                     PhoneNumber = o.PhoneNumber,
            //                     IsLockedByAgent = o.IsLockedByAgent,
            //                     LockedByAgentName = o.LockedByAgentName,
            //                     IsOpen = o.IsOpen,
            //                     EmailAddress = o.EmailAddress,
            //                     Address = o.Description,
            //                     Id = o.Id
            //                 },
            //                 ChatStatuseChatStatusName = s1 == null || s1.ChatStatusName == null ? "" : s1.ChatStatusName.ToString(),
            //                 ContactStatuseContactStatusName = s2 == null || s2.ContactStatusName == null ? "" : s2.ContactStatusName.ToString()
            //             });

            //var contactListDtos = await query.ToListAsync();
        }
        public async Task<FileDto> BackUpConversation(ContactDto input)
        {
            try
            {
                //var contact = GetContact((int)input.Id);// await _contactRepository.FirstOrDefaultAsync((int)input.Id);
                //var contact = getContactbyId(input.Id);
                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == input.UserId);
                var customer = customerResult.Result;

                if (customer != null)
                {
                    var itemsCollectionchat = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var chatConversation = await itemsCollectionchat.GetItemsAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == customer.userId, null, int.MaxValue, 0);



                    List<BackUpConversationModel> backUpConversationModels = new List<BackUpConversationModel>();

                    foreach (var item in chatConversation.Item1)
                    {

                        if (item.sender == Tenants.Contacts.MessageSenderType.Customer)
                        {
                            backUpConversationModels.Add(new BackUpConversationModel
                            {

                                UserName = customer.displayName,
                                PhoneNumber = customer.phoneNumber,
                                Text = item.text,
                                TextDate = item.CreateDate.Value,
                                MediaUrl = item.mediaUrl
                            });
                        }
                        else
                        {
                            backUpConversationModels.Add(new BackUpConversationModel
                            {

                                UserName = item.agentName,
                                PhoneNumber = customer.phoneNumber,
                                Text = item.text,
                                TextDate = item.CreateDate.Value,
                                MediaUrl = item.mediaUrl
                            });

                        }

                    }

                    return _contactsExcelExporter.BackUpConversation(backUpConversationModels);
                }
                else
                {
                    List<BackUpConversationModel> backUpConversationModels = new List<BackUpConversationModel>();
                    return _contactsExcelExporter.BackUpConversation(backUpConversationModels);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<FileDto> BackUpConversationForAll()
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            List<CustomerModel> customerList = new List<CustomerModel>();

            var customers = await itemsCollection.GetItemsRAsync(a => a.TenantId == AbpSession.TenantId  && a.ItemType == InfoSeedContainerItemTypes.CustomerItem, null, int.MaxValue, 0, a => a.LastMessageData);

            customerList = customers.Item1.ToList();


            List<BackUpConversationModel> backUpConversationModels = new List<BackUpConversationModel>();
            foreach (var item in customerList)
            {
                var itemsCollectionchat = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var chatConversation = await itemsCollectionchat.GetItemsAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == item.userId, null, int.MaxValue, 0);

                if (!item.displayName.IsNullOrEmpty())
                {

                    foreach (var itemChat in chatConversation.Item1)
                    {

                        if (itemChat.type!="notification")
                        {
                            if (itemChat.sender == Tenants.Contacts.MessageSenderType.Customer)
                            {
                                backUpConversationModels.Add(new BackUpConversationModel
                                {
                                    UserName = item.displayName,
                                    PhoneNumber = item.phoneNumber,
                                    Text = itemChat.text,
                                    TextDate = itemChat.CreateDate.Value,
                                    MediaUrl = itemChat.mediaUrl
                                });
                            }
                            else
                            {

                                backUpConversationModels.Add(new BackUpConversationModel
                                {
                                    UserName = itemChat.agentName,
                                    PhoneNumber = item.phoneNumber,
                                    Text = itemChat.text,
                                    TextDate = itemChat.CreateDate.Value,
                                    MediaUrl = itemChat.mediaUrl
                                });
                            }

                        }

                    }



                }


                if (chatConversation.Item1.Count()>0)
                {
                    backUpConversationModels.Add(new BackUpConversationModel
                    {
                        UserName = "*******",
                        PhoneNumber = "*******",
                        Text ="*******",
                        TextDate =  DateTime.Now,
                        MediaUrl = "*******"
                    });


                }

            }
            var rez = backUpConversationModels.Where(x => x.UserName!=null && x.UserName!="").ToList();
            return _contactsExcelExporter.BackUpConversation(rez);
        }

        public ContactDto GetContactbyId(int id)
        {
            return getContactbyId(id);
        }
        public void UpdateContact(ContactDto contactDto)
        {
            updateContact(contactDto);
        }
        //public void UpdateContactInfo(ContactDto contactDto)
        //{
        //    updateContactInfo(contactDto);
        //}
        public int CreateContact(ContactDto contactDto)
        {
            return createContact(contactDto);
        }
        public ContactsCampaignEntity GetContactsCampaign(int tenantId, int pageNumber = 0, int pageSize = 50, string phone = null, long? templateId = null, long? campaignId = null, bool? isSent = null, bool? isDelivered = null, bool? isRead = null, bool? isFailed = null, bool? isHanged = null)
        {
            return getContactsCampaign(tenantId, pageNumber, pageSize, phone, templateId, campaignId, isSent, isDelivered, isRead, isFailed, isHanged);
        }
        [HttpGet]
        public async Task<ContactsCampaignEntity> ContactsCampaignGet(ContactsCampaignFilter contactsCampaignFilter)
        {
            return await contactsCampaignGet(contactsCampaignFilter);
        }
        public List<ContactsInterestedOfModel> GetContactsInterested(int tenantId, int contactId)
        {
            return getContactsInterested(tenantId, contactId);
        }

        public bool CheckIfExistContactByPhoneNumber(string phoneNumber)
        {
            return checkIfExistContactByPhoneNumber(phoneNumber);
        }
        public async Task<FileDto> ExportContactCampaignToExcel(long? templateId, long? campaignId, bool? isSent = null, bool? isDelivered = null, bool? isRead = null, bool? isFailed = null, bool? isHanged = null)
        {
            var result = new ContactsCampaignEntity { contacts = new List<ContactCampaignDto>() };


            int statusId = 0;

            if (isSent==true)
            {
                statusId=1;
            }
            if (isDelivered==true)
            {
                statusId=2;
            }
            if (isRead==true)
            {
                statusId=3;
            }
            if (isFailed==true)
            {
                statusId=4;
            }

            try
            {
                var mongoClient = new MongoClient(AppSettingsModel.connectionStringMongoDB);
                var database = mongoClient.GetDatabase(AppSettingsModel.databaseName);
                var collection = database.GetCollection<CampaginMongoModel>(AppSettingsModel.collectionName);

                var baseFilter = Builders<CampaginMongoModel>.Filter.Eq(x => x.tenantId, AbpSession.TenantId.Value.ToString());

                FilterDefinition<CampaginMongoModel> finalFilter = Builders<CampaginMongoModel>.Filter.Ne(x => x.campaignId, "1") & baseFilter;

                if (!string.IsNullOrEmpty(campaignId?.ToString()))
                    finalFilter &= Builders<CampaginMongoModel>.Filter.Eq(x => x.campaignId, campaignId.ToString());

                //if (!string.IsNullOrEmpty(filter.phone))
                //    finalFilter &= Builders<CampaginMongoModel>.Filter.Eq(x => x.phoneNumber, filter.phone);

                // Apply status filter
                finalFilter &= statusId switch
                {
                    1 => Builders<CampaginMongoModel>.Filter.Eq(x => x.is_sent, true),
                    2 => Builders<CampaginMongoModel>.Filter.Eq(x => x.is_delivered, true),
                    3 => Builders<CampaginMongoModel>.Filter.Eq(x => x.is_read, true),
                    4 => Builders<CampaginMongoModel>.Filter.Eq(x => x.is_sent, false) &
                         Builders<CampaginMongoModel>.Filter.Eq(x => x.is_delivered, false) &
                         Builders<CampaginMongoModel>.Filter.Eq(x => x.is_read, false),
                    _ => Builders<CampaginMongoModel>.Filter.Empty
                };

                var sort = Builders<CampaginMongoModel>.Sort.Descending("createdAt");

                var pagedDocs = await collection.Find(finalFilter)
                    .Sort(sort)
                    .Skip(0)
                    .Limit(int.MaxValue)
                    .ToListAsync();

                foreach (var doc in pagedDocs)
                {
                    var campaign = GetCampaignFun(long.Parse(doc.campaignId)).FirstOrDefault();

                    result.contacts.Add(new ContactCampaignDto
                    {
                        Id = 0,
                        PhoneNumber = doc.phoneNumber,
                        TemplateName = campaign?.templateName ?? "not found",
                        CampaignName = campaign?.campaignName ?? "not found",
                        IsSent = doc.is_sent,
                        IsDelivered = doc.is_delivered,
                        IsRead = doc.is_read,
                        IsFailed = !doc.is_read && !doc.is_delivered && !doc.is_sent,
                        IsReplied = false,
                        IsHanged = false
                    });
                }

                result.TotalCount = (int)await collection.CountDocumentsAsync(finalFilter);
            }
            catch (Exception ex)
            {
                throw;
            }

            return _excelExporterAppServicer.ExportContactCampaignToFile(result.contacts);
            //var campaignCosmoDBModel = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            //var campaignCosmoResult = await campaignCosmoDBModel.GetItemsRAsync(a =>
            //        a.tenantId == AbpSession.TenantId.Value
            //        && a.itemType == 5
            //        && (a.templateId == templateId.ToString())
            //        && (a.campaignId == campaignId.ToString())
            //    , null, int.MaxValue, 0, x => x.sendTime);

            //var campaignCosmo = campaignCosmoResult.Item1.ToList();
            //List<ContactCampaignDto> contacts = campaignCosmo.Select(a => new ContactCampaignDto
            //{
            //    PhoneNumber = a.phoneNumber,
            //    IsSent = a.isSent,
            //    IsDelivered =a.isDelivered,
            //    IsRead =a.isRead,
            //    IsFailed =a.isFailed,
            //    IsReplied = a.isReplied,
            //    IsHanged = (a.isSent && !a.isRead && !a.isDelivered && !a.isFailed),
            //    TemplateName = a.templateName,
            //    CampaignName = a.campaignName
            //}).ToList();
            //if (contacts != null)
            //{
            //    return _excelExporterAppServicer.ExportContactCampaignToFile(contacts);
            //}
            //else
            //{
            //    return new FileDto();
            //}
        }

        public List<WhatsAppContactsDto> GetOptOutContactByTenantId(int tenantId)
        {
            return getOptOutContactByTenantId(tenantId);
        }
        private List<ContactCampaignDto> getContactCampaignToExcel(long templateId, long campaignId, bool? isSent = null, bool? isDelivered = null, bool? isRead = null, bool? isFailed = null, bool? isHanged = null)
        {
            try
            {
                List<ContactCampaignDto> contacts = new List<ContactCampaignDto>();
                var SP_Name = Constants.Contacts.SP_ContactsCampaignToExcelGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TemplateId", templateId),
                    new System.Data.SqlClient.SqlParameter("@CampaignId", campaignId),
                    new System.Data.SqlClient.SqlParameter("@IsSent", isSent),
                    new System.Data.SqlClient.SqlParameter("@IsDelivered", isDelivered),
                    new System.Data.SqlClient.SqlParameter("@IsRead", isRead),
                    new System.Data.SqlClient.SqlParameter("@IsFailed", isFailed),
                    new System.Data.SqlClient.SqlParameter("@IsHanged", isHanged),
                };

                contacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapContactCampaign, AppSettingsModel.ConnectionStrings).ToList();
                return contacts;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #region Checked Apis
        public async Task<PagedResultDto<GetContactForViewDto>> GetAll(GetAllContactsInput input)
        {

            var filteredContacts = _contactRepository.GetAll()
                        .Include(e => e.ChatStatuseFk)
                        .Include(e => e.ContactStatuseFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.AvatarUrl.Contains(input.Filter) || e.DisplayName.Contains(input.Filter) || e.PhoneNumber.Contains(input.Filter) || e.SunshineAppID.Contains(input.Filter) || e.LockedByAgentName.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.EmailAddress.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.UserId.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneNumberFilter), e => e.PhoneNumber == input.PhoneNumberFilter)
                        .WhereIf(input.IsLockedByAgentFilter.HasValue && input.IsLockedByAgentFilter > -1, e => (input.IsLockedByAgentFilter == 1 && e.IsLockedByAgent) || (input.IsLockedByAgentFilter == 0 && !e.IsLockedByAgent))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LockedByAgentNameFilter), e => e.LockedByAgentName == input.LockedByAgentNameFilter)
                        .WhereIf(input.IsOpenFilter.HasValue && input.IsOpenFilter > -1, e => (input.IsOpenFilter == 1 && e.IsOpen) || (input.IsOpenFilter == 0 && !e.IsOpen))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailAddressFilter), e => e.EmailAddress == input.EmailAddressFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserIdFilter), e => e.UserId == input.UserIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ChatStatuseChatStatusNameFilter), e => e.ChatStatuseFk != null && e.ChatStatuseFk.ChatStatusName == input.ChatStatuseChatStatusNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactStatuseContactStatusNameFilter), e => e.ContactStatuseFk != null && e.ContactStatuseFk.ContactStatusName == input.ContactStatuseContactStatusNameFilter);

            var pagedAndFilteredContacts = filteredContacts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var contacts = from o in pagedAndFilteredContacts
                           join o1 in _lookup_chatStatuseRepository.GetAll() on o.ChatStatuseId equals o1.Id into j1
                           from s1 in j1.DefaultIfEmpty()

                           join o2 in _lookup_contactStatuseRepository.GetAll() on o.ContactStatuseId equals o2.Id into j2
                           from s2 in j2.DefaultIfEmpty()

                           select new GetContactForViewDto()
                           {
                               Contact = new ContactDto
                               {

                                   DisplayName = o.DisplayName,
                                   PhoneNumber = o.PhoneNumber,
                                   IsLockedByAgent = o.IsLockedByAgent,
                                   LockedByAgentName = o.LockedByAgentName,
                                   IsBlock = o.IsBlock,
                                   IsOpen = o.IsOpen,
                                   EmailAddress = o.EmailAddress,
                                   Address = o.Description,

                                   Id = o.Id
                               },
                               ChatStatuseChatStatusName = s1 == null || s1.ChatStatusName == null ? "" : s1.ChatStatusName.ToString(),
                               ContactStatuseContactStatusName = s2 == null || s2.ContactStatusName == null ? "" : s2.ContactStatusName.ToString(),
                               loyalityPoint = o.loyalityPoint,
                               TakeAwayOrder = o.TakeAwayOrder,
                               DeliveryOrder = o.DeliveryOrder,
                               TotalOrder = o.TotalOrder,

                           };


            var contactList = await contacts.ToListAsync();



            var totalCount = await filteredContacts.CountAsync();

            return new PagedResultDto<GetContactForViewDto>(
                totalCount,
                contactList
            );
        }
        public async Task<GetContactForViewDto> GetContactForView(int id)
        {
            var contact = await _contactRepository.GetAsync(id);

            var output = new GetContactForViewDto { Contact = ObjectMapper.Map<ContactDto>(contact) };

            if (output.Contact.ChatStatuseId != null)
            {
                var _lookupChatStatuse = await _lookup_chatStatuseRepository.FirstOrDefaultAsync((int)output.Contact.ChatStatuseId);
                output.ChatStatuseChatStatusName = _lookupChatStatuse?.ChatStatusName?.ToString();
            }

            if (output.Contact.ContactStatuseId != null)
            {
                var _lookupContactStatuse = await _lookup_contactStatuseRepository.FirstOrDefaultAsync((int)output.Contact.ContactStatuseId);
                output.ContactStatuseContactStatusName = _lookupContactStatuse?.ContactStatusName?.ToString();
            }

            return output;
        }

        //[AbpAuthorize(AppPermissions.Pages_Contacts_Edit)]
        public async Task<GetContactForEditOutput> GetContactForEdit(EntityDto input)
        {
            try
            {
                var contact = await _contactRepository.FirstOrDefaultAsync(input.Id);
                if (contact.UserId.IsNullOrEmpty())
                {
                    contact.UserId = contact.TenantId + "_" + contact.PhoneNumber;
                }
                var output = new GetContactForEditOutput { Contact = ObjectMapper.Map<CreateOrEditContactDto>(contact) };

                if (output.Contact.ChatStatuseId != null)
                {
                    var _lookupChatStatuse = await _lookup_chatStatuseRepository.FirstOrDefaultAsync((int)output.Contact.ChatStatuseId);
                    output.ChatStatuseChatStatusName = _lookupChatStatuse?.ChatStatusName?.ToString();
                }

                if (output.Contact.ContactStatuseId != null)
                {
                    var _lookupContactStatuse = await _lookup_contactStatuseRepository.FirstOrDefaultAsync((int)output.Contact.ContactStatuseId);
                    output.ContactStatuseContactStatusName = _lookupContactStatuse?.ContactStatusName?.ToString();
                }

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        //[AbpAuthorize(AppPermissions.Pages_Contacts)]
        public async Task<PagedResultDto<ContactChatStatuseLookupTableDto>> GetAllChatStatuseForLookupTable(Dtos.GetAllForLookupTableInput input)
        {
            var query = _lookup_chatStatuseRepository.GetAll().WhereIf(
                  !string.IsNullOrWhiteSpace(input.Filter),
                 e => e.ChatStatusName != null && e.ChatStatusName.Contains(input.Filter)
              );

            var totalCount = await query.CountAsync();

            var chatStatuseList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ContactChatStatuseLookupTableDto>();
            foreach (var chatStatuse in chatStatuseList)
            {
                lookupTableDtoList.Add(new ContactChatStatuseLookupTableDto
                {
                    Id = chatStatuse.Id,
                    DisplayName = chatStatuse.ChatStatusName?.ToString()
                });
            }

            return new PagedResultDto<ContactChatStatuseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
        //[AbpAuthorize(AppPermissions.Pages_Contacts)]
        public async Task<PagedResultDto<ContactContactStatuseLookupTableDto>> GetAllContactStatuseForLookupTable(Dtos.GetAllForLookupTableInput input)
        {
            var query = _lookup_contactStatuseRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ContactStatusName != null && e.ContactStatusName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var contactStatuseList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ContactContactStatuseLookupTableDto>();
            foreach (var contactStatuse in contactStatuseList)
            {
                lookupTableDtoList.Add(new ContactContactStatuseLookupTableDto
                {
                    Id = contactStatuse.Id,
                    DisplayName = contactStatuse.ContactStatusName?.ToString()
                });
            }

            return new PagedResultDto<ContactContactStatuseLookupTableDto>(totalCount, lookupTableDtoList);
        }
        #endregion

        #region Private Methods
        private ContactEntity getContact(int pageNumber = 0, int pageSize = 50, string name = null, string phoneNumber = null, int? selectedStatus = null)
        {
            try
            {

                ContactEntity contacts = new ContactEntity();
                var SP_Name = Constants.Contacts.SP_ContactsGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                      new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                     ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                     ,new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                     ,new System.Data.SqlClient.SqlParameter("@Name",name)
                     ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",phoneNumber)

                };
                if (selectedStatus.HasValue)
                {
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@Status", selectedStatus.Value));
                }

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                contacts.lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapContact, AppSettingsModel.ConnectionStrings).ToList();
                contacts.TotalCount = (int)OutputParameter.Value;
                return contacts;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<WhatsAppContactsDto> getOptOutContactByTenantId(int tenantId)
        {
            try
            {
                List<WhatsAppContactsDto> contacts = new List<WhatsAppContactsDto>();
                var SP_Name = Constants.Contacts.SP_ContactsOptOutByTenantIdGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                      new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };

                contacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapExternalContacts, AppSettingsModel.ConnectionStrings).ToList();
                return contacts;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private ContactsCampaignEntity getContactsCampaign(int tenantId, int pageNumber = 0, int pageSize = 50, string phone = null, long? templateId = null, long? campaignId = null, bool? isSent = null, bool? isDelivered = null, bool? isRead = null, bool? isFailed = null, bool? isHanged = null)
        {
            try
            {
                ContactsCampaignEntity contacts = new ContactsCampaignEntity();
                var SP_Name = Constants.Contacts.SP_ContactsExternalGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId", tenantId),
                    new System.Data.SqlClient.SqlParameter("@PageNumber", pageNumber),
                    new System.Data.SqlClient.SqlParameter("@PageSize", pageSize),
                    new System.Data.SqlClient.SqlParameter("@Phone", phone),
                    new System.Data.SqlClient.SqlParameter("@TemplateId", templateId),
                    new System.Data.SqlClient.SqlParameter("@CampaignId", campaignId),
                    new System.Data.SqlClient.SqlParameter("@IsSent", isSent),
                    new System.Data.SqlClient.SqlParameter("@IsDelivered", isDelivered),
                    new System.Data.SqlClient.SqlParameter("@IsRead", isRead),
                    new System.Data.SqlClient.SqlParameter("@IsFailed", isFailed),
                    new System.Data.SqlClient.SqlParameter("@IsHanged", isHanged)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);

                contacts.contacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapContactCampaign, AppSettingsModel.ConnectionStrings).ToList();
                contacts.TotalCount = Convert.ToInt32(OutputParameter.Value);
                return contacts;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task<ContactsCampaignEntity> contactsCampaignGet(ContactsCampaignFilter filter)
        {
            var result = new ContactsCampaignEntity { contacts = new List<ContactCampaignDto>() };

            try
            {
                var mongoClient = new MongoClient(AppSettingsModel.connectionStringMongoDB);
                var database = mongoClient.GetDatabase(AppSettingsModel.databaseName);
                var collection = database.GetCollection<CampaginMongoModel>(AppSettingsModel.collectionName);

                // Base filter: tenantId in ["517", 517] and campaignId != "1"
                var filterDoc = new BsonDocument
{
    {
        "tenantId", new BsonDocument("$in", new BsonArray
        {
            AbpSession.TenantId.Value.ToString(),
            AbpSession.TenantId.Value
        })
    }
};

                // Always apply this only if campaignId not specified in filter
                if (string.IsNullOrEmpty(filter.campaignId?.ToString()))
                {
                    filterDoc.Add("campaignId", new BsonDocument("$ne", "1"));
                }
                else
                {
                    filterDoc.Add("campaignId", filter.campaignId.ToString());
                }

                // Optional phone filter
                if (!string.IsNullOrEmpty(filter.phone))
                {
                    filterDoc.Add("phoneNumber", filter.phone);
                }

                // Status filter
                switch (filter.statusId)
                {
                    case 1:
                        filterDoc.Add("is_sent", true);
                        break;
                    case 2:
                        filterDoc.Add("is_delivered", true);
                        break;
                    case 3:
                        filterDoc.Add("is_read", true);
                        break;
                    case 4:
                        filterDoc.Add("is_sent", false);
                        filterDoc.Add("is_delivered", false);
                        filterDoc.Add("is_read", false);
                        break;
                }

                // Execute paged query
                var pagedDocs = await collection.Find(filterDoc)
                    .Skip(filter.pageNumber)
                    .Limit(filter.pageSize)
                    .ToListAsync();

                foreach (var doc in pagedDocs)
                {
                    var campaign = GetCampaignFun(long.Parse(doc.campaignId)).FirstOrDefault();

                    result.contacts.Add(new ContactCampaignDto
                    {
                        Id = 0,
                        PhoneNumber = doc.phoneNumber,
                        TemplateName = campaign?.templateName ?? "not found",
                        CampaignName = campaign?.campaignName ?? "not found",
                        IsSent = doc.is_sent,
                        IsDelivered = doc.is_delivered,
                        IsRead = doc.is_read,
                        IsFailed = !doc.is_read && !doc.is_delivered && !doc.is_sent,
                        IsReplied = false,
                        IsHanged = false
                    });
                }

                result.TotalCount = (int)await collection.CountDocumentsAsync(filterDoc);
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }
        private static CampaginModel MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                CampaginModel model = new CampaginModel
                {
                    rowId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    campaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
                    templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
                    IsExternal = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                    CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId"),
                    JopName = SqlDataHelper.GetValue<string>(dataReader, "JopName"),
                    campaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                    templateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent"),

                };

                try
                {

                    model.model=System.Text.Json.JsonSerializer.Deserialize<MessageTemplateModel>(SqlDataHelper.GetValue<string>(dataReader, "TemplateJson"));
                }
                catch
                {


                }
                try
                {

                    model.templateVariablles=System.Text.Json.JsonSerializer.Deserialize<TemplateVariablles>(SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables"));
                }
                catch
                {
                    model.templateVariablles=new TemplateVariablles();

                }

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactToCampin>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new CampaginModel();
            }
        }

        private static List<CampaginModel> GetCampaignFun(long CampaignId)
        {
            try
            {
                var SP_Name = "GetSendCampaignNowById";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CampaignId", CampaignId)
                };
                List<CampaginModel> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, AppSettingsModel.ConnectionStrings).ToList();
                return model;
            }
            catch
            {
                return new List<CampaginModel>();
            }
        }
        private List<ContactsInterestedOfModel> getContactsInterested(int tenantId, int contactId)
        {
            try
            {
                List<ContactsInterestedOfModel> InterestedOf = new List<ContactsInterestedOfModel>();

                var SP_Name = Constants.Contacts.SP_ContactsInterestedOfGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId", tenantId),
                    new System.Data.SqlClient.SqlParameter("@ContactId", contactId)
                };
                InterestedOf = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapContactInterestedOf, AppSettingsModel.ConnectionStrings).ToList();
                return InterestedOf;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task<List<Order>> GetOrderList(int? TenantID)
        {

            //var x = GetContactId("962779746365", "28");


            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID;


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
                //var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                //if (!IsDeleted)
                //{
                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                order.Add(new Order
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    //OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                    //Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    //Address = dataSet.Tables[0].Rows[i]["Address"].ToString(),
                    //AgentId = long.Parse(dataSet.Tables[0].Rows[i]["AgentId"].ToString()),
                    //AreaId = long.Parse(dataSet.Tables[0].Rows[i]["AreaId"].ToString()),
                    //BranchId = long.Parse(dataSet.Tables[0].Rows[i]["BranchId"].ToString()),
                    ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                    //CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                    //OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),

                    //IsLockByAgent = bool.Parse(dataSet.Tables[0].Rows[i]["IsLockByAgent"].ToString()),
                    //LockByAgentName = dataSet.Tables[0].Rows[i]["LockByAgentName"].ToString(),
                    //orderStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), dataSet.Tables[0].Rows[i]["orderStatus"].ToString(), true),
                    // OrderType = (OrderTypeEunm)Enum.Parse(typeof(OrderTypeEunm), dataSet.Tables[0].Rows[i]["OrderType"].ToString(), true),
                    //TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                    //            OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                    //OrderRemarks = dataSet.Tables[0].Rows[i]["OrderRemarks"].ToString()

                });

                //}

            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private List<OrderDetailDto> GetOrderDetail2(int? TenantID, long? OrderId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[OrderDetails] where TenantID=" + TenantID + " and OrderId=" + OrderId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<OrderDetailDto> orderDetailDtos = new List<OrderDetailDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new OrderDetailDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    Discount = decimal.Parse(dataSet.Tables[0].Rows[i]["Discount"].ToString()),
                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    OrderId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    //TotalAfterDiscunt = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalAfterDiscunt"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private List<ExtraOrderDetailsDto> GetOrderDetailExtra(int? TenantID, long? OrderDetailId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ExtraOrderDetail] where TenantID=" + TenantID + " and OrderDetailId=" + OrderDetailId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ExtraOrderDetailsDto> orderDetailDtos = new List<ExtraOrderDetailsDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new ExtraOrderDetailsDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    OrderDetailId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderDetailId"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private void DeleteOrder(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM Orders   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private void DeleteOrderDetails(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM OrderDetails   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private void DeleteExtraOrderDetail(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM ExtraOrderDetail   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private ContactDto getContactbyId(int id)
        {
            try
            {
                ContactDto contactDto = new ContactDto();
                var SP_Name = Constants.Contacts.SP_ContactbyIdGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@Id",id)
                };

                contactDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapContact, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return contactDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void updateContact(ContactDto contactDto)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@Id",contactDto.Id)
                   ,new System.Data.SqlClient.SqlParameter("@ContactDisplayName",contactDto.ContactDisplayName)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void updateContactInfo(ContactDto contactDto)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactInfoUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto))

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int createContact(ContactDto contactDto)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto))
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@ContactId";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (int)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void DeleteOrder(int? TenantID, int ContactId)
        {

            var orderDrift = GetOrderList(TenantID).Result;
            var orderDrift2 = orderDrift.Where(x => x.ContactId == ContactId).ToList();
            foreach (var order in orderDrift2)
            {
                var orderDetailsDrft = GetOrderDetail2(TenantID, order.Id);

                foreach (var orderDetai in orderDetailsDrft)
                {
                    var GetOrderDetailExtraDraft = GetOrderDetailExtra(TenantID, orderDetai.Id);

                    foreach (var ExtraOrde in GetOrderDetailExtraDraft)
                    {

                        DeleteExtraOrderDetail(ExtraOrde.Id);
                    }
                    DeleteOrderDetails(orderDetai.Id);
                }
                DeleteOrder(order.Id);
            }
        }
        private void deleteContact(int id)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactDelete;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Id",id)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int updateContactFailed(int id, string phoneNumber)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactUpdatePhoneNumber;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",AbpSession.TenantId.Value) ,
                    new System.Data.SqlClient.SqlParameter("@Id",id) ,
                    new System.Data.SqlClient.SqlParameter("@phoneNumber",phoneNumber)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int deleteContactFailed(int id)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_DeleteContactFailed;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Id",id)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int deleteContactFromGroup(int id)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_DeleteContactFromGroup;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Id",id)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<Dictionary<string, dynamic>> contactDelete(EntityDto input)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();

                var contact = await _contactRepository.FirstOrDefaultAsync((int)input.Id);
                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contact.UserId);
                var customer = new CustomerModel();

                try { customer = customerResult.Result; }
                catch { customer = null; }

                if (customer != null)
                {
                    if (customer.IsDeleted)
                    {
                        response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "This number has already been deleted before" } };
                    }
                    else
                    {
                        customer.IsDeleted = true;
                        customer.GroupId = 0;
                        customer.GroupName = "";

                        var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
                        if (Result.ID != null)
                        {
                            //delete contact from DB
                            int contactFromDB = deleteContactFailed(input.Id);
                            if (contactFromDB != 0)
                            {
                                //delete from group
                                int idFromGroup = deleteContactFromGroup(input.Id);
                                if (idFromGroup != 0)
                                {
                                    return new Dictionary<string, dynamic> { { "state", 2 }, { "message", contactFromDB } };
                                }
                            }
                        }
                    }
                    response = new Dictionary<string, dynamic> { { "state", -1 }, { "message", "Error while Deleting" } };
                }
                else
                {
                    response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "This number has already been deleted before" } };
                }
                return response;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        private async Task<Dictionary<string, dynamic>> contactUpdate(int contactId, string phoneNumber)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();

                phoneNumber = phoneNumber.Trim();

                if ((phoneNumber.Length >= 11 && phoneNumber.Length <= 15) &&
                         (long.TryParse(phoneNumber, out _) && long.Parse(phoneNumber) >= 0))
                {
                    var contact = await _contactRepository.FirstOrDefaultAsync(contactId);
                    string UserId = AbpSession.TenantId.Value + "_" + phoneNumber;
                    var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == UserId);
                    var customer = new CustomerModel();

                    try { customer = customerResult.Result; }
                    catch { customer = null; }

                    if (customer == null)
                    {
                        var customerResults = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == contact.UserId);
                        var customers = new CustomerModel();
                        try { customers = customerResults.Result; }
                        catch { customers = null; }
                        if (customers != null)
                        {
                            customers.phoneNumber = phoneNumber;
                            customers.userId = UserId;
                            var Result = itemsCollection.UpdateItemAsync(customers._self, customers).Result;
                            if (Result.ID != null)
                            {
                                //update contact from DB
                                int contactFromDB = updateContactFailed(contactId, phoneNumber);
                                if (contactFromDB > 0)
                                    return new Dictionary<string, dynamic> { { "state", 2 }, { "message", contactFromDB } };
                            }
                        }
                        response = new Dictionary<string, dynamic> { { "state", -1 }, { "message", "Error while Update" } };
                    }
                    else
                    {
                        if (customer.IsDeleted)
                        {
                            response = new Dictionary<string, dynamic> { { "state", 4 }, { "message", "This number has already been deleted" } };
                        }
                        else
                        {
                            response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The number already exists" } };
                        }
                    }
                }
                else
                {
                    response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "Invalid phone number format" } };
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        private bool checkIfExistContactByPhoneNumber(string phoneNumber)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactCheckIfExistGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@PhoneNumber",phoneNumber),
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                bool result = (bool)OutputParameter.Value;
                return result;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}