using CampaignSendNew.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using System.Text.RegularExpressions;

namespace CampaignSendNew
{
    public static class CreateGroups
    {

        #region Functions
        [FunctionName("CreateGroups1")]
        public static async Task Group1Async([QueueTrigger("creategroups1", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);

            if(group == null)
            {
                return;
            }
            var funName = "creategroups1"+obj.rowId.ToString();


            try
            {

                await sendGroup(log, obj, group, "creategroups1");

            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }

        }
        [FunctionName("CreateGroups2")]
        public static async Task Group2Async([QueueTrigger("creategroups2", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }

            var funName = "creategroups2"+obj.rowId.ToString();

            try
            {


                await sendGroup(log, obj, group, "creategroups2");



            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }



        }
        [FunctionName("CreateGroups3")]
        public static async Task Group3Async([QueueTrigger("creategroups3", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }



            var funName = "creategroups3"+obj.rowId.ToString();


            try
            {


                await sendGroup(log, obj, group, "creategroups3");


            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }


        }
        [FunctionName("CreateGroups4")]
        public static async Task Group4Async([QueueTrigger("creategroups4", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }


            var funName = "creategroups4"+obj.rowId.ToString();


            try
            {
                await sendGroup(log, obj, group, "creategroups4");


            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }


        }
        [FunctionName("CreateGroups5")]
        public static async Task Group5Async([QueueTrigger("creategroups5", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }



            var funName = "creategroups5"+obj.rowId.ToString();


            try
            {


                await sendGroup(log, obj, group, "creategroups5");


            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }


        }
        [FunctionName("CreateGroups6")]
        public static async Task Group6Async([QueueTrigger("creategroups6", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }



            var funName = "creategroups6"+obj.rowId.ToString();

            try
            {


                await sendGroup(log, obj, group, "creategroups6");



            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }


        }
        [FunctionName("CreateGroups7")]
        public static async Task Group7Async([QueueTrigger("creategroups7", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }



            var funName = "creategroups7"+obj.rowId.ToString();



            try
            {

                await sendGroup(log, obj, group, "creategroups7");


            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }


        }
        [FunctionName("CreateGroups8")]
        public static async Task Group8Async([QueueTrigger("creategroups8", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }

            try
            {
                await sendGroup(log, obj, group, "creategroups8");

            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }

        }
        [FunctionName("CreateGroups9")]
        public static async Task Group9Async([QueueTrigger("creategroups9", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }



            var funName = "creategroups9"+obj.rowId.ToString();


            try
            {

                await sendGroup(log, obj, group, "creategroups9");

            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }


        }
        [FunctionName("CreateGroups10")]
        public static async Task Group10Async([QueueTrigger("creategroups10", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            GroupSetQueueModel obj = JsonConvert.DeserializeObject<GroupSetQueueModel>(message);
            GroupQueueModel group = GetGroup(obj.rowId);
            if (group == null)
            {
                return;
            }



            var funName = "creategroups10"+obj.rowId.ToString();

            try
            {
                await sendGroup(log, obj, group, "creategroups10");

            }
            catch (Exception ex)
            {

                log.LogInformation($"{ex}");
            }


        }
        #endregion

        private static async Task sendGroup(ILogger log, GroupSetQueueModel obj, GroupQueueModel group,string jopName)
        {
            if (group.membersDto.Any())
            {
                try
                {
                    GroupCreateDto groupModel = new GroupCreateDto();

                    if (group.IsExternalContact)
                    {
                        groupModel = await AddNewMembersOnGroupAsync(group, obj.groupName, jopName);

                    }
                    else
                    {
                        groupModel = await AddNewMembersOnGroupAsync(group, obj.groupName, jopName);
                        // groupModel = await AddOldMembersOnGroup(group, obj.groupName);

                    }



                    if (groupModel.NUmberList != null)
                    {
                        //foreach (var member in groupModel.NUmberList)
                        //{
                        //    var itemsCollection3 = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
                        //    var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == ContainerItemTypes.CustomerItem && a.userId == obj.tenantId+"_"+ member.NUmber && a.TenantId == obj.tenantId);
                        //    var Customer3 = customerResult3.Result;

                        //    if (Customer3 != null)
                        //    {
                        //        Customer3.GroupId = group.groupId;
                        //        Customer3.GroupName = obj.groupName;
                        //        var Result = itemsCollection3.UpdateItemAsync(Customer3._self, Customer3).Result;
                        //    }
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation($"{ex}");
                }
            }
        }
        private static async Task<GroupCreateDto> AddNewMembersOnGroupAsync(GroupQueueModel group,string groupName, string jopName)
        {
            try
            {
                GroupCreateDto model = new GroupCreateDto();
                model.failedAdd = new List<MembersDto>(); // Initialize failedAdd as a new list
                model.NUmberList = new List<phoneNumbers>();
                if (group.membersDto.Count > 0)
                {
                    List<MembersDto> newMembersList = new List<MembersDto>();
                    TenantModel tenantModel = new TenantModel();

                    var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
                    tenantModel = await itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.Tenant && a.TenantId == group.tenantId);

                    if (tenantModel != null)
                    {
                        List<AddContactBulkDto> contactList = new List<AddContactBulkDto>();
                        List<CustomerModel> customerModels = new List<CustomerModel>();
                        foreach (MembersDto member in group.membersDto)
                        {

                            if (member.isFailed == false)
                            {

                                var cont = new AddContactBulkDto()
                                {
                                    UserId = group.tenantId + "_" + member.phoneNumber,
                                    DisplayName = member.displayName,

                                    PhoneNumber = member.phoneNumber,

                                    TenantId = group.tenantId,

                                    Group=group.groupId.ToString(),
                                    GroupName=groupName,
                                    variables = member.variables,
                                    customeropt = member.customeropt
                                };

                                contactList.Add(cont);

                                var CustomerModel = new CustomerModel()
                                {
                                    TennantPhoneNumberId = tenantModel.D360Key,
                                    ConversationsCount = 0,
                                    ContactID = null,
                                    IsComplaint = false,
                                    userId = group.tenantId + "_" + member.phoneNumber,
                                    displayName = member.displayName,
                                    avatarUrl = "",
                                    type = "text",
                                    D360Key = tenantModel.D360Key,
                                    CreateDate = DateTime.Now,
                                    IsLockedByAgent = false,
                                    LockedByAgentName = tenantModel.botId,
                                    IsOpen = false,
                                    agentId = 100000,
                                    IsBlock = false,
                                    IsConversationExpired = false,
                                    CustomerChatStatusID = (int)CustomerChatStatus.Active,
                                    CustomerStatusID = (int)CustomerStatus.Active,
                                    LastMessageData = DateTime.Now,
                                    IsNew = true,
                                    TenantId = group.tenantId,
                                    phoneNumber = member.phoneNumber,
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
                                    creation_timestamp = 0,
                                    expiration_timestamp = 0,
                                    CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false },
                                    GroupId=group.groupId,
                                    GroupName=groupName
                                };

                                customerModels.Add(CustomerModel);



                                newMembersList.Add(member);
                            }
                            else
                            {
                                model.failedAdd.Add(member);
                                model.failedCount++;
                                continue;
                            }


                        }

                        var x= createContactBulkAsync(contactList, group.tenantId, group.groupId, groupName, group.rowId).Result;


                        if (x==1)
                        {

                            AddFiledContactBulk(group.tenantId, group.groupId, groupName, group.rowId);


                            GroupQueueModel groupList = GetGroupList(group.groupId, group.tenantId);

                            if (groupList!=null)
                            {

                                GroupSetQueueModel groupSetQueueModelS = new GroupSetQueueModel();

                                var JopName = RemoveNumbers(jopName);

                                if (JopName=="creategroups11")
                                {



                                }
                                else
                                {

                                    groupSetQueueModelS.rowId = groupList.rowId;
                                    groupSetQueueModelS.tenantId = group.tenantId;
                                    groupSetQueueModelS.groupId = group.groupId;
                                    groupSetQueueModelS.functionName = JopName;
                                    groupSetQueueModelS.groupName = groupName;
                                    SetCampinQueueContact(groupSetQueueModelS);
                                }


                            }
                        }




                        //if (x==0)
                        //{
                        //    var SP_Name = Constants.Group.AddMembersOnGroupFromJop;
                        //    //string str = JsonConvert.SerializeObject(newMembersList);
                        //    List<List<MembersDto>> splitLists = await SplitList(newMembersList, 5);

                        //    List<string> MembersAdds = new List<string>();

                        //    foreach (var member in splitLists)
                        //    {
                        //        if (member.Count == 0)
                        //        {
                        //            break;
                        //        }
                        //        // Serialize the current member object and add it to the list
                        //        MembersAdds.Add(JsonConvert.SerializeObject(member));
                        //    }

                        //    string failedAdd = JsonConvert.SerializeObject(model.failedAdd);

                        //    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                        //    new System.Data.SqlClient.SqlParameter("@tenantId", group.tenantId),
                        //    new System.Data.SqlClient.SqlParameter("@groupId", group.groupId),
                        //     new System.Data.SqlClient.SqlParameter("@groupName", groupName),
                        //    new System.Data.SqlClient.SqlParameter("@failedAdd", failedAdd),
                        //    new System.Data.SqlClient.SqlParameter("@rowId", group.rowId),
                        //    new System.Data.SqlClient.SqlParameter("@failedCounEX", model.failedCount),
                        //};
                        //    // Set serialized member lists parameters
                        //    for (int i = 0; i < MembersAdds.Count && i < 5; i++)
                        //    {
                        //        string paramName = $"@membersJson{i + 1}";
                        //        string json = MembersAdds[i] ?? ""; // If null, set to empty string

                        //        sqlParameters.Add(new System.Data.SqlClient.SqlParameter(paramName, json));
                        //    }

                        //    model.NUmberList = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapMembersUpdateNew, Constants.ConnectionString).ToList();

                        //}

                        return model;
                    }
                }
                model.id = 0;
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static async Task<List<List<T>>> SplitList<T>(List<T> list, int numberOfLists)
        {
            int chunkSize = (int)Math.Ceiling((double)list.Count / numberOfLists);

            return Enumerable.Range(0, numberOfLists)
                .Select(i => list.Skip(i * chunkSize).Take(chunkSize).ToList())
                .ToList();
        }
        private static async Task<GroupCreateDto> AddOldMembersOnGroup(GroupQueueModel group, string groupName)
        {
            try
            {
                GroupCreateDto model = new GroupCreateDto();
                model.failedAdd = new List<MembersDto>();
                model.NUmberList = new List<phoneNumbers>();

                if (group.membersDto.Count > 0)
                {
                    var SP_Name = Constants.Group.AddMembersOnGroupFromJop;

                    List<List<MembersDto>> splitLists = await SplitList(group.membersDto, 5);

                    List<string> MembersAdds = new List<string>();

                    foreach (var member in splitLists)
                    {
                        if (member.Count == 0)
                        {
                            break;
                        }
                        // Serialize the current member object and add it to the list
                        MembersAdds.Add(JsonConvert.SerializeObject(member));
                    }

                    string failedAdd = "";
                    int failedCount = 0;
                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                            new System.Data.SqlClient.SqlParameter("@tenantId", group.tenantId),
                            new System.Data.SqlClient.SqlParameter("@groupId", group.groupId),
                            new System.Data.SqlClient.SqlParameter("@groupName", groupName),
                            new System.Data.SqlClient.SqlParameter("@failedAdd", failedAdd),
                            new System.Data.SqlClient.SqlParameter("@rowId", group.rowId),
                            new System.Data.SqlClient.SqlParameter("@failedCounEX", failedCount),
                        };
                    // Set serialized member lists parameters
                    for (int i = 0; i < MembersAdds.Count && i < 5; i++)
                    {
                        string paramName = $"@membersJson{i + 1}";
                        string json = MembersAdds[i] ?? ""; // If null, set to empty string

                        sqlParameters.Add(new System.Data.SqlClient.SqlParameter(paramName, json));
                    }

                    model.NUmberList = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapMembersUpdateNew, Constants.ConnectionString).ToList();

                    return model;

                    //var SP_Name = Constants.Group.AddMembersOnGroupFromJop;
                    //string str = JsonConvert.SerializeObject(group.membersDto);
                    //string failedAdd = "";
                    //int failedCount = 0;
                    //var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    //    new System.Data.SqlClient.SqlParameter("@tenantId",group.tenantId) ,
                    //    new System.Data.SqlClient.SqlParameter("@groupId",group.groupId) ,
                    //    new System.Data.SqlClient.SqlParameter("@membersJson",str),
                    //    new System.Data.SqlClient.SqlParameter("@failedAdd",failedAdd),
                    //    new System.Data.SqlClient.SqlParameter("@rowId",group.rowId),
                    //    new System.Data.SqlClient.SqlParameter("@failedCounEX",failedCount),
                    //};

                    //model.NUmberList = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapMembersUpdateNew, Constants.ConnectionString).ToList();

                    //return model;
                }
                model.id = 0;






                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static GroupQueueModel GetGroup(long rowId)
        {
            try
            {
                var SP_Name = Constants.Group.GroupGetQueDB;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                };
                var model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapGroup, Constants.ConnectionString).FirstOrDefault();
                return model;
            }
            catch
            {
                return new GroupQueueModel();
            }
        }
        private static GroupQueueModel GetGroupList(long GroupId, int TenantId)
        {
            try
            {
                var SP_Name = Constants.Group.GroupGetListQueDB;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@GroupId", GroupId)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId", TenantId)
                };
                var model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapGroup, Constants.ConnectionString).FirstOrDefault();
                return model;
            }
            catch
            {
                return new GroupQueueModel();
            }
        }
        private static CustomerModel CreateNewCustomer(string from, string name, string type, string botID, int TenantId, string D360Key)
        {
            // string userId = TenantId + "_" + from;
            //string displayName = name;
            //  string phoneNumber = from;

            var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);

            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = TenantId + "_" + from,
                DisplayName = name,
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
                PhoneNumber = from,
                SunshineAppID = "",// model.app._id,
                                   //ConversationId = model.statuses.FirstOrDefault().conversation.id,
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1,
                // ConversationId = model.statuses.FirstOrDefault().conversation.id
            };

            //var idcont = InsertContact(cont);
            var idcont = createContact(cont);

            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId = D360Key,
                ConversationsCount = 0,
                ContactID = idcont.ToString(),
                IsComplaint = false,
                userId = TenantId + "_" + from,
                displayName = name,
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
                phoneNumber = from,
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
                creation_timestamp = 0,
                expiration_timestamp = 0,
                CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false }
            };

            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;

            var itemsCollection2 = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);

            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == ContainerItemTypes.CustomerItem && a.userId ==  TenantId + "_" + from && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;
            return CustomerModel;
        }
        private static int createContact(ContactDto contactDto)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto)),

                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@ContactId";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

