using Infoseed.MessagingPortal.SendCampaing;
using Infoseed.MessagingPortal.TargetReach;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Sunshine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using Framework.Data;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Wallet;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Infoseed.MessagingPortal.WhatsApp;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using NPOI.HPSF;

namespace Infoseed.MessagingPortal.Integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendCampaignController : MessagingPortalControllerBase
    {

        IDocumentClient _documentClient;
        IDBService _dbService;
        ITargetReachAppService _ITargetReachAppService;
        private readonly TenantDashboardAppService _tenantDashboardAppService;
        private readonly IWalletAppService _walletAppService;
        private readonly IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;
        private readonly IDocumentClient _IDocumentClient;


        private string url = "https://startcampingstgnew.azurewebsites.net/api/startCampaign";
        //private string url = "https://startcampign.azurewebsites.net/api/startCampaign";

        public SendCampaignController(
            IDocumentClient documentClient,
            IDBService dbService,
            ITargetReachAppService iTargetReachAppService,
            TenantDashboardAppService tenantDashboardAppService,
            IWalletAppService walletAppService,
            IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService,
            IDocumentClient documentClient1
        )
        {
            _documentClient = documentClient;
            _dbService = dbService;
            _ITargetReachAppService = iTargetReachAppService;
            _tenantDashboardAppService = tenantDashboardAppService;
            _walletAppService = walletAppService;
            _whatsAppMessageTemplateAppService = whatsAppMessageTemplateAppService;
            _IDocumentClient = documentClient1;
        }
        [Route("DeleteContactChat")]
        [HttpGet]
        public  async Task DeleteContactChat(int TenantId, string UserId)
        {
            //var contact = await _contactRepository.FirstOrDefaultAsync((int)input);
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetAllItemsAsync(null,1000,a => a.ItemType == 0 && a.TenantId.Value==TenantId);
            var customer = customerResult.Result;

            foreach(var cus in customer.Item1)
            {
                // delete contact caht 
                var queryString = "SELECT * FROM c WHERE c.TenantId=" + TenantId.ToString() + " and c.userId='" + cus.userId + "' and  c.ItemType= 0";
                await itemsCollection.DeleteChatItem(queryString);
                //// delete teaminbox caht 
                //var queryString2 = "SELECT * FROM c WHERE c.ItemType= 1 and c.userId='" + customer.userId + "'";
                //await itemsCollection.DeleteChatItem(queryString2);


            }



        }
        [Route("SendMessage")]
        [HttpPost]
        public async Task<string> SendMessageAsync([FromBody] SendModel item,
            [FromHeader(Name = "tenancy")] string tenancy,
            [FromHeader(Name = "username")] string username,
            [FromHeader(Name = "password")] string password,
            [FromHeader(Name = "templateName")] string templateName

            )
        {

            

            if ( string.IsNullOrEmpty(tenancy))
            {
                return "incorrect password or username";

            }
            if (string.IsNullOrEmpty(username))
            {

                return "incorrect password or username";
            }
            if (string.IsNullOrEmpty(password))
            {
                return "incorrect password or username";

            }
            if (string.IsNullOrEmpty(templateName))
            {
                return "incorrect password or username";
            }

            var tenantModel = TenancyName(tenancy);
            try
            {
                if (tenantModel==null||tenantModel.TenantId==0)
                {
                    return "incorrect password or username";

                }
            }
            catch
            {
                return "incorrect password or username";
            }

            var UserModel=GetUserByName(username, tenantModel.TenantId);
            try
            {
                if (UserModel==null||UserModel.Id==0)
                {
                    return "incorrect password or username";

                }
            }
            catch
            {
                return "incorrect password or username";
            }

            //if (tenancy=="infoseed"&&item.ReciverPhoneNumber!="962798599163" && item.ReciverPhoneNumber!="962779746365" && item.ReciverPhoneNumber!="962788002429")
            //{

            //    return "incorrect Reciver PhoneNumber";
            //}


            int tenantId = tenantModel.TenantId;
            string user = UserModel.Name;
          //  templateName=templateName;
            try
            {
                #region Create Contact

                item.ReciverPhoneNumber=item.ReciverPhoneNumber.Replace("+", "");
                if (item.ReciverPhoneNumber != null)
                {
                    var NewContact = new Dictionary<string, dynamic>();

                    NewContact = await NewContactAsync(item.ReciverPhoneNumber, item.ReciverName, tenantId);
                    int Value = NewContact.Values.FirstOrDefault();
                    var messageError = NewContact.Values.LastOrDefault();
                    switch (Value)
                    {
                        case -1:
                            return messageError;
                        case 1:
                            return messageError;
                        case 2: break;
                        case 3:break;
                    }
                }
                
                
                
                //if (NewContact.CustomerOPT == 1)
                //{
                //    return new Dictionary<string, dynamic> { { "state", 10 }, { "message", "The Contact is Opt Out" } };
                //}
                #endregion

                #region ChecDailyLimit
                int DailyLimit = DailyLimitGet(tenantId);
                if (DailyLimit == 0)
                {
                    //response = new Dictionary<string, dynamic> { { "state", 5 }, { "message", "You have exceeded your daily limit" } };
                    return "You have exceeded your daily limit";
                }
                #endregion

                #region Wallet Information
                var walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                if (walletModel == null)
                {
                    _walletAppService.CreateWallet(tenantId);
                    walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                    //response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                    return "You don't have enough Funds";
                }
                else if (walletModel.TotalAmount <= 0)
                {
                    //response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                    return "You don't have enough Funds";
                }
                #endregion

                #region create name for new campign And Check if 
                var TemplatesInfo = GetTemplatesInfo(templateName, tenantId);

                long campaignId = 0;
                WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel();
                DateTime dateTime = DateTime.UtcNow;
                TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                whatsAppCampaignModel.Title = item.ReciverName + (long)timeSpan.TotalSeconds; ;
                whatsAppCampaignModel.Language = "en";
                whatsAppCampaignModel.TemplateId = TemplatesInfo.templatId;
                whatsAppCampaignModel.Type = 2;
                campaignId = addWhatsAppCampaign(whatsAppCampaignModel, tenantId, UserModel.Id);
                if (campaignId == 0)
                {
                    //response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The tenant is have an active campaign" } };
                    return "The tenant is have an active campaign";
                }
                #endregion

                #region check if you hava Funds gretar than or equal convarsaction prise 
                double Price = 0.014;
                if (walletModel.TotalAmount > 0)
                {
                    //UTILITY MARKETING
                    if (TemplatesInfo.category == "MARKETING" || TemplatesInfo.category == "UTILITY"|| TemplatesInfo.category == "AUTHENTICATION")
                    {
                        if (walletModel.TotalAmount >= (decimal)0.014)
                        {
                            walletModel.TotalAmount = walletModel.TotalAmount - (decimal)0.014;

                            WhatsAppContactsDto contacts = new WhatsAppContactsDto();
                            List<ListContactToCampin> OuterList = new List<ListContactToCampin>();
                            TemplateVariablles templateVariables = new TemplateVariablles
                            {
                                VarOne = item.MssageContent,
                                VarTwo = item.MssageContent2,
                                VarThree = item.MssageContent3,
                                VarFour = item.MssageContent4,
                                VarFive = item.MssageContent5,
                                VarSixteen = item.MssageContent16
                            };
                            OuterList.Add(new ListContactToCampin
                            {
                                Id = 0,
                                templateVariables = templateVariables,
                                ContactName = item.ReciverName,
                                PhoneNumber = item.ReciverPhoneNumber,
                                CustomerOPT = 0
                            });

                            long returnValue = 0;

                           
                            var tenantInfo = GetTenantInfo(tenantId);
                            // Display the count of each split list
                            string sendcompaing = "campaign1";

                            var SP_Name = Constants.WhatsAppCampaign.SP_SendCampaignAddOnDB;

                            MessageTemplateModel objWhatsAppTemplateModel = _whatsAppMessageTemplateAppService.GetTemplateById(TemplatesInfo.templatId);
                            MessageTemplateModel templateWA = getTemplateByWhatsId(tenantInfo, objWhatsAppTemplateModel.id).Result;

                            if (templateWA != null && templateWA.status == "APPROVED")
                            {
                                objWhatsAppTemplateModel.components = templateWA.components;
                                if (item.IsPDF)
                                {


                                    if (string.IsNullOrEmpty(item.LinkPDF))
                                    {

                                        return "LinkPDF Is Empty";
                                    }

                                    if (string.IsNullOrEmpty(item.FileNamePDF))
                                    {
                                        objWhatsAppTemplateModel.mediaLink = item.LinkPDF;

                                    }
                                    else
                                    {

                                        objWhatsAppTemplateModel.mediaLink = item.FileNamePDF + "," + item.LinkPDF;
                                    }


                                }
                                if (item.IsImage)
                                {

                                    if (string.IsNullOrEmpty(item.LinkImage))
                                    {

                                        return "image Is Empty";
                                    }
                                    objWhatsAppTemplateModel.mediaLink = item.LinkImage;

                                }
                                if (item.IsVideo)
                                {


                                    if (string.IsNullOrEmpty(item.LinkVideo))
                                    {

                                        return "LinkPDF Is Empty";
                                    }
                                    objWhatsAppTemplateModel.mediaLink = item.LinkVideo;

                                }
                                string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, out string type);

                                CampinQueueNew campinQueueNew = new CampinQueueNew();
                                var dates = DateTime.UtcNow.ToString();
                                string str = JsonConvert.SerializeObject(OuterList);
                                string TemplateJson = JsonConvert.SerializeObject(objWhatsAppTemplateModel);
                                string variable = JsonConvert.SerializeObject(templateVariables);



                                string userId = tenantId + "_" + item.ReciverPhoneNumber;

                                var isExternal = true;
                                try
                                {
                                    var CustomerCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                                    var customerResult = CustomerCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == tenantId);
                                    var Customer = customerResult.Result;
                                    if (Customer == null)
                                    {
                                        isExternal=true;

                                    }
                                    else
                                    {
                                        isExternal=false;
                                    }

                                }
                                catch
                                {

                                }
                              
                                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                                     new System.Data.SqlClient.SqlParameter("@Contacts",str)
                                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                                    ,new System.Data.SqlClient.SqlParameter("@TemplateId",TemplatesInfo.templatId)
                                    ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                                    ,new System.Data.SqlClient.SqlParameter("@CreatedDate",dates)
                                    ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",UserModel.Id) //userId
                                    ,new System.Data.SqlClient.SqlParameter("@IsExternalContact",isExternal)
                                    ,new System.Data.SqlClient.SqlParameter("@JopName",sendcompaing)
                                    ,new System.Data.SqlClient.SqlParameter("@TemplateName",objWhatsAppTemplateModel.name)
                                    ,new System.Data.SqlClient.SqlParameter("@CampaignName",whatsAppCampaignModel.Title)
                                    ,new System.Data.SqlClient.SqlParameter("@TemplateJson",TemplateJson)
                                    ,new System.Data.SqlClient.SqlParameter("@TemplateVariablesJson",variable)
                                    ,new System.Data.SqlClient.SqlParameter("@TemplateVariablesHeaderJson","")
                                    , new System.Data.SqlClient.SqlParameter("@URLButton1VariablesTemplate", "")
                                    ,new System.Data.SqlClient.SqlParameter("@URLButton2VariablesTemplate", "")
                                    ,new System.Data.SqlClient.SqlParameter("@carouselVariabllesTemplatejson", "")
                                };
                                var OutputParameter = new System.Data.SqlClient.SqlParameter
                                {
                                    SqlDbType = SqlDbType.BigInt,
                                    ParameterName = "@Id",
                                    Direction = ParameterDirection.Output
                                };
                                sqlParameters.Add(OutputParameter);

                                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                                if (OutputParameter.Value != DBNull.Value && OutputParameter.Value != null)
                                {

                                     
                                    campinQueueNew.messageTemplateModel = objWhatsAppTemplateModel;
                                    campinQueueNew.campaignId = campaignId;
                                    campinQueueNew.templateId = TemplatesInfo.templatId;
                                    campinQueueNew.IsExternal = isExternal;
                                    campinQueueNew.TenantId = tenantInfo.TenantId;
                                    campinQueueNew.D360Key = tenantInfo.D360Key;
                                    campinQueueNew.AccessToken = tenantInfo.AccessToken;
                                    campinQueueNew.functionName = sendcompaing;
                                    campinQueueNew.msg = msg;
                                    campinQueueNew.type = type;
                                    campinQueueNew.contacts = null;
                                    campinQueueNew.templateVariables = null;
                                    campinQueueNew.campaignName = whatsAppCampaignModel.Title;
                                    campinQueueNew.rowId = Convert.ToInt64(OutputParameter.Value);
                                    // SetCampinQueueContact(campinQueueNew);
                                    SetCampinInFun(campinQueueNew);
                                }
                                else
                                {
                                    return "An error occurred while Sending template";
                                }

                                #region Add in transaction table 

                                            TransactionModel transactionModel = new TransactionModel();

                                            //var usersDashModel = _tenantDashboardAppService.GetUserInfo((long)AbpSession.UserId.Value);
                                            transactionModel.DoneBy = user;
                                            transactionModel.TotalTransaction = (decimal)Price;
                                            transactionModel.TotalRemaining = walletModel.TotalAmount;
                                            transactionModel.TransactionDate = DateTime.UtcNow;
                                            transactionModel.CategoryType = objWhatsAppTemplateModel.category;
                                            transactionModel.TenantId = tenantInfo.TenantId;

                                            var result = addTransaction(transactionModel, OuterList.Count, objWhatsAppTemplateModel.name, campaignId);
                                            #endregion
                                whatsAppCampaignModel = new WhatsAppCampaignModel
                                {
                                    Id = campaignId,
                                    SentTime = DateTime.UtcNow,
                                    Status = 1 //Sent
                                };
                                _whatsAppMessageTemplateAppService.UpdateWhatsAppCampaign(whatsAppCampaignModel);

                                returnValue = 1;
                            }
                            else
                            {
                                returnValue = -10;
                            }
                                    

                            if (returnValue > 0)
                            {
                                //response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", "Sent Successfully" } };
                            }
                            else if (returnValue == -1)
                            {
                                long campinId = ChangeCampaignActive(tenantId, campaignId);

                                //response = new Dictionary<string, dynamic> { { "state", 4 }, { "message", "Invalid date" } };
                                return "Invalid date";
                            }
                            else if (returnValue == -10)
                            {
                                //response = new Dictionary<string, dynamic> { { "state", 9 }, { "message", "template Not APPROVED" } };
                                return "template Not APPROVED";
                            }
                            else
                            {
                                //response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                                return "You don't have enough Funds";
                            }
                        }
                        else
                        {
                            //response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                            return "You don't have enough Funds";
                        }
                    }
                }

                #endregion

                return campaignId.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #region private 
        /// <summary>
        /// get Daily Limit For same tenant
        /// </summary>
        /// <returns></returns>
        private int DailyLimitGet(int tenantId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_DailylimitGetCount;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@OutputParameter",
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
        /// <summary>
        /// get Category type for Templates
        /// </summary>
        /// <param name="templatesId">templates Id</param>
        /// <returns>return category type fpr template </returns>
        private TemplatesInfo GetTemplatesInfo(string templatesName, int tenantId)
        {
            try
            {
                TemplatesInfo templatesInfo = new TemplatesInfo();
                var SP_Name = Constants.WhatsAppTemplates.SP_IntegrationGetTemplatesInfo;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@templatesName",templatesName),
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                templatesInfo = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplatesIslamicInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return templatesInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long ChangeCampaignActive(int tenantId, long campaignId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_ChangeCampaignActive;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@RowsAffected",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                long result = (long)OutputParameter.Value;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private TenantInfoDto GetTenantInfo(int tenantId)
        {
            TenantInfoDto tenantInfoDto = new TenantInfoDto();
            try
            {
                var SP_Name = Constants.Tenant.SP_TenantInfoGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                tenantInfoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGetTenantInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return tenantInfoDto;
            }
            catch
            {
                return tenantInfoDto;
            }
        }
        private TenantInfoDto TenancyName(string tenancyName)
        {
            TenantInfoDto tenantInfoDto = new TenantInfoDto();
            try
            {
                var SP_Name = Constants.Tenant.SP_TenancyName;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenancyName",tenancyName)
                };

                tenantInfoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGetTenantInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return tenantInfoDto;
            }
            catch
            {
                return tenantInfoDto;
            }
        }
        private UsersDashModel GetUserByName(string UserName,int tenantId)
        {
            UsersDashModel UserInfoDto = new UsersDashModel();
            try
            {
                var SP_Name = Constants.User.SP_GetUserByName;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                     new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };

                UserInfoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapUserInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return UserInfoDto;
            }
            catch
            {
                return UserInfoDto;
            }
        }
        private static async Task<MessageTemplateModel> getTemplateByWhatsId(TenantInfoDto tenant, string templateId)
        {
            using (var httpClient = new HttpClient())
            {
                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + templateId;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                using (var response = await httpClient.GetAsync(postUrl))
                {
                    using (var content = response.Content)
                    {
                        try
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var WhatsAppTemplate = await content.ReadAsStringAsync();
                                MessageTemplateModel objWhatsAppTemplate = new MessageTemplateModel();
                                objWhatsAppTemplate = JsonConvert.DeserializeObject<MessageTemplateModel>(WhatsAppTemplate);
                                return objWhatsAppTemplate;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }
        private string prepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, out string type)
        {
            try
            {
                string result = string.Empty;
                type = "text";
                if (objWhatsAppTemplateModel.components != null)
                {
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type.Equals("HEADER"))
                        {
                            type = item.format.ToLower();
                        }
                        if (item.type.Equals("BUTTONS"))
                        {
                            for (int i = 0; i < item.buttons.Count; i++)
                            {
                                result = result + "\n\r" + (i + 1) + "-" + item.buttons[i].text;
                            }
                        }
                        result += item.text;

                    }

                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task SetCampinInFun(CampinQueueNew obj)
        {


            //CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

            try
            {
                List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);

                if (campaigns.Count()>0)
                {

                    WhatsAppCampaignModel whatsAppCampaignModel2 = new WhatsAppCampaignModel
                    {
                        Id = obj.campaignId,
                        SentTime = DateTime.UtcNow,
                        Status = 3 // as sent
                    };
                    updateWhatsAppCampaign2(whatsAppCampaignModel2, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());


                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.urlSendCampaignProject);
                    var content = new StringContent("{\n    \"campaignId\": "+obj.campaignId+"\n}", null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();


                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //WhatsAppCampaignModel whatsAppCampaignModel23 = new WhatsAppCampaignModel
                        //{
                        //    id = obj.campaignId,
                        //    sentTime = DateTime.UtcNow,
                        //    status = 3 // as sent
                        //};
                        //updateWhatsAppCampaign(whatsAppCampaignModel23, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());
                    }
                    else
                    {
                        WhatsAppCampaignModel whatsAppCampaignModel23 = new WhatsAppCampaignModel
                        {
                            Id = obj.campaignId,
                            SentTime = DateTime.UtcNow,
                            Status = 4 // as sent
                        };
                        updateWhatsAppCampaign2(whatsAppCampaignModel23, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());

                    }


                }









            }
            catch (Exception ex)
            {


            }
        }

        private static List<SendCampaignNow> GetCampaign(long rowId)
        {
            try
            {
                var SP_Name = "[dbo].[SendCampaignGetFromDB]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                };
                List<SendCampaignNow> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, AppSettingsModel.ConnectionStrings).ToList();
                return model;
            }
            catch
            {
                return new List<SendCampaignNow>();
            }
        }
        public static SendCampaignNow MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                SendCampaignNow model = new SendCampaignNow
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
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent")
                };

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactToCampin>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new SendCampaignNow();
            }
        }
        private static void updateWhatsAppCampaign2(WhatsAppCampaignModel whatsAppCampaignModel, int TenantId, int count=0)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",whatsAppCampaignModel.Id)
                    ,new System.Data.SqlClient.SqlParameter("@SentTime",whatsAppCampaignModel.SentTime)
                    ,new System.Data.SqlClient.SqlParameter("@Status",whatsAppCampaignModel.Status)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Count",count)
                    ,new System.Data.SqlClient.SqlParameter("@SentCampaignId",Guid.NewGuid())
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetCampinQueueContact(CampinQueueNew campinQueueNew)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
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
        private long addTransaction(TransactionModel model, int totalCount, string TemplateName, long campaignId)
        {
            try
            {
                var SP_Name = Constants.Transaction.SP_CampaignMinusPrice;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@DoneBy",model.DoneBy)
                    ,new System.Data.SqlClient.SqlParameter("@TotalTransaction",model.TotalTransaction)
                    ,new System.Data.SqlClient.SqlParameter("@TotalRemaining",model.TotalRemaining)
                    ,new System.Data.SqlClient.SqlParameter("@TransactionDate",model.TransactionDate)
                    ,new System.Data.SqlClient.SqlParameter("@CategoryType",model.CategoryType)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",model.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@totalCount",totalCount)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateName",TemplateName)
                    ,new System.Data.SqlClient.SqlParameter("@campaignId",campaignId)
                  };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Output",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt64(OutputParameter.Value) : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal async Task<Dictionary<string, dynamic>> NewContactAsync(string phoneNumber, string contactName, int tenantId)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();
                    
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);

                    string userId = tenant.TenantId + "_" + phoneNumber;
                    var CustomerCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var customerResult = CustomerCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == tenantId);
                    var Customer = customerResult.Result;

                    if (Customer == null )
                    {
                        //type = text
                        ContactsTeamInboxs contactsTeamInboxs = new ContactsTeamInboxs();
                        var customerModel = _dbService.CreateNewCustomer(phoneNumber, contactName, "text", tenant.botId, tenantId, tenant.D360Key);

                        if (customerModel != null)
                        {
                            contactsTeamInboxs.contactId = int.Parse(customerModel.ContactID);
                            contactsTeamInboxs.displayName = customerModel.displayName;
                            contactsTeamInboxs.phoneNumber = customerModel.phoneNumber;
                            contactsTeamInboxs.userId = customerModel.userId;

                            if (customerModel.displayName.Length != 0)
                            {
                                contactsTeamInboxs.combinedValue = customerModel.phoneNumber + " (" + customerModel.displayName + ")";
                            }
                            else
                            {
                                contactsTeamInboxs.combinedValue = customerModel.phoneNumber;
                            }

                            response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", contactsTeamInboxs } };
                        }
                        else
                        {
                            // Handle the case when customerModel is null
                            response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "Contact Create failed." } };
                        }
                    }
                    else
                    {
                        response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The number already exists" } };
                    }

                    return response;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        private long addWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel,int tenantId ,long userId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignAdd;
                var date = DateTime.UtcNow.ToString();

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                    new System.Data.SqlClient.SqlParameter("@CampaignTilte",whatsAppCampaignModel.Title)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignLanguage",whatsAppCampaignModel.Language)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateId",whatsAppCampaignModel.TemplateId)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignType",1)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",userId) // userId
                    ,new System.Data.SqlClient.SqlParameter("@CreatedDate",date)
                    ,new System.Data.SqlClient.SqlParameter("@Type",whatsAppCampaignModel.Type)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@CampaignId";
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
        #endregion
    }
}
