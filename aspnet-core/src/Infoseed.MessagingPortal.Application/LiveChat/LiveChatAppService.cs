using Framework.Data;
using Google.Apis.Auth.OAuth2;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.Departments;
using Infoseed.MessagingPortal.Departments.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Teams.Dto;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Microsoft.Azure.Documents;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;
using static Infoseed.MessagingPortal.LiveChat.Dto.LiveChatEnums;

namespace Infoseed.MessagingPortal.LiveChat
{
    public class LiveChatAppService : MessagingPortalAppServiceBase, ILiveChatAppService
    {
        private readonly IDocumentClient _IDocumentClient;
        private readonly IExcelExporterAppService _ExcelExporter;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IDashboardUIAppService _dashboardUIAppService;
        private readonly IUserAppService _iUserAppService;

        public LiveChatAppService(IDocumentClient IDocumentClient, IDepartmentAppService departmentAppService, IDashboardUIAppService dashboardUIAppService, IUserAppService iUserAppService, IExcelExporterAppService ExcelExporter = null)
        {
            _IDocumentClient=IDocumentClient;
            _ExcelExporter = ExcelExporter;
            _departmentAppService = departmentAppService;
            _dashboardUIAppService = dashboardUIAppService;
            _iUserAppService=iUserAppService;

        }

        public LiveChatAppService()
        {

        }

