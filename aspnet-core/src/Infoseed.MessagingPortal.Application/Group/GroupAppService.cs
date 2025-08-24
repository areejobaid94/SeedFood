using Abp.Application.Services.Dto;
using Framework.Data;
using Infoseed.MessagingPortal.Group.Dto;
using Infoseed.MessagingPortal.Groups.Dto;
using Infoseed.MessagingPortal.InfoSeedParser;
using InfoSeedParser.ConfigrationFile;
using InfoSeedParser.Parsers;
using InfoSeedParser;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InfoSeedParser.Interfaces;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Microsoft.Azure.Documents;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.General;
using Newtonsoft.Json;
using NUglify.Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Abp.Dependency;
using Microsoft.AspNetCore.Identity;
using Abp.Runtime.Session;
using Infoseed.MessagingPortal.Authorization.Roles;
using System.Diagnostics;

namespace Infoseed.MessagingPortal.Group
{
    public class GroupAppService : MessagingPortalAppServiceBase, IGroupAppService
    {
        private readonly IMembersGroupParser _MembersParser;
        private readonly IDocumentClient _IDocumentClient;
        private readonly IGeneralAppService _IGeneralAppService;

        public GroupAppService(IDocumentClient iDocumentClient, IGeneralAppService IGeneralAppService)
        {
            _MembersParser = new ParserFactory().CreateParserMembers(nameof(MembersGroupParser));
            _IDocumentClient = iDocumentClient;
            _IGeneralAppService = IGeneralAppService;
        }

        #region CRUD Operation for Groups (Public)

