using Abp.Runtime.Session;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Framework.Data;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.Departments;
using Infoseed.MessagingPortal.Departments.Dto;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.SealingReuest;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Documents;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Constants;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;

namespace Infoseed.MessagingPortal.SellingRequest
{
    //[AbpAuthorize(AppPermissions.Pages_SellingRequests)]
    public class SellingRequestAppService : MessagingPortalAppServiceBase, ISellingRequestAppService
    {
        private IHubContext<SellingRequestHub> _hub;
        private readonly IDepartmentAppService _departmentAppService;
        private readonly IDocumentClient _IDocumentClient;
        private IGeneralAppService _generalAppService;
        private readonly IDashboardUIAppService _dashboardUIAppService;


        public SellingRequestAppService(IHubContext<SellingRequestHub> hub, IDepartmentAppService departmentAppService, IGeneralAppService generalAppService, IDocumentClient iDocumentClient, IDashboardUIAppService dashboardUIAppService)
        {
            _hub = hub;
            _departmentAppService = departmentAppService;
            _IDocumentClient = iDocumentClient;
            _generalAppService = generalAppService;
            _dashboardUIAppService = dashboardUIAppService;
        }

        public SellingRequestAppService()
        {
        }
        public SellingRequestDto AddSellingRequest(SellingRequestDto sellingRequestDto)
        {
            return addSellingRequest(sellingRequestDto);
        }


        public void DeleteSellingRequest(long sellingRequestId)
        {
            _=deleteSellingRequestAsync(sellingRequestId);
        }

        public void DoneSellingRequest(long sellingRequestId)
        {
            _ = doneSellingRequestAsync(sellingRequestId);
        }
        public async Task<string> TicketUpdateStatus(long ticketId, int statusId, string summary,int type=1)
        {
            //LiveChatStatus = 4 >> LiveChatStatusName = "Confirm"
            //LiveChatStatus = 5 >> LiveChatStatusName = "Reject"


            if (type==1)
            {
                if (statusId == 4 || statusId == 5)
                {
                    return await ticketUpdateStatusAsync(ticketId, statusId, summary);
                }

            }
            else
            {

                if (statusId == 4 || statusId == 5)
                {
                    return await checkticketUpdateStatusAsync(ticketId, statusId, summary);
                }
            }


            return "an error occurred";
        }
        public long AddSginUpRequest(SellingRequestDto sellingRequestDto)
        {
            return addSginUpRequest(sellingRequestDto);
        }
        public SellingRequestEntity GetSellingRequest(int? tenantId = null, int pageNumber = 0, int pageSize = 50)
        {
            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;



            return getSellingRequest(tenantID.Value, pageNumber, pageSize);
        }

        public SellingRequestDto GetSellingRequestById(long sellingRequestId, int? tenantId)
        {
            int? tenantID = 0;
            if (tenantId.HasValue)
                tenantID = tenantId.Value;
            else
                tenantID = (int?)AbpSession.TenantId.Value;


            return getSellingRequestById(sellingRequestId, tenantID.Value);
        }

        public void UpdateSellingRequest(SellingRequestDto sellingRequestDto)
        {
            updateSellingRequest(sellingRequestDto);
        }