        public CustomerLiveChatModel AddLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, string Department1 = null, string Department2 = null, bool IsOpen = true, int DepartmentId = 0, string UserIds = "")
        {
            return addLiveChat(tenantId, phoneNumber, userId, displayName, liveChatStatus, isliveChat, Department1, Department2, IsOpen, DepartmentId, UserIds);
        }
        public CustomerLiveChatModel AddNewLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, string type, string Department1 = null, string Department2 = null, bool IsOpen = true, int DepartmentId = 0, string UserIds = "", CustomerLiveChatModel newmodel = null, DateTime? ContactCreationDate = null)
        {
            return addNewLiveChat(tenantId, phoneNumber, userId, displayName, liveChatStatus, isliveChat, type, Department1, Department2, IsOpen, DepartmentId, UserIds, newmodel);
        }
        public LiveChatEntity GetLiveChat(string phoneNumber, string filteredUserId, string name, string startDate, string endDate, int? pageNumber = 0, int? pageSize = 50)
        {
            return getLiveChat(phoneNumber, filteredUserId, name, startDate, endDate, pageNumber, pageSize);
        }
        public LiveChatEntity GetTicket(DateTime? startDate, DateTime? endDate, string phoneNumber = null, string filteredUserId = null, string name = null, string departemnt = null, string ticketType = null, int? statusId = 0, int? pageNumber = 0, int? pageSize = 50, string ticketId = null, string userId = null, string summary = null, DateTime? startDateC = null, DateTime? endDateC = null, int byteam = 0)
        {
            return getTicketAsync(startDate, endDate, phoneNumber, filteredUserId, name, departemnt, ticketType, statusId, pageNumber, pageSize, ticketId, userId, summary, startDateC, endDateC, byteam).Result;
        }
        public FileDto GetLiveChatToExcel()
        {
            List<CustomerLiveChatModel> lstLiveChat = getLiveChatToExcel();
            if (lstLiveChat!= null)
            {
                return _ExcelExporter.ExportLiveChatToExcel(lstLiveChat);
            }
            else
            {
                return new FileDto();
            }
        }
        public CustomerLiveChatModel UpdateLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, int agentId, string agentName, int selectedLiveChatID = 0, bool IsOpen = true)
        {
            return updateLiveChat(tenantId, phoneNumber, userId, displayName, liveChatStatus, isliveChat, agentId, agentName, selectedLiveChatID, IsOpen);
        }


        public void UpdateIsOpenLiveChat(int tenantId, string phoneNumber, bool IsOpen, int ConversationsCount = 0, int creationTimestamp = 0, int expirationTimestamp = 0)
        {
            updateIsOpenLiveChat(tenantId, phoneNumber, IsOpen, ConversationsCount, creationTimestamp, expirationTimestamp);
        }
        public void UpdateNote(int tenantId, string phoneNumber,bool IsNote)
        {
            updateNote(tenantId, phoneNumber, IsNote);
        }

        public CustomerLiveChatModel UpdateTicket(int agentId, string agentName, int TicketId, int liveChatStatus, bool IsOpen = true, string Summary = "")
        {
            return UpdateTicketFun(agentId, agentName, TicketId, liveChatStatus, IsOpen, Summary);
        }

        public void UpdateConversationsCount(string userId)
        {
            var x = UpdateConversationsCountFUN(userId);
        }

        public void AssignLiveChatToUser(long liveChatId, string usersIds, string userName = "", string UserAssignName = "", long UserAssign = 0, string TeamsIds = "")
        {
            assignLiveChatToUser(liveChatId, usersIds, userName, UserAssignName, UserAssign, TeamsIds);
        }







        #region Private Methods
        private async Task<LiveChatEntity> getTicketAsync(DateTime? startDate, DateTime? endDate, string phoneNumber = null, string filteredUserId = null, string name = null,string departemnt=null, string ticketType = null, int? statusId = 0, int? pageNumber = 0, int? pageSize = 50, string ticketId = null, string userId = null, string summary = null, DateTime? startDateC = null, DateTime? endDateC = null, int byteam = 0)
        {
            try
            {


                if (string.IsNullOrEmpty(userId))
                {
                    userId = AbpSession.UserId.ToString();

                }
                LiveChatEntity liveChatEntity = new LiveChatEntity();
                var SP_Name = Constants.LiveChat.SP_newLiveChatGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber),
                   new System.Data.SqlClient.SqlParameter("@pageSize",pageSize),
                   new System.Data.SqlClient.SqlParameter("@tenantId",AbpSession.TenantId.Value),
                   new System.Data.SqlClient.SqlParameter("@userId",userId),
                   new System.Data.SqlClient.SqlParameter("@PhoneNumber",phoneNumber),
                   new System.Data.SqlClient.SqlParameter("@FilteredUserId",filteredUserId),
                   new System.Data.SqlClient.SqlParameter("@StartDate",startDate),
                   new System.Data.SqlClient.SqlParameter("@EndDate",endDate),
                   new System.Data.SqlClient.SqlParameter("@Name",name),
                   new System.Data.SqlClient.SqlParameter("@Departemnt",departemnt),
                   new System.Data.SqlClient.SqlParameter("@ticketType",ticketType),
                   new System.Data.SqlClient.SqlParameter("@statusId",statusId),
                   new System.Data.SqlClient.SqlParameter("@ticketId",ticketId),
                   new System.Data.SqlClient.SqlParameter("@summary",summary),
                   new System.Data.SqlClient.SqlParameter("@StartDateC",startDateC),
                   new System.Data.SqlClient.SqlParameter("@EndDateC",endDateC),
                    new System.Data.SqlClient.SqlParameter("@teamId",byteam),
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                var OutputParameterPending = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalPending",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterPending);

                var OutputParameterOpen = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalOpen",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterOpen);

                var OutputParameterClosed = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalClosed",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterClosed);

                var OutputParameterExpired = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalExpired",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterExpired);

                var OutputParameterResolutionTime = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Decimal,
                    ParameterName = "@TotalResolutionTime",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterResolutionTime);

                liveChatEntity.lstLiveChat = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToGetCustomerLiveChatForViewDto, AppSettingsModel.ConnectionStrings).ToList();

                if (liveChatEntity.lstLiveChat.Count !=0)
                {
                    liveChatEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                    liveChatEntity.TotalPending = Convert.ToInt32(OutputParameterPending.Value);
                    liveChatEntity.TotalOpen = Convert.ToInt32(OutputParameterOpen.Value);
                    liveChatEntity.TotalClosed = Convert.ToInt32(OutputParameterClosed.Value);
                    liveChatEntity.TotalExpired = Convert.ToInt32(OutputParameterExpired.Value);

                    if (OutputParameterResolutionTime.Value != DBNull.Value && (decimal)OutputParameterResolutionTime.Value > 0)
                    {
                        liveChatEntity.TotalResolutionTime = Math.Round(Convert.ToDecimal(OutputParameterResolutionTime.Value), 2);
                    }
                }
                //List<string> vs = new List<string>();


                //foreach (var x in liveChatEntity.lstLiveChat)
                //{
                //    vs.Add(x.phoneNumber);
                //}

                //vs=  vs.Distinct().ToList();


                //foreach (var items in vs)
                //{

                //    try
                //    {
                //        var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                //        var c = await itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.TenantId == AbpSession.TenantId.Value && a.phoneNumber==items);

                //        if (c!=null)
                //        {



                //            if (c.IsConversationExpired==null)
                //            {
                //                c.IsConversationExpired=false;


                //            }
                //            if (c.expiration_timestamp==0 || c.creation_timestamp==0)
                //            {

                //                c.IsConversationExpired = true;

                //            }
                //            if (c.expiration_timestamp != 0)
                //            {
                //                var diff = c.expiration_timestamp - c.creation_timestamp;


                //                var offsetcreation = DateTimeOffset.FromUnixTimeSeconds(c.creation_timestamp);
                //                DateTime creationDate = offsetcreation.UtcDateTime;

                //                var offsetexpiration = DateTimeOffset.FromUnixTimeSeconds(c.expiration_timestamp);
                //                DateTime expirationDate = offsetexpiration.UtcDateTime;


                //                TimeSpan timediff = expirationDate - creationDate;
                //                int totalHoursforuser = (int)(timediff.TotalHours);


                //                if (DateTime.UtcNow <= expirationDate)
                //                {
                //                    c.IsConversationExpired = false;
                //                }
                //                else
                //                {

                //                    c.IsConversationExpired = true;
                //                }


                //            }

                //        }




                //        updateTicket(AbpSession.TenantId.Value, items, c.creation_timestamp, c.expiration_timestamp, c.IsConversationExpired);
                //    }
                //    catch
                //    {


                //    }

                //}






                //if(statusId==1) //pending
                //{
                //    liveChatEntity.TotalCount=liveChatEntity.TotalPending;
                //}
                //if (statusId==2 )//open
                //{
                //    liveChatEntity.TotalCount=liveChatEntity.TotalOpen;
                //}
                //if (statusId==3) //close
                //{
                //    liveChatEntity.TotalCount=liveChatEntity.TotalClosed;
                //}
                //if (statusId==6) //Expired
                //{
                //    liveChatEntity.TotalCount=liveChatEntity.TotalExpired;
                //}

                return liveChatEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void updateTicket(int TenantId, string phonenumber, int create, int exp, bool isConvertaionExp)
        {
            try
            {

                var SP_Name = "[dbo].[UpdateAllTiketByContact]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId),
                    new System.Data.SqlClient.SqlParameter("@phonenumber",phonenumber),
                     new System.Data.SqlClient.SqlParameter("@create",create),
                    new System.Data.SqlClient.SqlParameter("@exp",exp),
                     new System.Data.SqlClient.SqlParameter("@isConvertaionExp",isConvertaionExp)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private LiveChatEntity getLiveChat(string phoneNumber, string filteredUserId, string name, string startDate, string endDate, int? pageNumber = 0, int? pageSize = 50)
        {
            try
            {
                //if (startDate != null && endDate != null)
                //{
                //    startDate = DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString();
                //    endDate = DateTime.ParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString();
                //}
                //else
                //{
                //    startDate = null;
                //    endDate = null;
                //}

                LiveChatEntity liveChatEntity = new LiveChatEntity();
                var SP_Name = Constants.LiveChat.SP_LiveChatGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber),
                   new System.Data.SqlClient.SqlParameter("@pageSize",pageSize),
                   new System.Data.SqlClient.SqlParameter("@tenantId",AbpSession.TenantId.Value),
                   new System.Data.SqlClient.SqlParameter("@userId",AbpSession.UserId.ToString()),
                   new System.Data.SqlClient.SqlParameter("@PhoneNumber",phoneNumber),
                   new System.Data.SqlClient.SqlParameter("@FilteredUserId",filteredUserId),
                   new System.Data.SqlClient.SqlParameter("@StartDate",startDate),
                   new System.Data.SqlClient.SqlParameter("@EndDate",endDate),
                   new System.Data.SqlClient.SqlParameter("@Name",name),
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                var OutputParameterPending = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalPending",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterPending);

                var OutputParameterOpen = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalOpen",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterOpen);

                var OutputParameterClosed = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalClosed",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameterClosed);

                liveChatEntity.lstLiveChat = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToGetCustomerLiveChatForViewDto, AppSettingsModel.ConnectionStrings).ToList();

                liveChatEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                liveChatEntity.TotalPending = Convert.ToInt32(OutputParameterPending.Value);
                liveChatEntity.TotalOpen = Convert.ToInt32(OutputParameterOpen.Value);
                liveChatEntity.TotalClosed = Convert.ToInt32(OutputParameterClosed.Value);

                return liveChatEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CustomerLiveChatModel addNewLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, string type, string Department1 = "", string Department2 = "", bool IsOpen = true, int DepartmentId = 0, string UserIds = "", CustomerLiveChatModel newmodel = null, DateTime? ContactCreationDate = null)
        {
            try
            {

                if (ContactCreationDate==null)
                {
                    ContactCreationDate=new DateTime();

                }
                var SP_Name = Constants.LiveChat.SP_TickitAddAndRequest;

                if (Department2 == "\"\"")
                {
                    Department2 = "";
                }
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();
                string SellingRequestDetials = "";

                if (type == "Request" && !newmodel.IsRequestForm)
                {
                    if (string.IsNullOrEmpty(newmodel.userId))
                        newmodel.userId = "";

                    if (string.IsNullOrEmpty(newmodel.TeamsIds))
                        newmodel.TeamsIds = "";




                    if (DepartmentId == null)
                        DepartmentId = 0;
                    if (newmodel.ContactInfo == null)
                        newmodel.ContactInfo = "";
                    //if (newmodel.Price == null)
                    //    newmodel.Price = 0;

                    SellingRequestDetials = JsonConvert.SerializeObject(newmodel.lstSellingRequestDetailsDto);
                }
                else
                {
                    SellingRequestDetials = "noSellingRequestDetials";
                }
                string Departments = "";
                if (Department2 == "" || Department2 == null)
                {
                    Departments = Department1;
                }
                else
                {
                    Departments = Department1 + "-" + Department2;
                }

                if (type == "Request")
                {
                    sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                            new System.Data.SqlClient.SqlParameter("@RequestDescription", newmodel.RequestDescription),
                            new System.Data.SqlClient.SqlParameter("@CategoryType", type),
                            new System.Data.SqlClient.SqlParameter("@ContactId", newmodel.ContactId),
                            new System.Data.SqlClient.SqlParameter("@ContactInfo", newmodel.ContactInfo),
                            new System.Data.SqlClient.SqlParameter("@IsRequestForm",newmodel.IsRequestForm),
                            new System.Data.SqlClient.SqlParameter("@AreaId",newmodel.AreaId??Convert.DBNull),
                            new System.Data.SqlClient.SqlParameter("@tenantId", tenantId),
                            new System.Data.SqlClient.SqlParameter("@phoneNumber", phoneNumber),
                            new System.Data.SqlClient.SqlParameter("@userId", userId),
                            new System.Data.SqlClient.SqlParameter("@displayName", displayName),
                            new System.Data.SqlClient.SqlParameter("@LiveChatStatus", liveChatStatus),
                            new System.Data.SqlClient.SqlParameter("@requestedLiveChatTime", DateTime.UtcNow),
                            new System.Data.SqlClient.SqlParameter("@IsliveChat", isliveChat),
                            new System.Data.SqlClient.SqlParameter("@Department", Departments),
                            new System.Data.SqlClient.SqlParameter("@IsOpen", IsOpen),
                            new System.Data.SqlClient.SqlParameter("@DepartmentId", DepartmentId),
                            new System.Data.SqlClient.SqlParameter("@UserIds", UserIds)
                        };
                }
                else
                {
                    sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                            new System.Data.SqlClient.SqlParameter("@CategoryType", type),
                            //new System.Data.SqlClient.SqlParameter("@SellingRequestDetials", @SellingRequestDetials),
                            new System.Data.SqlClient.SqlParameter("@tenantId", tenantId),
                            new System.Data.SqlClient.SqlParameter("@phoneNumber", phoneNumber),
                            new System.Data.SqlClient.SqlParameter("@userId", userId),
                            new System.Data.SqlClient.SqlParameter("@displayName", displayName),
                            new System.Data.SqlClient.SqlParameter("@LiveChatStatus", liveChatStatus),
                            new System.Data.SqlClient.SqlParameter("@requestedLiveChatTime", DateTime.UtcNow),
                            new System.Data.SqlClient.SqlParameter("@IsliveChat", isliveChat),
                            new System.Data.SqlClient.SqlParameter("@Department", Departments),
                            new System.Data.SqlClient.SqlParameter("@IsOpen", IsOpen),
                            new System.Data.SqlClient.SqlParameter("@DepartmentId", DepartmentId),
                            new System.Data.SqlClient.SqlParameter("@UserIds", UserIds)
                        };
                }

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@IdLiveChat";
                OutputParameter.Direction = ParameterDirection.Output;

                var NumberNoteOutputParameter = new System.Data.SqlClient.SqlParameter();
                NumberNoteOutputParameter.SqlDbType = SqlDbType.Int;
                NumberNoteOutputParameter.ParameterName = "@NumberNoteOut";
                NumberNoteOutputParameter.Direction = ParameterDirection.Output;

                var outputContactCreationDate = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime2,
                    ParameterName = "@ContactCreationDate",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                sqlParameters.Add(NumberNoteOutputParameter);
                sqlParameters.Add(outputContactCreationDate);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                //for add Attachments
                //if (type == "Request" && SellingRequestDetials != "[]" && !newmodel.IsRequestForm && SellingRequestDetials != "noSellingRequestDetials" && (long)OutputParameter.Value != 0)
                //{
                //    SP_Name = "[dbo].[LiveChatAddSellingRequest]";
                //    sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                //        new System.Data.SqlClient.SqlParameter("@IdChat", (long)OutputParameter.Value ),
                //        new System.Data.SqlClient.SqlParameter("@tenantId", tenantId ),
                //        new System.Data.SqlClient.SqlParameter("@SellingRequestDetials", SellingRequestDetials)
                //    };
                //    OutputParameter = new System.Data.SqlClient.SqlParameter();
                //    OutputParameter.SqlDbType = SqlDbType.BigInt;
                //    OutputParameter.ParameterName = "@IdSellingRequest";
                //    OutputParameter.Direction = ParameterDirection.Output;

                //    sqlParameters.Add(OutputParameter);
                //    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                //}
                DepartmentModel department = new DepartmentModel();
                string departmentUserIds = null;
                if (DepartmentId != 0)
                {
                    department = _departmentAppService.GetDepartmentById(DepartmentId, true, false);
                    if (department != null)
                    {
                        departmentUserIds = department.UserIds;
                    }
                    else
                    {
                        departmentUserIds = "";
                    }
                }
                CustomerLiveChatModel customerLiveChatModel = new CustomerLiveChatModel();
                if (Department2 == "")
                {
                    customerLiveChatModel = new CustomerLiveChatModel
                    {
                        IdLiveChat = (long)OutputParameter.Value,
                        TenantId = tenantId,
                        phoneNumber = phoneNumber,
                        userId = userId,
                        displayName = displayName,
                        LiveChatStatus = liveChatStatus,
                        requestedLiveChatTime = DateTime.UtcNow,
                        ContactCreationDate = Convert.ToDateTime(outputContactCreationDate.Value),
                        IsliveChat = isliveChat,
                        Department = Department1,
                        IsOpen = IsOpen,
                        DepartmentUserIds = departmentUserIds,
                        LiveChatStatusName = Enum.GetName(typeof(LiveChatStatusEnum), liveChatStatus),
                        UserIds = UserIds,
                        CategoryType = type,
                        ActionTime = null,
                         NumberNote=(int)NumberNoteOutputParameter.Value,
                    };
                    if (type == "Request")
                    {
                        customerLiveChatModel.RequestDescription = newmodel.RequestDescription;
                    }
                }
                else
                {
                    customerLiveChatModel = new CustomerLiveChatModel
                    {
                        IdLiveChat = (long)OutputParameter.Value,
                        TenantId = tenantId,
                        phoneNumber = phoneNumber,
                        userId = userId,
                        displayName = displayName,
                        LiveChatStatus = liveChatStatus,
                        requestedLiveChatTime = DateTime.UtcNow,
                        ContactCreationDate = Convert.ToDateTime(outputContactCreationDate.Value),
                        IsliveChat = isliveChat,
                        Department = Department1 + "-" + Department2,
                        IsOpen = IsOpen,
                        DepartmentUserIds = departmentUserIds,
                        LiveChatStatusName = Enum.GetName(typeof(LiveChatStatusEnum), liveChatStatus),
                        UserIds = UserIds,
                        CategoryType = type,
                        ActionTime = null,
                        NumberNote=(int)NumberNoteOutputParameter.Value,
                    };
                    if (type == "Request")
                    {
                        customerLiveChatModel.RequestDescription = newmodel.RequestDescription;
                    }
                }
                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = tenantId.Value,
                    TypeId = (int)DashboardTypeEnum.LiveChat,
                    StatusId = (int)LiveChatStatusEnum.Pending,
                    StatusName = Enum.GetName(typeof(LiveChatStatusEnum), (int)LiveChatStatusEnum.Pending)
                });
                return customerLiveChatModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CustomerLiveChatModel addLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, string Department1 = "", string Department2 = "", bool IsOpen = true, int DepartmentId = 0, string UserIds = "")
        {
            try
            {
                var SP_Name = Constants.LiveChat.SP_CustomersLiveAdd;

                if (Department2 == "\"\"")
                {
                    Department2 = "";
                }
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();
                if (Department2 == "")
                {
                    sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                        new System.Data.SqlClient.SqlParameter("@tenantId", tenantId),
                        new System.Data.SqlClient.SqlParameter("@phoneNumber", phoneNumber),
                        new System.Data.SqlClient.SqlParameter("@userId", userId),
                        new System.Data.SqlClient.SqlParameter("@displayName", displayName),
                        new System.Data.SqlClient.SqlParameter("@LiveChatStatus", liveChatStatus),
                        new System.Data.SqlClient.SqlParameter("@requestedLiveChatTime", DateTime.UtcNow),
                        new System.Data.SqlClient.SqlParameter("@IsliveChat", isliveChat),
                        new System.Data.SqlClient.SqlParameter("@Department", Department1),
                        new System.Data.SqlClient.SqlParameter("@IsOpen", IsOpen),
                        new System.Data.SqlClient.SqlParameter("@DepartmentId", DepartmentId),
                        new System.Data.SqlClient.SqlParameter("@UserIds", UserIds),


                    };
                }
                else
                {
                    sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                        new System.Data.SqlClient.SqlParameter("@tenantId", tenantId),
                        new System.Data.SqlClient.SqlParameter("@phoneNumber", phoneNumber),
                        new System.Data.SqlClient.SqlParameter("@userId", userId),
                        new System.Data.SqlClient.SqlParameter("@displayName", displayName),
                        new System.Data.SqlClient.SqlParameter("@LiveChatStatus", liveChatStatus),
                        new System.Data.SqlClient.SqlParameter("@requestedLiveChatTime", DateTime.UtcNow),
                        new System.Data.SqlClient.SqlParameter("@IsliveChat", isliveChat),
                        new System.Data.SqlClient.SqlParameter("@Department", Department1+"-"+Department2),
                        new System.Data.SqlClient.SqlParameter("@IsOpen", IsOpen),
                        new System.Data.SqlClient.SqlParameter("@DepartmentId", DepartmentId),
                        new System.Data.SqlClient.SqlParameter("@UserIds", UserIds),
                    };
                }
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@IdLiveChat";
                OutputParameter.Direction = ParameterDirection.Output;


                var NumberNoteOutputParameter = new System.Data.SqlClient.SqlParameter();
                NumberNoteOutputParameter.SqlDbType = SqlDbType.Int;
                NumberNoteOutputParameter.ParameterName = "@NumberNoteOut";
                NumberNoteOutputParameter.Direction = ParameterDirection.Output;



                var outputContactCreationDate = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime2,
                    ParameterName = "@ContactCreationDate",
                    Direction = ParameterDirection.Output
                };


                sqlParameters.Add(NumberNoteOutputParameter);

                sqlParameters.Add(OutputParameter);
                sqlParameters.Add(outputContactCreationDate);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                DepartmentModel department = new DepartmentModel();
                string departmentUserIds = null;
                if (DepartmentId != 0)
                {
                    department = _departmentAppService.GetDepartmentById(DepartmentId, true, false);
                    if (department != null)
                    {
                        departmentUserIds = department.UserIds;
                    }
                    else
                    {
                        departmentUserIds = "";
                    }
                }
                CustomerLiveChatModel customerLiveChatModel = new CustomerLiveChatModel();
                if (Department2 == "")
                {
                    customerLiveChatModel = new CustomerLiveChatModel
                    {
                        IdLiveChat = (long)OutputParameter.Value,
                        TenantId = tenantId,
                        phoneNumber = phoneNumber,
                        userId = userId,
                        displayName = displayName,
                        LiveChatStatus = liveChatStatus,
                        requestedLiveChatTime = DateTime.UtcNow,
                        ContactCreationDate = Convert.ToDateTime(outputContactCreationDate.Value),
                        IsliveChat = isliveChat,
                        Department = Department1,
                        IsOpen = IsOpen,
                        DepartmentUserIds = departmentUserIds,
                        LiveChatStatusName = Enum.GetName(typeof(LiveChatStatusEnum), liveChatStatus),
                        UserIds = UserIds,
                        NumberNote= (int)NumberNoteOutputParameter.Value,
                    };
                }
                else
                {
                    customerLiveChatModel = new CustomerLiveChatModel
                    {
                        IdLiveChat = (long)OutputParameter.Value,
                        TenantId = tenantId,
                        phoneNumber = phoneNumber,
                        userId = userId,
                        displayName = displayName,
                        LiveChatStatus = liveChatStatus,
                        requestedLiveChatTime = DateTime.UtcNow,
                        ContactCreationDate = Convert.ToDateTime(outputContactCreationDate.Value),
                        IsliveChat = isliveChat,
                        Department = Department1 + "-" + Department2,
                        IsOpen = IsOpen,
                        DepartmentUserIds = departmentUserIds,
                        LiveChatStatusName = Enum.GetName(typeof(LiveChatStatusEnum), liveChatStatus),
                        UserIds = UserIds,
                        NumberNote= (int)NumberNoteOutputParameter.Value,
                    };
                }
                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = tenantId.Value,
                    TypeId = (int)DashboardTypeEnum.LiveChat,
                    StatusId = (int)LiveChatStatusEnum.Pending,
                    StatusName = Enum.GetName(typeof(LiveChatStatusEnum), (int)LiveChatStatusEnum.Pending)
                });
                return customerLiveChatModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CustomerLiveChatModel updateLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, int agentId, string agentName, int selectedLiveChatID = 0, bool IsOpen = true)
        {
            try
            {
                var SP_Name = Constants.LiveChat.SP_CustomersLiveUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId),
                    new System.Data.SqlClient.SqlParameter("@phoneNumber",phoneNumber),
                    new System.Data.SqlClient.SqlParameter("@userId",userId),
                    new System.Data.SqlClient.SqlParameter("@displayName",displayName),
                    new System.Data.SqlClient.SqlParameter("@LiveChatStatus",liveChatStatus),
                    new System.Data.SqlClient.SqlParameter("@IsliveChat",isliveChat),
                    new System.Data.SqlClient.SqlParameter("@agentId",agentId),
                    new System.Data.SqlClient.SqlParameter("@agentName",agentName),
                    new System.Data.SqlClient.SqlParameter("@selectedLiveChatID",selectedLiveChatID),
                    new System.Data.SqlClient.SqlParameter("@IsOpen", IsOpen),
                };


                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime2,
                    ParameterName = "@requestedLiveChatTime",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);

                var model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToGetCustomerLiveChatForViewDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = tenantId.Value,
                    TypeId = (int)DashboardTypeEnum.LiveChat,
                    StatusId = liveChatStatus,
                    StatusName = Enum.GetName(typeof(LiveChatStatusEnum), liveChatStatus)
                });

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void updateNote(int tenantId, string phoneNumber, bool IsNote)
        {
            try
            {

                var SP_Name = Constants.LiveChat.SP_UpdateNote;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId),
                    new System.Data.SqlClient.SqlParameter("@phoneNumber",phoneNumber),
                    new System.Data.SqlClient.SqlParameter("@IsNote", IsNote)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void updateIsOpenLiveChat(int tenantId, string phoneNumber, bool IsOpen = true, int ConversationsCount = 0, int creationTimestamp = 0, int expirationTimestamp = 0)
        {
            try
            {

                var SP_Name = Constants.LiveChat.SP_UpdateIsOpenLiveChat;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId),
                    new System.Data.SqlClient.SqlParameter("@phoneNumber",phoneNumber),
                    new System.Data.SqlClient.SqlParameter("@IsOpen", IsOpen),
                    new System.Data.SqlClient.SqlParameter("@ConversationsCount", ConversationsCount),
                    new System.Data.SqlClient.SqlParameter("@CreationTimestamp", creationTimestamp),
                    new System.Data.SqlClient.SqlParameter("@ExpirationTimestamp", expirationTimestamp)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private CustomerLiveChatModel UpdateTicketFun(int agentId, string agentName, int TicketId, int liveChatStatus, bool IsOpen = true, string Summary = "")
        {
            try
            {


                var SP_Name = Constants.LiveChat.SP_TicketUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                    {
                       new System.Data.SqlClient.SqlParameter("@tenantId",AbpSession.TenantId),
                       new System.Data.SqlClient.SqlParameter("@liveChatStatus",liveChatStatus),
                       new System.Data.SqlClient.SqlParameter("@agentId",agentId),
                       new System.Data.SqlClient.SqlParameter("@agentName",agentName),
                       new System.Data.SqlClient.SqlParameter("@ticketId",TicketId),
                       new System.Data.SqlClient.SqlParameter("@isOpen", IsOpen),
                       new System.Data.SqlClient.SqlParameter("@summary", Summary),

                     };


                //var OutputParameter = new System.Data.SqlClient.SqlParameter();
                //OutputParameter.SqlDbType = System.Data.SqlDbType.DateTime2;
                //OutputParameter.ParameterName = "@requestedLiveChatTime";
                //OutputParameter.Direction = System.Data.ParameterDirection.Output;

                //sqlParameters.Add(OutputParameter);
                var model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToGetCustomerLiveChatForViewDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                // var xx=JsonConvert.SerializeObject(model);
                // model.IsOpen = false;
                // SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),  AppSettingsModel.ConnectionStrings);
                //CustomerLiveChatModel customerLiveChatModel = new CustomerLiveChatModel();
                //if (IsOpen)
                //{

                //    customerLiveChatModel.IdLiveChat = TicketId;
                //    customerLiveChatModel.TenantId = AbpSession.TenantId;
                //    customerLiveChatModel.LiveChatStatus = liveChatStatus;
                //    customerLiveChatModel.agentId = agentId;
                //    customerLiveChatModel.LockedByAgentName = agentName;
                //    customerLiveChatModel.IsLockedByAgent = true;
                //    customerLiveChatModel.IsOpen = IsOpen;
                //    customerLiveChatModel.TicketSummary=Summary;
                //    //customerLiveChatModel.expiration_timestamp=Summary;
                //    //customerLiveChatModel.creation_timestamp=Summary;
                //    customerLiveChatModel.OpenTimeTicket=DateTime.Now.AddHours(AppSettingsModel.AddHour);
                //}
                //else
                //{
                //    customerLiveChatModel.IdLiveChat = TicketId;
                //    customerLiveChatModel.TenantId = AbpSession.TenantId;
                //    customerLiveChatModel.LiveChatStatus = liveChatStatus;
                //    customerLiveChatModel.agentId = agentId;
                //    customerLiveChatModel.LockedByAgentName = agentName;
                //    customerLiveChatModel.IsLockedByAgent = true;
                //    customerLiveChatModel.IsOpen = IsOpen;
                //    customerLiveChatModel.TicketSummary=Summary;
                //    customerLiveChatModel.CloseTimeTicket=DateTime.Now.AddHours(AppSettingsModel.AddHour);
                //    //customerLiveChatModel.expiration_timestamp=Summary;
                //    //customerLiveChatModel.creation_timestamp=Summary;
                //}
                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = AbpSession.TenantId.Value,
                    TypeId = (int)DashboardTypeEnum.LiveChat,
                    StatusId = liveChatStatus,
                    StatusName = Enum.GetName(typeof(LiveChatStatusEnum), liveChatStatus)
                });
                model.TenantId = AbpSession.TenantId.Value;
                SocketIOManager.SendLiveChat(model, AbpSession.TenantId.Value);
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<CustomerModel> UpdateConversationsCountFUN(string userId)
        {
            try
            {
                var customer = new CustomerModel();


                List<CustomerLiveChatModel> lstLiveChat = new List<CustomerLiveChatModel>();
                var SP_Name = Constants.LiveChat.SP_UpdateConversationsCount;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@userId",userId)

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);



                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            //var customer = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId &&a.TenantId== AbpSession.TenantId);


            //customer.ConversationsCount=1;
            //var objCustomer = itemsCollection.UpdateItemAsync(customer._self, customer).Result;





        }
        private List<CustomerLiveChatModel> getLiveChatToExcel()
        {
            try
            {
                List<CustomerLiveChatModel> lstLiveChat = new List<CustomerLiveChatModel>();
                var SP_Name = Constants.LiveChat.SP_LiveChatToExcelGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@tenantId",AbpSession.TenantId.Value),
                   new System.Data.SqlClient.SqlParameter("@userId",AbpSession.UserId.ToString()),

                };

                lstLiveChat = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToGetCustomerLiveChatForViewDto, AppSettingsModel.ConnectionStrings).ToList();


                return lstLiveChat;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void assignLiveChatToUser(long liveChatId, string usersIds, string userName = "", string UserAssignName = "", long UserAssign = 0, string TeamsIds = "")
        {
            try
            {


                if (!string.IsNullOrEmpty(TeamsIds))
                {
                    var teams = teamsGetAll("", 0, 100000, AbpSession.TenantId.Value);


                    var list = "";


                    foreach (var te in teams.TeamsDtoModel)
                    {

                   
                            list=list+","+te.UserIds;

                    }
                    string result = string.Join(",", list.Split(',').Distinct());
                    usersIds=result;
                }




                var SP_Name = Constants.LiveChat.SP_LiveChatAssignToUserUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@LiveChatId",liveChatId),
                    new System.Data.SqlClient.SqlParameter("@UsersIds",usersIds),
                    new System.Data.SqlClient.SqlParameter("@UserName",userName),
                    new System.Data.SqlClient.SqlParameter("@AssignedToUserId",UserAssign),
                    new System.Data.SqlClient.SqlParameter("@AssignedToUserName",UserAssignName),
                      new System.Data.SqlClient.SqlParameter("@TeamsIds",TeamsIds),
                };

                CustomerLiveChatModel lstLiveChat = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToGetCustomerLiveChatForViewDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                try
                {
                    var titl = "New Ticket ";
                    var body = "From : " + userName;
                    // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                    SendMobileNotification(AbpSession.TenantId.Value, titl, body, true, usersIds);
                }
                catch (Exception)
                {

                }
                SocketIOManager.SendLiveChat(lstLiveChat, lstLiveChat.TenantId.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendMobileNotification(int TenaentId, string title, string msg, bool islivechat = false, string userIds = null, bool isCancelOrder = false)
        {



            var tenant = GetTenantById(TenaentId).Result;
            if (tenant.IsBellOn)
            {

                string mainSoundIOS = string.Empty;
                string mainSoundAndroid = string.Empty;

                if (isCancelOrder)
                {
                    mainSoundIOS = "cancel.caf";
                    mainSoundAndroid = "cancel";

                    if (tenant.IsBellContinues)
                    {
                        mainSoundIOS = "cancel_con.caf";
                        mainSoundAndroid = "cancel_con";
                    }
                }
                else
                {
                    mainSoundIOS = "sound.caf";
                    mainSoundAndroid = "sound";
                    if (tenant.IsBellContinues)
                    {
                        mainSoundIOS = "sound_con.caf";
                        mainSoundAndroid = "sound_con";
                    }
                }


                var tokens = GetUserByTeneantId(TenaentId, userIds);
                foreach (var token in tokens)
                {
                    var payload = new
                    {
                        message = new
                        {
                            token = token,
                            notification = new
                            {
                                title = title,
                                body = msg
                            },
                            android = new
                            {
                                priority = "high",
                                notification = new
                                {
                                    channel_id = $"high_importance_channel_{mainSoundAndroid}",
                                    sound = "sound.caf"
                                }
                            },
                            apns = new
                            {
                                headers = new
                                {
                                    apns_priority = "10"
                                },
                                payload = new
                                {
                                    aps = new
                                    {
                                        sound = mainSoundIOS
                                    }
                                }
                            },
                            data = new
                            {
                                title = title,
                                body = msg,
                                sound = mainSoundAndroid,
                            }
                        }
                    };

                    // Call your SendNotification method here with the current payload
                    var Serializer = new JavaScriptSerializer();
                    var json = Serializer.Serialize(payload);

                    SendNotificationAsync(json);
                }



            }
        }
        private async Task<string> GetAccessTokenAsync()
        {
            GoogleCredential credential;
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", "info-seed-firebase-adminsdk-nwflp-9dcbb1b151.json");

            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }

            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
        private async Task<string> SendNotificationAsync(string data)
        {
            string accessToken = await GetAccessTokenAsync(); // Get Bearer token
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/v1/projects/info-seed/messages:send");
            request.Headers.Add("Authorization", "Bearer "+accessToken);
            var content = new StringContent(data, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var x = await response.Content.ReadAsStringAsync();

            return "";
            // SendNotificationAsync(byteArray);
        }
        private async Task<TenantModel> GetTenantById(int? id)
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }
        private List<string> GetUserByTeneantId(int TenaentId, string userIds = null)
        {

            List<string> lstUserToken = new List<string>();
            var list = _iUserAppService.GetUserToken(TenaentId, userIds);
            if (list != null)
            {
                foreach (var item in list)
                {
                    lstUserToken.Add(item.Token);
                }
            }

            return lstUserToken;
        }

        private TeamsModel teamsGetAll(string searchTerm = "", int? pageNumber = 0, int? pageSize = 10, int tenantId = 0)
        {
            TeamsModel TeamsModel = new TeamsModel();
            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "")
                    searchTerm = searchTerm.ToLower();

                var SP_Name = Constants.Teams.SP_TeamsGetAll;


                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize) ,
                    new System.Data.SqlClient.SqlParameter("@searchTerm",searchTerm)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                TeamsModel.TeamsDtoModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTeams, AppSettingsModel.ConnectionStrings).ToList();


                TeamsModel.state = 2;
                TeamsModel.message = "OK";
                TeamsModel.total = OutputParameter.Value != DBNull.Value ? Convert.ToInt64(OutputParameter.Value) : 0;
                if (TeamsModel.total == 0)
                {
                    TeamsModel.state = 2;
                    TeamsModel.message = "No results found";
                }
                return TeamsModel;
            }
            catch (Exception ex)
            {
                TeamsModel.state = -1;
                TeamsModel.message = ex.Message;
                return TeamsModel;
            }
        }
        #endregion



    }
}