        [HttpGet]
        public GroupModel GroupGetAll(string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            GroupModel groupModel = new GroupModel();
            try
            {
                groupModel = groupGetAll(searchTerm, pageNumber, pageSize);
                return groupModel;
            }
            catch (Exception ex)
            {
                groupModel.state = -1;
                groupModel.message = ex.Message;
                return groupModel;
            }
        }
        [HttpGet]
        public GroupMembersDto GroupGetById(long groupId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            GroupMembersDto groupMembersDto = new GroupMembersDto();
            try
            {
                groupMembersDto.groupDtoModel = groupGetById(groupId);
                groupMembersDto.membersDto = MembersGet(groupId, searchTerm, pageNumber, pageSize);
                groupMembersDto.totalCount = groupMembersDto.membersDto.Count;
                return groupMembersDto;
            }
            catch
            {
                // Return a default value or throw an exception here
                return new GroupMembersDto();
            }
        }
        internal List<MembersDto> MembersGet(long groupId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            try
            {
                return membersGet(groupId, searchTerm, pageNumber, pageSize);
            }
            catch
            {
                return new List<MembersDto>();
            }
        }
        [HttpGet]
        public PagedResultDto<MembersDto> MembersFilter(MembersFilter input)
        {
            try
            {
                return membersFilter(input);
            }
            catch
            {
                return new PagedResultDto<MembersDto>(0, new List<MembersDto>());
            }
        }
        [HttpPost]
        public async Task<GroupCreateDto> GroupCreateMembers(GroupMembersDto input, bool isExternal, bool markContactsAsUnsubscribed = false)
        {
            GroupCreateDto groupModel = new GroupCreateDto();
            try
            {
                if (input.groupDtoModel.groupName.Length <= 20)
                {
                    var model = GroupGetAll("", 0, int.MaxValue)
                                    .groupDtoModel
                                    .Where(x => x.groupName.ToLower().Trim() == input.groupDtoModel.groupName.ToLower().Trim())
                                    .FirstOrDefault();
                    input.groupDtoModel.TotolForPrograss = input.membersDto.Count();
                    if (model == null)
                    {
                        groupModel = await createMembers(input, isExternal, markContactsAsUnsubscribed);
                        if (groupModel != null)
                        {
                            groupModel.state = 2;
                            groupModel.message = "Ok";
                        }
                        else
                        {
                            groupModel.state = 3;
                            groupModel.message = "Tenant missing";
                        }
                    }
                    else
                    {
                        groupModel.state = 1;
                        groupModel.message = "The group name is used";
                    }
                }
                else
                {
                    groupModel.state = 4;
                    groupModel.message = "The group name is more than 20 characters long";
                }
                return groupModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public Dictionary<string, dynamic> ValidGroupName(string groupName)
        {
            try
            {
                return validGroupName(groupName);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        [HttpPost]
        public async Task<ReadFromExcelDto> ReadFromExcel([FromForm] UploadFileModel file)
        {
            ReadFromExcelDto result = new ReadFromExcelDto();
            try
            {
                if (file == null || file.FormFile.Length == 0)
                {
                    return result;
                }


                var formFile = file.FormFile;

                byte[] fileData = null;
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    fileData = ms.ToArray();
                }

                var MembersList = _MembersParser.ParseMembers(new ParseConfig()
                {
                    MembersConfig = new MembersConfigrationExcelFile(),
                    FileData = fileData,
                    FileName = formFile.FileName,
                    Parser = nameof(MembersGroupParser)
                });

                result.DuplicateCount = MembersList.duplicateCount;

                result.List = MembersList.members.OrderBy(member => !member.isFailed).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPut]
        public async Task<GroupCreateDto> GroupUpdate(GroupMembersDto input, bool isExternal, int statusId = 0)
        {
            GroupCreateDto groupModel = new GroupCreateDto();
            try
            {
                if (statusId != 0 && statusId >= 1 && statusId <= 3)
                {
                    if (AbpSession.TenantId.HasValue)
                    {
                        input.groupDtoModel.TotolForPrograss = input.membersDto.Count();

                        long outId = await groupUpdate(input);
                        if (outId != 0)
                        {
                            switch (statusId)
                            {
                                case 1:
                                case 2:
                                    groupModel = await updateMembers(input, isExternal);
                                    break;
                                case 3:
                                    groupModel = RemoveMembers(input);
                                    break;
                            }

                            groupModel.id = input.groupDtoModel.id;
                            groupModel.groupName = input.groupDtoModel.groupName;

                            groupModel.state = 2;
                            groupModel.message = "Ok";

                            return groupModel;
                        }
                    }
                    //groupModel = updateOldMembers(input);
                    //break;
                }
                groupModel.id = 0;
                return groupModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal GroupCreateDto RemoveMembers(GroupMembersDto input)
        {
            GroupCreateDto groupModel = new GroupCreateDto();
            groupModel.failedAdd = new List<MembersDto>(); // Initialize failedAdd as a new list
            try
            {
                if (AbpSession.TenantId.HasValue && input.membersDto.Count > 0)
                {
                    groupModel = removeMembers(input);

                    groupModel.id = input.groupDtoModel.id;
                    groupModel.groupName = input.groupDtoModel.groupName;

                    return groupModel;
                }
                else
                {
                    groupModel.id = 0;
                    return groupModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpDelete]
        public GroupModel GroupDelete(long groupId)
        {
            GroupModel groupModel = new GroupModel();
            try
            {
                groupModel = groupDelete(groupId);
                return groupModel;
            }
            catch (Exception ex)
            {
                groupModel.state = -1;
                groupModel.message = ex.Message;
                return groupModel;
            }
        }
        [HttpPost]
        public GroupCreateDto MovingGroup(MoveMembersDto input)
        {
            GroupCreateDto model = new GroupCreateDto();
            try
            {
                model = movingGroup(input);
                return model;
            }
            catch (Exception ex)
            {
                model.state = -1;
                model.message = ex.Message;
                return model;
            }
        }
        [HttpGet]
        public async Task<GroupLog> GroupLogGetAll(long groupId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            GroupLog groupModel = new GroupLog();
            try
            {
                groupModel = await groupLogGetAll(groupId, searchTerm, pageNumber, pageSize);
                return groupModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region CRUD Operation for Groups (private)

        private GroupModel groupGetAll(string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            GroupModel groupModel = new GroupModel();
            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "")
                    searchTerm = searchTerm.ToLower();
                var SP_Name = Constants.Groups.SP_GroupsGetAllPerUser;
                //var SP_Name = Constants.Groups.SP_GroupsGetAllNew;
                int tenantId = AbpSession.TenantId.Value;
                long userId = AbpSession.UserId.Value;
                bool isAdmin = IsUserAdmin();

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize) ,
                    new System.Data.SqlClient.SqlParameter("@userId", isAdmin ? (object)DBNull.Value : userId),
                    new System.Data.SqlClient.SqlParameter("@isAdmin", isAdmin),
                    new System.Data.SqlClient.SqlParameter("@searchTerm",searchTerm)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                groupModel.groupDtoModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGroups, AppSettingsModel.ConnectionStrings).ToList();


                groupModel.state = 2;
                groupModel.message = "OK";
                groupModel.total = OutputParameter.Value != DBNull.Value ? Convert.ToInt64(OutputParameter.Value) : 0;
                if (groupModel.total == 0)
                {
                    groupModel.state = 2;
                    groupModel.message = "No results found";
                }
                return groupModel;
            }
            catch (Exception ex)
            {
                groupModel.state = -1;
                groupModel.message = ex.Message;
                return groupModel;
            }
        }

        private GroupDtoModel groupGetById(long id, int? tenantId = null)
        {
            GroupDtoModel groupModel = new GroupDtoModel();
            try
            {
                var SP_Name = Constants.Groups.SP_GroupsGetByIdNew;
                tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@groupId",id) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                groupModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGroups, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return groupModel;
            }
            catch
            {
                return groupModel;
            }
        }
        private List<MembersDto> membersGet(long groupId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            try
            {
                var SP_Name = Constants.Groups.SP_MembersGetAll;
                int tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@groupId",groupId) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@searchTerm",searchTerm) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                List<MembersDto> itemes = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMembers, AppSettingsModel.ConnectionStrings).ToList();

                return itemes;
            }
            catch
            {
                return new List<MembersDto>();
            }
        }
        private PagedResultDto<MembersDto> membersFilter(MembersFilter input)
        {
            try
            {
                var SP_Name = Constants.Groups.SP_ContactFilter;
                int tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber",input.pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",input.pageSize) ,
                    new System.Data.SqlClient.SqlParameter("@contactName",input.contactName) ,
                    new System.Data.SqlClient.SqlParameter("@countryCode",input.countryCode) ,
                    new System.Data.SqlClient.SqlParameter("@joiningFrom",input.joiningFrom) ,
                    new System.Data.SqlClient.SqlParameter("@joiningTo",input.joiningTo) ,
                    new System.Data.SqlClient.SqlParameter("@isOpt",input.isOpt),
                    new System.Data.SqlClient.SqlParameter("@groupid",input.groupId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                List<MembersDto> itemes = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMembers, AppSettingsModel.ConnectionStrings).ToList();
                itemes = itemes.DistinctBy(x => x.phoneNumber).ToList();
                return new PagedResultDto<MembersDto>(OutputParameter.Value != DBNull.Value ? Convert.ToInt32(OutputParameter.Value) : 0, itemes);
            }
            catch
            {
                return new PagedResultDto<MembersDto>(0, new List<MembersDto>());
            }
        }
        private async Task<GroupCreateDto> createMembers(GroupMembersDto input, bool isExternal, bool markContactsAsUnsubscribed = false)
        {
            GroupCreateDto groupModel = new GroupCreateDto();
            try
            {
                if (AbpSession.TenantId.HasValue)
                {
                    var SP_Name = Constants.Groups.SP_GroupsCreateNew;
                    int tenantId = AbpSession.TenantId.Value;
                    long userId = AbpSession.UserId.Value;
                    var user = await UserManager.GetUserByIdAsync(userId);
                    string fullName = user.Name + " " + user.Surname;

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                        new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                        new System.Data.SqlClient.SqlParameter("@groupName",input.groupDtoModel.groupName) ,
                        new System.Data.SqlClient.SqlParameter("@creationDate",DateTime.UtcNow.AddHours(3)),
                        new System.Data.SqlClient.SqlParameter("@userId",userId),
                        new System.Data.SqlClient.SqlParameter("@fullName",fullName)

                    };
                    var OutputParameter = new System.Data.SqlClient.SqlParameter
                    {
                        SqlDbType = SqlDbType.BigInt,
                        ParameterName = "@outPut",
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters.Add(OutputParameter);
                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                    long outId = (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
                    if (outId == 0)
                    {
                        return groupModel;
                    }
                    else
                    {
                        if (input.membersDto.Count > 0)
                        {
                            if (markContactsAsUnsubscribed)
                            {

                                foreach (var contact in input.membersDto)
                                {
                                    contact.customeropt = 1; // Unsubscribed


                                }
                            }

                            List<List<MembersDto>> splitLists = new List<List<MembersDto>>();

                            if (input.membersDto.Count > 1000)
                            {

                                splitLists = SplitList(input.membersDto, 10);
                            }
                            else
                            {

                                splitLists = SplitList(input.membersDto, 1);
                            }


                            // Display the count of each split list
                            string sendgroups = "creategroups";
                            int count = 1;
                            long resultId = 0;

                            foreach (var OuterList in splitLists)
                            {
                                var JopName = sendgroups + count.ToString();
                                resultId = 0;
                                if (OuterList.Count == 0)
                                {
                                    break;
                                }
                                GroupSetQueueDBModel groupSetQueueModel = new GroupSetQueueDBModel();

                                groupSetQueueModel.tenantId = tenantId;
                                groupSetQueueModel.groupId = outId;
                                groupSetQueueModel.contactJson = JsonConvert.SerializeObject(OuterList);
                                groupSetQueueModel.createdDate = DateTime.UtcNow.AddHours(3);
                                groupSetQueueModel.IsExternalContact = isExternal;
                                groupSetQueueModel.IsCreated = false;
                                //Set Queue details on database
                                resultId = await SetQueueDetailsDB(groupSetQueueModel, input.membersDto.Count);

                                count++;
                                if (resultId != 0 && JopName == "creategroups1")
                                {
                                    GroupSetQueueModel groupSetQueueModelS = new GroupSetQueueModel();

                                    groupSetQueueModelS.rowId = resultId;
                                    groupSetQueueModelS.tenantId = tenantId;
                                    groupSetQueueModelS.groupId = input.groupDtoModel.id;
                                    groupSetQueueModelS.functionName = JopName;
                                    groupSetQueueModelS.groupName = input.groupDtoModel.groupName;
                                    SetGroupQueueContact(groupSetQueueModelS);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        groupModel.id = outId;
                        groupModel.groupName = input.groupDtoModel.groupName;

                        return groupModel;
                    }
                }
                groupModel.id = 0;
                return groupModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<long> SetQueueDetailsDB(GroupSetQueueDBModel groupSetQueueDBModel, int contactCount)
        {
            try
            {
                var SP_Name = Constants.Groups.SP_GroupsSetQueueDB;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",groupSetQueueDBModel.tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@groupId",groupSetQueueDBModel.groupId) ,
                    new System.Data.SqlClient.SqlParameter("@contactJson",groupSetQueueDBModel.contactJson) ,
                    new System.Data.SqlClient.SqlParameter("@createdDate",groupSetQueueDBModel.createdDate) ,
                    new System.Data.SqlClient.SqlParameter("@IsExternalContact",groupSetQueueDBModel.IsExternalContact) ,
                    new System.Data.SqlClient.SqlParameter("@IsCreated",groupSetQueueDBModel.IsCreated) ,
                    new System.Data.SqlClient.SqlParameter("@contactCount",contactCount) ,

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@outPut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                long outId = (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
                return outId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetGroupQueueContact(GroupSetQueueModel groupSetQueueModel)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference(groupSetQueueModel.functionName);
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           groupSetQueueModel
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception)
            {
                var Error = JsonConvert.SerializeObject(groupSetQueueModel);
            }
        }
        private CustomerModel CreateNewCustomer(string from, string name, string type, string botID, int TenantId, string D360Key)
        {
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
            if (string.IsNullOrEmpty(idcont.Group))
            {
                idcont.Group = "0";
                idcont.GroupName = "";
            }
            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId = D360Key,
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
                customerChat = new CustomerChat()
                {
                    CreateDate = DateTime.Now,
                },
                creation_timestamp = 0,
                expiration_timestamp = 0,
                // ConversationId = model.statuses.FirstOrDefault().conversation.id
                CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false },
                GroupName = idcont.GroupName,
                GroupId = long.Parse(idcont.Group)
            };

            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;
            //CustomerModel._self = Result.Uri;
            //CustomerModel._rid = Result.ID;

            var itemsCollection2 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;
            return CustomerModel;
        }

        //private async Task<GroupCreateDto> AddOldMembersOnGroup(List<MembersDto> membersDto, long groupId)
        //{
        //    try
        //    {
        //        GroupCreateDto model = new GroupCreateDto();
        //        model.failedAdd = new List<MembersDto>();
        //        if (AbpSession.TenantId.HasValue && membersDto.Count > 0)
        //        {
        //            //List<MembersDto> newMembersList = new List<MembersDto>();
        //            int tenantId = AbpSession.TenantId.Value;

        //            var SP_Name = Constants.Groups.SP_AddMembersOnGroup;
        //            string str = JsonConvert.SerializeObject(membersDto);

        //            var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
        //                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
        //                    new System.Data.SqlClient.SqlParameter("@groupId",groupId) ,
        //                    new System.Data.SqlClient.SqlParameter("@membersJson",str)
        //                };
        //            var failedOutputParameter = new System.Data.SqlClient.SqlParameter
        //            {
        //                SqlDbType = SqlDbType.Int,
        //                ParameterName = "@failedCount",
        //                Direction = ParameterDirection.Output
        //            };
        //            sqlParameters.Add(failedOutputParameter);
        //            var successOutputParameter = new System.Data.SqlClient.SqlParameter
        //            {
        //                SqlDbType = SqlDbType.Int,
        //                ParameterName = "@successCount",
        //                Direction = ParameterDirection.Output
        //            };
        //            sqlParameters.Add(successOutputParameter);

        //            model.failedAdd = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMembersUpdate, AppSettingsModel.ConnectionStrings).ToList();

        //            model.failedCount = (failedOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(failedOutputParameter.Value) : 0;
        //            model.successCount = (successOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(successOutputParameter.Value) : 0;

        //            return model;
        //        }
        //        model.id = 0;
        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //private async Task<GroupCreateDto> AddNewMembersOnGroupAsync(List<MembersDto> membersDto, long groupId)
        //{
        //    try
        //    {
        //        GroupCreateDto model = new GroupCreateDto();
        //        model.failedAdd = new List<MembersDto>(); // Initialize failedAdd as a new list
        //        if (AbpSession.TenantId.HasValue && membersDto.Count > 0)
        //        {
        //            List<MembersDto> newMembersList = new List<MembersDto>();
        //            TenantModel tenantModel = new TenantModel();

        //            int tenantId = AbpSession.TenantId.Value;

        //            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
        //            tenantModel = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);

        //            if (tenantModel != null)
        //            {
        //                foreach (MembersDto member in membersDto)
        //                {
        //                    member.phoneNumber = member.phoneNumber.Trim();

        //                    if (member.phoneNumber.Length >= 10 && member.phoneNumber.Length <= 15 && (long.TryParse(member.phoneNumber, out _)))
        //                    {
        //                        string userId = tenantId + "_" + member.phoneNumber;
        //                        var CustomerCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
        //                        var customerResult = CustomerCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == tenantId);
        //                        var Customer = customerResult.Result;

        //                        CustomerModel customerModel1 = new CustomerModel();
        //                        if (Customer == null)
        //                        {
        //                            //type = text
        //                            customerModel1 = CreateNewCustomer(member.phoneNumber, member.displayName, "text", tenantModel.botId, tenantId, tenantModel.D360Key);
        //                            member.id = int.Parse(customerModel1.ContactID);
        //                        }
        //                        else
        //                        {
        //                            member.id = int.Parse(Customer.ContactID);
        //                        }
        //                        newMembersList.Add(member);
        //                    }
        //                    else
        //                    {
        //                        model.failedAdd.Add(member);
        //                        model.failedCount++;
        //                        continue;
        //                    }
        //                }

        //                var SP_Name = Constants.Groups.SP_AddMembersOnGroup;
        //                string str = JsonConvert.SerializeObject(newMembersList);

        //                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
        //                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
        //                    new System.Data.SqlClient.SqlParameter("@groupId",groupId) ,
        //                    new System.Data.SqlClient.SqlParameter("@membersJson",str)
        //                };
        //                var failedOutputParameter = new System.Data.SqlClient.SqlParameter
        //                {
        //                    SqlDbType = SqlDbType.Int,
        //                    ParameterName = "@failedCount",
        //                    Direction = ParameterDirection.Output
        //                };
        //                sqlParameters.Add(failedOutputParameter);
        //                var successOutputParameter = new System.Data.SqlClient.SqlParameter
        //                {
        //                    SqlDbType = SqlDbType.Int,
        //                    ParameterName = "@successCount",
        //                    Direction = ParameterDirection.Output
        //                };
        //                sqlParameters.Add(successOutputParameter);

        //                model.failedAdd = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMembersUpdate, AppSettingsModel.ConnectionStrings).ToList();

        //                model.failedCount = (failedOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(failedOutputParameter.Value) : 0;
        //                model.successCount = (successOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(successOutputParameter.Value) : 0;

        //                return model;
        //            }
        //        }
        //        model.id = 0;
        //        return model;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private async Task<long> groupUpdate(GroupMembersDto input)
        {
            try
            {
                int res = groupCheck(input.groupDtoModel.id);
                if (res == 0) { return 0; }
                var SP_Name = Constants.Groups.SP_GroupsUpdate;
                int tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@groupId",input.groupDtoModel.id) ,
                    new System.Data.SqlClient.SqlParameter("@groupName",input.groupDtoModel.groupName) ,
                    new System.Data.SqlClient.SqlParameter("@modificationDate",DateTime.UtcNow.AddHours(3)) ,
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
            catch
            {
                return 0;
            }
        }

        private int groupCheck(long num)
        {
            try
            {
                var SP_Name = Constants.Groups.SP_GroupCheck;
                int tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@GroupId",num)
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
        static List<List<T>> SplitList<T>(List<T> list, int numberOfLists)
        {
            int chunkSize = (int)Math.Ceiling((double)list.Count / numberOfLists);

            return Enumerable.Range(0, numberOfLists)
                .Select(i => list.Skip(i * chunkSize).Take(chunkSize).ToList())
                .ToList();
        }
        private async Task<GroupCreateDto> updateMembers(GroupMembersDto input, bool isExternal)
        {
            GroupCreateDto groupModel = new GroupCreateDto();
            try
            {
                if (AbpSession.TenantId.HasValue && input.membersDto.Count > 0)
                {
                    List<List<MembersDto>> splitLists = new List<List<MembersDto>>();

                    if (input.membersDto.Count > 1000)
                    {

                        splitLists = SplitList(input.membersDto, 10);
                    }
                    else
                    {

                        splitLists = SplitList(input.membersDto, 1);
                    }


                    // Display the count of each split list
                    string sendgroups = "creategroups";
                    int count = 1;
                    int tenantId = AbpSession.TenantId.Value;
                    long resultId = 0;

                    foreach (var OuterList in splitLists)
                    {
                        var JopName = sendgroups + count.ToString();
                        resultId = 0;
                        if (OuterList.Count == 0)
                        {
                            break;
                        }
                        GroupSetQueueDBModel groupSetQueueModel = new GroupSetQueueDBModel();

                        groupSetQueueModel.tenantId = tenantId;
                        groupSetQueueModel.groupId = input.groupDtoModel.id;
                        groupSetQueueModel.contactJson = JsonConvert.SerializeObject(OuterList);
                        groupSetQueueModel.createdDate = DateTime.UtcNow.AddHours(3);
                        groupSetQueueModel.IsExternalContact = isExternal;
                        groupSetQueueModel.IsCreated = false;
                        //Set Queue details on database
                        resultId = await SetQueueDetailsDB(groupSetQueueModel, input.membersDto.Count);
                        count++;
                        if (resultId != 0 && JopName == "creategroups1")
                        {
                            GroupSetQueueModel groupSetQueueModelS = new GroupSetQueueModel();

                            groupSetQueueModelS.rowId = resultId;
                            groupSetQueueModelS.tenantId = tenantId;
                            groupSetQueueModelS.groupId = input.groupDtoModel.id;
                            groupSetQueueModelS.functionName = JopName;
                            groupSetQueueModelS.groupName = input.groupDtoModel.groupName;
                            SetGroupQueueContact(groupSetQueueModelS);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    groupModel.id = input.groupDtoModel.id;
                    groupModel.groupName = input.groupDtoModel.groupName;

                    return groupModel;
                    //GroupSetQueueDBModel groupSetQueueModel = new GroupSetQueueDBModel();
                    //int tenantId = AbpSession.TenantId.Value;
                    //groupSetQueueModel.tenantId = tenantId;
                    //groupSetQueueModel.groupId = input.groupDtoModel.id;
                    //groupSetQueueModel.contactJson = JsonConvert.SerializeObject(input.membersDto);
                    //groupSetQueueModel.createdDate = DateTime.UtcNow.AddHours(3);
                    //groupSetQueueModel.IsExternalContact = isExternal;
                    //groupSetQueueModel.IsCreated = false;
                    ////Set Queue details on database
                    //var resultId = await SetQueueDetailsDB(groupSetQueueModel, input.membersDto.Count);

                    //if (resultId != 0)
                    //{
                    //    GroupSetQueueModel groupSetQueueModelS = new GroupSetQueueModel();

                    //    groupSetQueueModelS.rowId = resultId;
                    //    groupSetQueueModelS.tenantId = tenantId;
                    //    groupSetQueueModelS.groupId = input.groupDtoModel.id;
                    //    groupSetQueueModelS.functionName = "creategroups";
                    //    groupSetQueueModelS.groupName = input.groupDtoModel.groupName;
                    //    SetGroupQueueContact(groupSetQueueModelS);

                    //    groupModel.id = input.groupDtoModel.id;
                    //    groupModel.groupName = input.groupDtoModel.groupName;

                    //    return groupModel;
                    //}
                }

                groupModel.id = 0;
                return groupModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private GroupCreateDto removeMembers(GroupMembersDto input)
        {
            GroupCreateDto groupModel = new GroupCreateDto();
            groupModel.failedAdd = new List<MembersDto>(); // Initialize failedAdd as a new list
            try
            {
                if (AbpSession.TenantId.HasValue && input.membersDto.Count > 0)
                {
                    var SP_Name = Constants.Groups.SP_RemoveMembers;
                    int tenantId = AbpSession.TenantId.Value;
                    string str = JsonConvert.SerializeObject(input.membersDto);
                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                            new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                            new System.Data.SqlClient.SqlParameter("@groupId",input.groupDtoModel.id) ,
                            new System.Data.SqlClient.SqlParameter("@membersJson",str)
                    };
                    var failedOutputParameter = new System.Data.SqlClient.SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@failedCount",
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters.Add(failedOutputParameter);
                    var successOutputParameter = new System.Data.SqlClient.SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@successCount",
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters.Add(successOutputParameter);

                    groupModel.failedAdd = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMembersUpdate, AppSettingsModel.ConnectionStrings).ToList();

                    groupModel.failedCount = (failedOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(failedOutputParameter.Value) : 0;
                    groupModel.successCount = (successOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(successOutputParameter.Value) : 0;

                    groupModel.id = input.groupDtoModel.id;
                    groupModel.groupName = input.groupDtoModel.groupName;


                    if (input.membersDto != null)
                    {
                        foreach (var member in input.membersDto)
                        {
                            var itemsCollection3 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                            var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == tenantId + "_" + member.phoneNumber && a.TenantId == tenantId);
                            var Customer3 = new CustomerModel();
                            try
                            {
                                Customer3 = customerResult3.Result;




                            }
                            catch { Customer3 = null; }
                            if (Customer3 == null)
                            {

                            }
                            else
                            {
                                if (Customer3.GroupId == input.groupDtoModel.id)
                                {
                                    Customer3.GroupId = 0;
                                    Customer3.GroupName = "";
                                    var Result = itemsCollection3.UpdateItemAsync(Customer3._self, Customer3).Result;
                                }

                            }
                        }
                    }

                    return groupModel;
                }
                else
                {
                    groupModel.id = 0;
                    return groupModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private GroupModel groupDelete(long id)
        {
            GroupModel groupModel = new GroupModel();
            try
            {
                var SP_Name = Constants.Groups.SP_GroupDelete;
                int tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@groupId",id) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                groupModel.state = 2;
                groupModel.message = "OK";
                groupModel.total = OutputParameter.Value != DBNull.Value ? Convert.ToInt64(OutputParameter.Value) : 0;

                if (groupModel.total == 0)
                {
                    groupModel.state = 2;
                    groupModel.message = "No results found";
                }
                return groupModel;
            }
            catch (Exception ex)
            {
                groupModel.state = -1;
                groupModel.message = ex.Message;
                return groupModel;
            }
        }

        private Dictionary<string, dynamic> validGroupName(string groupName)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();
                if (!string.IsNullOrEmpty(groupName) && groupName.Length <= 20)
                {
                    var model = GroupGetAll("", 0, int.MaxValue)
                                    .groupDtoModel
                                    .Where(x => x.groupName.ToLower().Trim() == groupName.ToLower().Trim())
                                    .FirstOrDefault();

                    if (model == null)
                    { response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", "Ok" } }; }
                    else
                    { response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The group name is used" } }; }
                }
                else
                {
                    response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "The group name must not be empty and must not exceed 20 characters" } };
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }

        private GroupCreateDto movingGroup(MoveMembersDto input)
        {
            GroupCreateDto model = new GroupCreateDto();
            try
            {
                var SP_Name = Constants.Groups.SP_MovingMembersFromGroup;
                int tenantId = AbpSession.TenantId.Value;
                string membersDto = JsonConvert.SerializeObject(input.membersDto);
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@OldGroupId",input.OldGroupId) ,
                    new System.Data.SqlClient.SqlParameter("@NewGroupId",input.NewGroupId) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@membersJson",membersDto)
                };
                var failedOutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@failedCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(failedOutputParameter);
                var successOutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@successCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(successOutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                model.failedCount = (failedOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(failedOutputParameter.Value) : 0;
                model.successCount = (successOutputParameter.Value != DBNull.Value) ? Convert.ToInt32(successOutputParameter.Value) : 0;

                model.groupName = "";
                model.id = input.NewGroupId;
                model.state = 2;
                model.message = "OK";

                if ((model.successCount == 0 && model.failedCount > 0) || (model.successCount > 0 && model.failedCount > 0))
                {
                    model.state = 2;
                    model.message = "Members already exist in the group";
                }
                else if (model.successCount == 0)
                {
                    model.state = 2;
                    model.message = "No results found";
                }

                return model;
            }
            catch (Exception ex)
            {
                model.state = -1;
                model.message = ex.Message;
                return model;
            }
        }
        private async Task<GroupLog> groupLogGetAll(long groupId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10)
        {
            GroupLog groupModel = new GroupLog();
            groupModel.members = new List<GroupPhoneNumberLog>();
            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "")
                    searchTerm = searchTerm.ToLower();

                var SP_Name = Constants.Groups.SP_GroupsLogGetAll;
                int tenantId = AbpSession.TenantId.Value;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@groupId",groupId) ,
                    new System.Data.SqlClient.SqlParameter("@searchTerm",searchTerm) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                groupModel.members = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGroupsLog, AppSettingsModel.ConnectionStrings).ToList();

                groupModel.totalCount = OutputParameter.Value != DBNull.Value ? Convert.ToInt64(OutputParameter.Value) : 0;

                return groupModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private bool IsUserAdmin()
        {
            var user = UserManager.GetUserAsync(AbpSession.ToUserIdentifier()).Result;
            return UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Admin).Result;
        }
        #endregion

        #region Group Progress 

        public GroupProgressDto GetGroupProgress(long groupId, int tenantId)
        {

            GroupDtoModel group = groupGetById(groupId, tenantId);
            if (group == null) return new GroupProgressDto();

            var total = group.OnHoldCount + group.totalNumber + group.FailedCount;
            var inserted = group.totalNumber + group.FailedCount;
            var remaining = group.OnHoldCount;

            var percent = total > 0 ? Math.Round((double)inserted / total * 100) : 0;

            return new GroupProgressDto
            {
                Total = total,
                Inserted = inserted,
                Remaining = remaining,
                ProgressPercent = percent
            };
        }


        #endregion
    }
}