using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Genders.Exporting;
using Infoseed.MessagingPortal.Genders;
using System;
using System.Collections.Generic;
using System.Text;
using Infoseed.MessagingPortal.Facebook_Template.Dtos;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using System.Web;
using System.Data;
using Framework.Data;
using Infoseed.MessagingPortal.MultiTenancy;
using Microsoft.Azure.Documents;
using Elasticsearch.Net;

namespace Infoseed.MessagingPortal.Facebook_Template
{

    public class FacebookTemplateAppService : MessagingPortalAppServiceBase, IFacebookTemplateAppService
    {
        private readonly IRepository<Gender, long> _genderRepository;
        private readonly IGendersExcelExporter _gendersExcelExporter;
        private readonly IDocumentClient _IDocumentClient;





        public FacebookTemplateAppService(IRepository<Gender, long> genderRepository, IGendersExcelExporter gendersExcelExporter)
        {
            _genderRepository = genderRepository;
            _gendersExcelExporter = gendersExcelExporter;

        }




        public Task<FacebookTemplateDto> DeleteFacebookMessageTemplateAsync(string templateName)
        {
            throw new NotImplementedException();
        }

        public FacebookTemplateDto GetAll(long id)
        {
            throw new NotImplementedException();
        }


        public async Task<WhatsAppTemplateResultModel> AddWhatsAppMessageTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null)
        {
            try
            {
                tenantId ??= AbpSession.TenantId.Value;
                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);
                WhatsAppComponentModel componentBody = messageTemplateModel.components.Where(x => x.type == "BODY").FirstOrDefault();
                //componentBody.text = HtmlToPlainText(componentBody.text);

                var httpClient = new HttpClient();
                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.WhatsAppAccountID + "/message_templates?name=" + messageTemplateModel.name + "&language=" + messageTemplateModel.language + "&category=" + messageTemplateModel.category + "&components=" + JsonConvert.SerializeObject(messageTemplateModel.components, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                var response = await httpClient.PostAsync(postUrl, new StringContent("", Encoding.UTF8, "application/json"));
                var content = response.Content;
                var result = await content.ReadAsStringAsync();
                WhatsAppTemplateResultModel objWhatsAppTemplateResultModel = new WhatsAppTemplateResultModel();
                objWhatsAppTemplateResultModel = JsonConvert.DeserializeObject<WhatsAppTemplateResultModel>(result);
                messageTemplateModel.id = objWhatsAppTemplateResultModel.Id;
                if (objWhatsAppTemplateResultModel.Id != null)
                {
                    messageTemplateModel.TenantId = tenantId.Value;
                    addWhatsAppMessageTemplate(messageTemplateModel);
                }
                return objWhatsAppTemplateResultModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private long addWhatsAppMessageTemplate(MessageTemplateModel messageTemplateModel)
        {
            try
            {
                if (messageTemplateModel.TenantId == 0)
                {
                    messageTemplateModel.TenantId = AbpSession.TenantId.Value;
                }
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesAdd;
                if (messageTemplateModel.TenantId != 0)
                {


                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                         new System.Data.SqlClient.SqlParameter("@TemplateName",messageTemplateModel.name)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateLanguage",messageTemplateModel.language)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateComponent",JsonConvert.SerializeObject(messageTemplateModel.components))
                        ,new System.Data.SqlClient.SqlParameter("@TemplateStatus",messageTemplateModel.status)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateCategory",messageTemplateModel.category)
                        ,new System.Data.SqlClient.SqlParameter("@WhatsAppTemplateId",messageTemplateModel.id)
                        ,new System.Data.SqlClient.SqlParameter("@MediaType",messageTemplateModel.mediaType)
                        ,new System.Data.SqlClient.SqlParameter("@MediaLink",messageTemplateModel.mediaLink)
                        ,new System.Data.SqlClient.SqlParameter("@TenantId",messageTemplateModel.TenantId)
                        ,new System.Data.SqlClient.SqlParameter("@VariableCount",messageTemplateModel.VariableCount)
                        ,new System.Data.SqlClient.SqlParameter("@BtnOneActionId",messageTemplateModel.BtnOneActionId)
                        ,new System.Data.SqlClient.SqlParameter("@BtnTwoActionId",messageTemplateModel.BtnTwoActionId)
                        ,new System.Data.SqlClient.SqlParameter("@BtnThreeActionId",messageTemplateModel.BtnThreeActionId)
                    };

                    var OutputParameter = new System.Data.SqlClient.SqlParameter();
                    OutputParameter.SqlDbType = SqlDbType.BigInt;
                    OutputParameter.ParameterName = "@TemplateId";
                    OutputParameter.Direction = ParameterDirection.Output;
                    sqlParameters.Add(OutputParameter);

                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                    return (long)OutputParameter.Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Task<FacebookTemplateDto> AddFacebookMessageTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageTemplateModel> GetTemplateFacebookById( string templateId)
        {
            var tenantInfo = GetTenantInfo(AbpSession.TenantId.Value);
            var httpClient = new HttpClient();
            var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + templateId;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenantInfo.AccessToken);

            var response = await httpClient.GetAsync(postUrl);
            var content = response.Content;
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
                    return new MessageTemplateModel();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<FacebookTemplateDto> UpdateTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null)
        {
            throw new NotImplementedException();
        }

        //public async Task<string> GetCatTest()
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://www.facebook.com/api/graphql/");
        //    request.Headers.Add("accept", "*/*");
        //    request.Headers.Add("accept-language", "en-US,en;q=0.9");
        //    request.Headers.Add("cache-control", "no-cache");
        //    request.Headers.Add("content-type", "application/x-www-form-urlencoded");
        //    request.Headers.Add("Cookie", "c_user=100016599555039; fr=1GEDxpGxvIeQVZGoS.AWXFeOTqZhjTldcsyNzOFDyf5lE.BnanDN..AAA.0.0.BnanDN.AWVR5tEqh_M; xs=38%3AR4ERAQvoOsezOw%3A2%3A1734334892%3A-1%3A6704%3AAAcVEbIATBmt0bXEXv-korNTpjmnHCCypF5bYOqrjeE4%3AAcWmrziJrBX0pRno331CtA0nsC3H4ezOuJ-YRi84Y70");
        //    var collection = new List<KeyValuePair<string, string>>();
        //    collection.Add(new("fb_dtsg", "NAcPDs6WmwF3hi0mTcTr4r77t1-5yaTwG527AYmR6h7LURI3U1gQ_lg:38:1734334892"));
        //    collection.Add(new("variables", "{\"count\":5,\"ownerTypes\":[\"BUSINESS\"],\"businessIDs\":[\"411493854124733\"],\"capabilities\":null,\"excludedCapabilities\":[]}"));
        //    collection.Add(new("doc_id", "27896879179926653"));
        //    var content = new FormUrlEncodedContent(collection);
        //    request.Content = content;
        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();
        //    Console.WriteLine(await response.Content.ReadAsStringAsync());
        //    return "";

        //}


        #region privet
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
        public static TenantInfoDto MapGetTenantInfo(IDataReader dataReader)
        {
            TenantInfoDto model = new TenantInfoDto();
            model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id");
            model.AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken");
            model.D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key");

            return model;
        }

        #endregion
    }
}