                return (int)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private static async Task<int> createContactBulkAsync(List<AddContactBulkDto> contactDto, int tenantId,long groupId, string groupName , long rowId)
        {
            try
            {


                if (contactDto.Count()>1000)
                {
                    int chunkSize = 5; // Define the size of each chunk
                    var chunks = SplitList(contactDto, chunkSize);

                    foreach (var chunk in chunks.Result)
                    {
                        await Task.Delay(2000);
                        string jsonChunk = JsonConvert.SerializeObject(chunk);

                        var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                           {
                               new System.Data.SqlClient.SqlParameter("@ContactInfoJson", SqlDbType.NVarChar, -1)
                               {
                                   Value = jsonChunk
                               },
                               new System.Data.SqlClient.SqlParameter("@TenantId", SqlDbType.Int)
                               {
                                   Value = tenantId
                               }
                               ,
                                    new System.Data.SqlClient.SqlParameter("@groupId", groupId),
                                   new System.Data.SqlClient.SqlParameter("@groupName", groupName),
                                  new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                           };

                        SqlDataHelper.ExecuteNoneQuery(Constants.Contacts.SP_ContactsAddBulk, sqlParameters.ToArray(), Constants.ConnectionString);
                    }




                }
                else
                {


                    string jsonChunk = JsonConvert.SerializeObject(contactDto);

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                           {
                               new System.Data.SqlClient.SqlParameter("@ContactInfoJson", SqlDbType.NVarChar, -1)
                               {
                                   Value = jsonChunk
                               },
                               new System.Data.SqlClient.SqlParameter("@TenantId", SqlDbType.Int)
                               {
                                   Value = tenantId
                               }
                               ,
                                    new System.Data.SqlClient.SqlParameter("@groupId", groupId),
                                   new System.Data.SqlClient.SqlParameter("@groupName", groupName),
                                  new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                           };

                    SqlDataHelper.ExecuteNoneQuery(Constants.Contacts.SP_ContactsAddBulk, sqlParameters.ToArray(), Constants.ConnectionString);
                }


                return 1;
            }
            catch (Exception ex)
            {
                // Consider logging the exception instead of rethrowing
                return -1;
            }
        }