        #region private Method
        private SellingRequestDto addSellingRequest(SellingRequestDto sellingRequestDto)
        {
            try
            {
                if (string.IsNullOrEmpty(sellingRequestDto.UserId))
                    sellingRequestDto.UserId="";


                //if (sellingRequestDto.AreaId==null ||sellingRequestDto.AreaId==0)
                //    sellingRequestDto.AreaId=DBNull.Value;

                if (sellingRequestDto.DepartmentId==null)
                    sellingRequestDto.DepartmentId=0;
                if (sellingRequestDto.ContactInfo==null)
                    sellingRequestDto.ContactInfo="";

                if (sellingRequestDto.Price==null)
                    sellingRequestDto.Price=0;

                var SP_Name = Constants.SellingRequest.SP_SellingRequestAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                if (!sellingRequestDto.IsRequestForm)
                {
                    sqlParameters = new List<System.Data.SqlClient.SqlParameter>() {
                        new System.Data.SqlClient.SqlParameter("@ContactId",sellingRequestDto.ContactId)
                        ,new System.Data.SqlClient.SqlParameter("@SellingStatusId",sellingRequestDto.SellingStatusId)
                        ,new System.Data.SqlClient.SqlParameter("@UserId",sellingRequestDto.UserId)
                        ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",sellingRequestDto.PhoneNumber)
                        ,new System.Data.SqlClient.SqlParameter("@TenantId",sellingRequestDto.TenantId)
                        ,new System.Data.SqlClient.SqlParameter("@RequestDescription",sellingRequestDto.RequestDescription)
                        ,new System.Data.SqlClient.SqlParameter("@Price",sellingRequestDto.Price)
                        ,new System.Data.SqlClient.SqlParameter("@ContactInfo",sellingRequestDto.ContactInfo)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedBy",sellingRequestDto.CreatedBy)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedOn",sellingRequestDto.CreatedOn)
                        ,new System.Data.SqlClient.SqlParameter("@DepartmentId",sellingRequestDto.DepartmentId)
                        ,new System.Data.SqlClient.SqlParameter("@SellingRequestDetials",JsonConvert.SerializeObject( sellingRequestDto.lstSellingRequestDetailsDto))
                        ,new System.Data.SqlClient.SqlParameter("@IsRequestForm",sellingRequestDto.IsRequestForm)
                        //,new SqlParameter("@UserIds",sellingRequestDto.UserIds)
                        ,new System.Data.SqlClient.SqlParameter("@AreaId",sellingRequestDto.AreaId??Convert.DBNull)
                    };
                }
                else
                {
                    SP_Name= Constants.SellingRequest.SP_SellingRequestFormAdd;
                    sqlParameters = new List<System.Data.SqlClient.SqlParameter>() {
                        new System.Data.SqlClient.SqlParameter("@ContactId",sellingRequestDto.ContactId)
                        ,new System.Data.SqlClient.SqlParameter("@TenantId",sellingRequestDto.TenantId)
                        ,new System.Data.SqlClient.SqlParameter("@RequestDescription", sellingRequestDto.RequestDescription)
                        ,new System.Data.SqlClient.SqlParameter("@Price",sellingRequestDto.Price)
                        ,new System.Data.SqlClient.SqlParameter("@ContactInfo",sellingRequestDto.ContactInfo)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedBy",sellingRequestDto.CreatedBy)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedOn",sellingRequestDto.CreatedOn)
                        ,new System.Data.SqlClient.SqlParameter("@SellingStatusId",sellingRequestDto.SellingStatusId)
                        ,new System.Data.SqlClient.SqlParameter("@UserId",sellingRequestDto.UserId)
                        ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",sellingRequestDto.PhoneNumber)
                        ,new System.Data.SqlClient.SqlParameter("@IsRequestForm",sellingRequestDto.IsRequestForm)
                        ,new System.Data.SqlClient.SqlParameter("@DepartmentId",sellingRequestDto.DepartmentId)
                        //,new SqlParameter("@UserIds",sellingRequestDto.UserIds)
                        ,new System.Data.SqlClient.SqlParameter("@AreaId",sellingRequestDto.AreaId??Convert.DBNull)
                        // ,new SqlParameter("@SellingRequestDetials",JsonConvert.SerializeObject( sellingRequestDto.RequestDescription))
                        // ,new SqlParameter("@IsRequestForm",sellingRequestDto.IsRequestForm) 
                    };

                }




                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = System.Data.SqlDbType.BigInt,
                    ParameterName = "@SellingRequestId",
                    Direction = System.Data.ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);