        private static int AddFiledContactBulk( int tenantId, long groupId, string groupName, long rowId)
        {
            try
            {


                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                     {

                          new System.Data.SqlClient.SqlParameter("@TenantId", tenantId),
                              new System.Data.SqlClient.SqlParameter("@groupId", groupId),
                             new System.Data.SqlClient.SqlParameter("@groupName", groupName),
                            new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                     };

                SqlDataHelper.ExecuteNoneQuery(Constants.Contacts.SP_AddFiledContactBulk, sqlParameters.ToArray(), Constants.ConnectionString);



                return 0;
            }
            catch (Exception ex)
            {
                // Consider logging the exception instead of rethrowing
                return 0;
            }
        }

        private static void createContactCosmoDBBulk(List<CustomerModel> contactDto)
        {
            try
            {
                var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);


                // Insert bulk items
                var result = itemsCollection.CreateItemsAsync(contactDto).Result;

                //return (int)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        static string RemoveNumbers(string input)
        {
            return Regex.Replace(input, @"\d+", m => (int.Parse(m.Value) + 1).ToString());
        }
        private static void SetCampinQueueContact(GroupSetQueueModel campinQueueNew)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constant.AzureStorageAccount);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference(campinQueueNew.functionName);
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           campinQueueNew
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception)
            {
                var Error = JsonConvert.SerializeObject(campinQueueNew);
            }
        }
        #region Mapper
        public static GroupQueueModel MapGroup(IDataReader dataReader)
        {
            try
            {
                GroupQueueModel model = new GroupQueueModel
                {
                    rowId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    tenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    groupId = SqlDataHelper.GetValue<long>(dataReader, "GroupId"),
                    //createdDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                    IsExternalContact = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                    IsCreated = SqlDataHelper.GetValue<bool>(dataReader, "IsCreated")
                };

                // Deserialize ContactsJson to List<MembersDto>
                model.membersDto = System.Text.Json.JsonSerializer.Deserialize<List<MembersDto>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new GroupQueueModel();
            }
        }
        public static MembersDto MapMembersUpdate(IDataReader dataReader)
        {
            MembersDto groupDtoModel = new MembersDto();

            groupDtoModel.id = SqlDataHelper.GetValue<int>(dataReader, "id");
            groupDtoModel.phoneNumber = SqlDataHelper.GetValue<string>(dataReader, "phoneNumber");
            groupDtoModel.displayName = SqlDataHelper.GetValue<string>(dataReader, "displayName");
            groupDtoModel.failedId = SqlDataHelper.GetValue<int>(dataReader, "failedId");
            //groupDtoModel.isChecked = false;

            return groupDtoModel;
        }
        public static phoneNumbers MapMembersUpdateNew(IDataReader dataReader)
        {
            phoneNumbers groupDtoModel = new phoneNumbers();

            groupDtoModel.NUmber = SqlDataHelper.GetValue<string>(dataReader, "phoneNumber");

            return groupDtoModel;
        }
        #endregion
    }
}