                DepartmentModel department = new DepartmentModel();
                string departmentUserIds = null;
                if (sellingRequestDto.DepartmentId != 0)
                {
                    department = _departmentAppService.GetDepartmentById(sellingRequestDto.DepartmentId.Value, false, true);
                    if (department != null)
                    {
                        departmentUserIds = department.UserIds;
                    }
                    else
                    {
                        departmentUserIds = "";
                    }
                }
                sellingRequestDto.Id = (long)OutputParameter.Value;
                sellingRequestDto.DepartmentUserIds = departmentUserIds;


                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = sellingRequestDto.TenantId,
                    TypeId = (int)DashboardTypeEnum.Request,
                    StatusId = (int)SellingRequestStatus.Pending,
                    StatusName = Enum.GetName(typeof(SellingRequestStatus), (int)SellingRequestStatus.Pending)
                });
                return sellingRequestDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        private async Task deleteSellingRequestAsync(long sellingRequestId)
        {
            try
            {

                var SP_Name = Constants.SellingRequest.SP_SellingRequestDelete;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@Id",sellingRequestId) };




                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);
                SellingRequestDto sellingRequestDto = new SellingRequestDto
                {
                    SellingStatusId = 3,//delete
                    Id = sellingRequestId
                };

                await _hub.Clients.All.SendAsync("brodCastAgentSellingRequest", sellingRequestDto);

                SocketIOManager.SendSellingRequest(sellingRequestDto, (int)AbpSession.TenantId.Value);

                SellingRequestDto sellingRequest = getSellingRequestById(sellingRequestId, AbpSession.TenantId.Value);
                var user = GetCustomerAzuer(sellingRequest.ContactId.ToString());
                var tenant = _generalAppService.GetTenantById(AbpSession.TenantId.Value);

                if (!user.IsConversationExpired && tenant.RejectRequestText != null && tenant.RejectRequestText != "")
                {
                    SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
                    {
                        text = tenant.RejectRequestText,
                        type = "text",
                        agentName = "agnetname",
                        agentId = AbpSession.UserId.ToString()
                    };
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {
                        type = "text",
                        to = sellingRequest.PhoneNumber,
                        text = new PostWhatsAppMessageModel.Text()
                    };
                    postWhatsAppMessageModel.text.body = tenant.RejectRequestText;



                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, tenant.D360Key, tenant.AccessToken, tenant.IsD360Dialog);
                    if (result)
                    {
                        var CustomerChat = UpdateCustomerChat(sellingRequest.TenantId, message, user.userId, user.SunshineConversationId);
                        user.customerChat = CustomerChat;
                        SocketIOManager.SendContact(user, (int)user.TenantId);
                    }



                }
                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = sellingRequestDto.TenantId,
                    TypeId = (int)DashboardTypeEnum.Request,
                    StatusId = (int)SellingRequestStatus.Deleted,
                    StatusName = Enum.GetName(typeof(SellingRequestStatus), (int)SellingRequestStatus.Deleted)
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        private long addSginUpRequest(SellingRequestDto sellingRequestDto)
        {
            try
            {
                var SP_Name = Constants.SellingRequest.SP_SignUpRequestFormAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();
                sqlParameters = new List<System.Data.SqlClient.SqlParameter>() {
                    new System.Data.SqlClient.SqlParameter("@SellingStatusId",sellingRequestDto.SellingStatusId)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedBy",sellingRequestDto.SginUpRequest.OwnerName)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedOn",DateTime.UtcNow)
                    ,new System.Data.SqlClient.SqlParameter("@PhoneNumber", sellingRequestDto.SginUpRequest.ContactNumber)
                    ,new System.Data.SqlClient.SqlParameter("@SginUpRequestDescription",JsonConvert.SerializeObject( sellingRequestDto.SginUpRequest))
                    ,new System.Data.SqlClient.SqlParameter("@IsSginUpForm",sellingRequestDto.IsSginUpForm)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = System.Data.SqlDbType.BigInt,
                    ParameterName = "@RequestId",
                    Direction = System.Data.ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);

                return (long)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> checkticketUpdateStatusAsync(long ticketId, int statusId, string summary)
        {
            Status status = new Status();
            status = TicketStatusGet(ticketId);
            if (status != null && status.CategoryType == "Request" && summary != null)
            {
                //var user = GetCustomerAzuer(status.ContactId.ToString());
                var user = GetCustomerByUserIDAzuer(AbpSession.TenantId.Value.ToString()+"_"+status.phoneNumber.ToString());
                if (user==null)
                {
                    return "the Conversationis Expired";
                }

                //var model = getConversationSessions(AbpSession.TenantId.Value, status.phoneNumber.ToString(), "0");

                //long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                //// Check if the expiration time has passed
                //if (currentTimestamp > model.expiration_timestamp)
                //{

                //    if (currentTimestamp > user.expiration_timestamp)
                //    {
                //        return "the Conversationis Expired";

                //    }


                //}
            }
            return await ticketUpdateStatusAsync(ticketId, statusId, summary);
   

        }
        private async Task<string> ticketUpdateStatusAsync(long ticketId, int statusId, string summary)
        {
            try
            {
                var IsConversationExpired = false;
                Status status = new Status();
                status = TicketStatusGet(ticketId);
                if (status != null && status.CategoryType == "Request" && summary != null)
                {
                    //var user = GetCustomerAzuer(status.ContactId.ToString());
                    var user = GetCustomerByUserIDAzuer(AbpSession.TenantId.Value.ToString()+"_"+status.phoneNumber.ToString());
                 

                    var tenant = _generalAppService.GetTenantById(AbpSession.TenantId.Value);
                    int StatusId = 0;
                    string StatusName = "";
                    string RequestText = "";
                    switch (statusId)
                    {
                        case 4:
                            StatusId = 4;
                            StatusName = "Confirm";
                            RequestText = tenant.ConfirmRequestText;
                            break;
                        case 5:
                            StatusId = 5;
                            StatusName = "Reject";
                            RequestText = tenant.RejectRequestText;
                            break;
                    }
                    if ( RequestText != null && RequestText != "")
                    {
                        if (status.LiveChatStatus != 0)
                        {
                            switch (status.LiveChatStatus)
                            {
                                case 4:
                                    return "It's been confirmed before";
                                case 5:
                                    return "It's been rejected before";
                            }
                        }
                        else
                        {
                            return "an error occurred";
                        }

                        var SP_Name = Constants.SellingRequest.SP_TicketUpdateStatus;

                        int sessionId = 0;
                        if (AbpSession.UserId != null)
                        {
                            sessionId = (int)AbpSession.UserId;
                        }
                        else
                        {
                            return "an error occurred";
                        }

                        DateTime dateTime = DateTime.UtcNow;
                        var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                            new System.Data.SqlClient.SqlParameter("@TicketId",ticketId)
                            ,new System.Data.SqlClient.SqlParameter("@StatusId",statusId)
                            ,new System.Data.SqlClient.SqlParameter("@ActionTime",dateTime)
                            ,new System.Data.SqlClient.SqlParameter("@UserId",sessionId)
                            ,new System.Data.SqlClient.SqlParameter("@TicketSummary",summary)
                        };

                        var models = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToGetCustomerLiveChatForViewDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                        //CustomerModel customerModel = new CustomerModel
                        //{
                        //    LiveChatStatus = statusId,
                        //    IdLiveChat = ticketId
                        //};
                        //SocketIOManager.SendLiveChat(customerModel, (int)AbpSession.TenantId.Value);

                        models.TenantId = AbpSession.TenantId.Value;
                        //models.
                        SocketIOManager.SendLiveChat(models, AbpSession.TenantId.Value);
                        try
                        {
                            if (user!=null)
                            {
                                SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
                                {
                                    text = RequestText,
                                    type = "text",
                                    agentName = "agnetname",
                                    agentId = AbpSession.UserId.ToString()
                                };
                                PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel
                                {
                                    type = "text",
                                    to = status.phoneNumber,
                                    text = new PostWhatsAppMessageModel.Text()
                                };
                                postWhatsAppMessageModel.text.body = RequestText;

                                var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, tenant.D360Key, tenant.AccessToken, tenant.IsD360Dialog);
                                if (result)
                                {
                                    var CustomerChat = UpdateCustomerChat(status.TenantId, message, user.userId, user.SunshineConversationId);
                                    user.customerChat = CustomerChat;
                                    SocketIOManager.SendContact(user, (int)user.TenantId);
                                }
                            }
                        }
                        catch
                        {

                        }
  

     


                        _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                        {
                            TenantId = status.TenantId,
                            TypeId = (int)DashboardTypeEnum.Request,
                            StatusId = StatusId,
                            StatusName = StatusName
                        });

                        if (statusId == 4)
                        {
                            return "The operation has been Confirmation successfully";
                        }
                        else if (statusId == 5)
                        {
                            return "The operation has been rejected successfully";
                        }

                        return "an error occurred";
                    }
                }
                return "an error occurred";
            }
            catch (Exception ex)
            {
                throw new Exception("the Conversationis Expired");
            }


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
                ///model.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");

                return model;
            }
            catch (Exception)
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = 0;
                model.expiration_timestamp = 0;

                return model;
            }
        }
        private Status TicketStatusGet(long ticketId)
        {
            try
            {
                Status status = new Status();
                var SP_Name = Constants.SellingRequest.SP_TicketStatusGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TicketId",ticketId)
                };

                status = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.StatusGet, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task doneSellingRequestAsync(long sellingRequestId)
        {
            try
            {
                var SP_Name = Constants.SellingRequest.SP_SellingRequestDone;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Id",sellingRequestId)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                SellingRequestDto sellingRequestDto = new SellingRequestDto
                {
                    SellingStatusId = 2,//done
                    Id = sellingRequestId
                };

                await _hub.Clients.All.SendAsync("brodCastAgentSellingRequest", sellingRequestDto);
                SocketIOManager.SendSellingRequest(sellingRequestDto, (int)AbpSession.TenantId.Value);


                SellingRequestDto sellingRequest = getSellingRequestById(sellingRequestId, AbpSession.TenantId.Value);
                var user = GetCustomerAzuer(sellingRequest.ContactId.ToString());
                var tenant = _generalAppService.GetTenantById(AbpSession.TenantId.Value);

                if (!user.IsConversationExpired && tenant.ConfirmRequestText != null && tenant.ConfirmRequestText != "")
                {
                    SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
                    {
                        text = tenant.ConfirmRequestText,
                        type = "text",
                        agentName = "agnetname",
                        agentId = AbpSession.UserId.ToString()
                    };
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {
                        type = "text",
                        to = sellingRequest.PhoneNumber,
                        text = new PostWhatsAppMessageModel.Text()
                    };
                    postWhatsAppMessageModel.text.body = tenant.ConfirmRequestText;

                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, tenant.D360Key, tenant.AccessToken, tenant.IsD360Dialog);
                    if (result)
                    {
                        var CustomerChat = UpdateCustomerChat(sellingRequest.TenantId, message, user.userId, user.SunshineConversationId);
                        user.customerChat = CustomerChat;
                        SocketIOManager.SendContact(user, (int)user.TenantId);
                    }


                }
                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = sellingRequestDto.TenantId,
                    TypeId = (int)DashboardTypeEnum.Request,
                    StatusId = (int)SellingRequestStatus.Done,
                    StatusName = Enum.GetName(typeof(SellingRequestStatus), (int)SellingRequestStatus.Done)
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private SellingRequestEntity getSellingRequest(int tenantId, int pageNumber, int pageSize)
        {
            try
            {
                List<SellingRequestDto> lstSellingRequestDto = new List<SellingRequestDto>();
                var SP_Name = Constants.SellingRequest.SP_SellingRequestGet;
                int? sessionId = null;
                if (AbpSession.UserId != null)
                {
                    sessionId = (int)AbpSession.UserId;
                }
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                    ,new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                    ,new System.Data.SqlClient.SqlParameter("@UserId",sessionId)
                };


                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = System.Data.SqlDbType.BigInt,
                    ParameterName = "@TotalCount",
                    Direction = System.Data.ParameterDirection.Output
                };


                sqlParameters.Add(OutputParameter);


                lstSellingRequestDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToSellingRequestDto, AppSettingsModel.ConnectionStrings).ToList();

                SellingRequestEntity sellingRequestEntity = new SellingRequestEntity();
                sellingRequestEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                sellingRequestEntity.lstSellingRequestDto = lstSellingRequestDto;
                return sellingRequestEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private SellingRequestDto getSellingRequestById(long id, int tenantId)
        {
            try
            {
                SellingRequestDto sellingRequestDto = new SellingRequestDto();
                var SP_Name = Constants.SellingRequest.SP_SellingRequestByIDGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                   , new System.Data.SqlClient.SqlParameter("@Id",id)
                };

                sellingRequestDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertToSellingRequestDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return sellingRequestDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void updateSellingRequest(SellingRequestDto sellingRequestDto)
        {
            try
            {


                var SP_Name = Constants.SellingRequest.SP_SellingRequestUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@Id",sellingRequestDto.ContactId)

                    ,new System.Data.SqlClient.SqlParameter("@TenantId",sellingRequestDto.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@RequestDescription",sellingRequestDto.RequestDescription)
                    ,new System.Data.SqlClient.SqlParameter("@Price",sellingRequestDto.Price)
                    ,new System.Data.SqlClient.SqlParameter("@ContactInfo",sellingRequestDto.ContactInfo)
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedBy",sellingRequestDto.CreatedBy)
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedOn",sellingRequestDto.CreatedOn)
                    ,new System.Data.SqlClient.SqlParameter("@SellingRequestDetials",JsonConvert.SerializeObject( sellingRequestDto.lstSellingRequestDetailsDto))
                };





                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private CustomerChat UpdateCustomerChat(int? tenantId, SunshinePostMsgBotModel.Content model, string userId, string conversationID)
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
                status = 1,
                sender = Tenants.Contacts.MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                agentName = model.agentName,
                agentId = model.agentId,
                TenantId = tenantId,

            };
            var result = itemsCollection.CreateItemAsync(CustomerChat).Result;

            return CustomerChat;
        }
        private CustomerModel GetCustomerAzuer(string ContactID)
        {
            string result = string.Empty;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.ContactID == ContactID);
            if (customerResult.IsCompletedSuccessfully)
            {
                return customerResult.Result;

            }


            return null;
        }
        private CustomerModel GetCustomerByUserIDAzuer(string userId)
        {
            string result = string.Empty;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);
            if (customerResult.IsCompletedSuccessfully)
            {
                return customerResult.Result;

            }


            return null;
        }
        #endregion



    }
}
