using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Runtime.Caching;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using Framework.Data;
using Infoseed.MessagingPortal.BotFlow.Dtos;
using Infoseed.MessagingPortal.BranchAreas;
using Infoseed.MessagingPortal.Branches;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.WhatsApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Attachment = Microsoft.Bot.Connector.DirectLine.Attachment;

namespace Infoseed.MessagingPortal.BotFlow
{
    public class BotFlowAppService : MessagingPortalAppServiceBase, IBotFlowAppService
    {
        private readonly IDocumentClient _IDocumentClient;
        public IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;

        public BotFlowAppService(IDocumentClient IDocumentClient, IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService)
        {
            _IDocumentClient = IDocumentClient;
            _whatsAppMessageTemplateAppService= whatsAppMessageTemplateAppService;

        }
        #region public

        public long CreateBotFlows(GetBotModelFlowForViewDto mdeol)
        {
            return createBotFlows(mdeol);
        }
        public long DeleteBotFlows(long Id)
        {
            return deleteBotFlows(Id);
        }
        public Task<PagedResultDto<GetBotModelFlowForViewDto>> GetAllBotFlows(int TenantId, int? PageNumber=0, int? PageSize = 0)
        {
           return getAllBotFlows(TenantId, PageNumber.Value, PageSize.Value);
        }
        public Task<long> GetBotFlowsById(long id)
        {
            return getBotFlowsById(id);
        }
        public GetBotModelFlowForViewDto GetByIdBotFlows(long Id)
        {
            return getByIdBotFlows(Id);
        }

        public long UpdateBotFlowsIsPublished(long Id, long modifiedUserId, string modifiedUserName , bool isPublished, string BotChannel = "whatsapp")
        {
           return updateBotFlowsIsPublished(Id, modifiedUserId, modifiedUserName, isPublished, BotChannel);
        }
        public long UpdateBotFlowsJsonModel(long Id, GetBotModelFlowForViewDto mdeol)
        {
            return updateBotFlowsJsonModel(Id , mdeol);
        }
     
        public List<Activity> TestBotStart(int TenantId, string text, int FlowId)
        {
            var model = GetBotTest(TenantId);//Get  Customer
            if (model==null)
            {



                model= CreateNewBotTest(TenantId);
            }
            model.text=text.Trim();
            model.IdFlow=FlowId;
            return TestBotMessageHandler(model);

        }
  
        public List<Activity> TestBotMessageHandler(GetTestBotFlowDto model)
        {
            List<Activity> Bot = new List<Activity>();


            BotTestFlowAsync(model, model.BotTestStepModel, Bot);

            UpdateBotTest(model);//update  Customer

            return Bot;
        }

        public long BotParameterCreate(BotParameterModel mdeol)
        {
            return botParameterCreate(mdeol);
        }

        public long BotParameterDeleteById(long Id)
        {
            return botParameterDeleteById(Id);
        }

        public Task<PagedResultDto<BotParameterModel>> BotParameterGetAll(int TenantId)
        {
            return botParameterGetAll(TenantId);
        }


        public void UpdateBotReStart(int TenantId)
        {
            try
            {
                var itemsCollection = new DocumentCosmoseDB<GetTestBotFlowDto>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 4  && a.TenantId == TenantId).Result;//&& a.TenantId== TenantId

                    customerResult.IsReStart=true;
                
                var Result = itemsCollection.UpdateItemAsync(customerResult._self, customerResult).Result;
            }
            catch
            {

            }


        }
        #endregion


        #region private

        private long createBotFlows(GetBotModelFlowForViewDto mdeol)
        {
            try
            {
                var SP_Name = Constants.BotFlows.SP_BotFlowsCreate;

                string jsonModelValue="";

                jsonModelValue = JsonConvert.SerializeObject(mdeol.getBotFlowForViewDto);

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId", mdeol.TenantId),
                    new System.Data.SqlClient.SqlParameter("@JsonModel", jsonModelValue),
                    new System.Data.SqlClient.SqlParameter("@CreatedDate", DateTime.UtcNow),
                    new System.Data.SqlClient.SqlParameter("@CreatedUserId", mdeol.CreatedUserId),
                    new System.Data.SqlClient.SqlParameter("@CreatedUserName", mdeol.CreatedUserName),
                    new System.Data.SqlClient.SqlParameter("@FlowName", mdeol.FlowName),
                    new System.Data.SqlClient.SqlParameter("@StatusId", mdeol.StatusId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@BotFlowsId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "")
                {
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

        private long deleteBotFlows(long id)
        {
            try
            {
                var SP_Name = Constants.BotFlows.SP_BotFlowsDelete;
                int TenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Id",id)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "")
                {
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

        private async Task<PagedResultDto<GetBotModelFlowForViewDto>> getAllBotFlows(int TenantId ,int pagenumber=0,int pagesize=50)
        {
            try
            {
                List<GetBotModelFlowForViewDto> getBotModelFlowForViewDto = new List<GetBotModelFlowForViewDto>();

                int totalCountOut = 0;
                var SP_Name = Constants.BotFlows.SP_BotFlowsGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId),
                    new System.Data.SqlClient.SqlParameter("@PageNumber",pagenumber),
                    new System.Data.SqlClient.SqlParameter("@PageSize",pagesize)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                getBotModelFlowForViewDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertBotFlowsDto, AppSettingsModel.ConnectionStrings).ToList();
                totalCountOut = (int)OutputParameter.Value;

                return  new PagedResultDto<GetBotModelFlowForViewDto>(totalCountOut, getBotModelFlowForViewDto); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<long> getBotFlowsById(long id)
        {
            try
            {
                var SP_Name = Constants.BotFlows.SP_BotFlowsGetByTempId;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Id",id)
                };
               
               long  getBotModelFlowForViewDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertBotFlowsByTempIdDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();
              

                return getBotModelFlowForViewDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private GetBotModelFlowForViewDto getByIdBotFlows(long Id)
        {
            try
            {
                GetBotModelFlowForViewDto getBotModelFlowForViewDto = new GetBotModelFlowForViewDto();
                
                var SP_Name = Constants.BotFlows.SP_BotFlowsGetById;
                int TenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Id",Id)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };

                getBotModelFlowForViewDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertBotFlowsDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return getBotModelFlowForViewDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private long updateBotFlowsIsPublished(long id,long modifiedUserId,string modifiedUserName ,bool isPublished,string BotChannel="whatsapp")
        {
            try
            {
                var SP_Name = Constants.BotFlows.SP_BotFlowsUpdateIsPublished;
                int TenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Id",id)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedDate",DateTime.UtcNow)
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedUserId",modifiedUserId)
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedUserName",modifiedUserName)
                    ,new System.Data.SqlClient.SqlParameter("@BotChannel",BotChannel)
                    ,new System.Data.SqlClient.SqlParameter("@IsPublished",isPublished)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@BotFlowsId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "")
                {
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

        private long updateBotFlowsJsonModel(long id, GetBotModelFlowForViewDto mdeol)
        {
            try
            {
                var SP_Name = Constants.BotFlows.SP_BotFlowUpdateJson;
                int TenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Id",id)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@FlowName",mdeol.FlowName)
                    ,new System.Data.SqlClient.SqlParameter("@JsonModel",JsonConvert.SerializeObject(mdeol.getBotFlowForViewDto))
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedDate",DateTime.UtcNow)
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedUserId",mdeol.ModifiedUserId)
                    ,new System.Data.SqlClient.SqlParameter("@ModifiedUserName",mdeol.ModifiedUserName)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@BotFlowsId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "")
                {
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

        private  void BotTestFlowAsync(GetTestBotFlowDto model, BotTestStepModel botStepModel, List<Activity> Bot)
        {
            var botflowModels = GetAllBotFlows(model.TenantId).Result.Items.ToList().Where(x => x.Id==model.IdFlow).FirstOrDefault();
            if (model.text.ToLower().Trim()=="clean")
            {
                model.text="#";
                try
                {
                    model.BotTestStepModel.UserParmeter=new Dictionary<string, string>();
                }
                catch
                {

                }
               
            }

            model.BotTestStepModel.LangId=2;
            model.BotTestStepModel.LangString="en";

            var flow = botflowModels.getBotFlowForViewDto.ToList();

            if (model.text.Trim()=="#" || model.IsReStart)
            {
                model.IsReStart=false;
                var firstitem = flow.Where(x => x.isNodeRoot.Value).FirstOrDefault();
                if (firstitem!=null)
                {

                    if (botflowModels.StatusId==4)
                    {


                        if (firstitem.childId==null)
                        {
                            model.BotTestStepModel.ChatStepId= firstitem.content.dtoContent.FirstOrDefault().childId.Value;


                        }
                        else
                        {
                            model.BotTestStepModel.ChatStepId= firstitem.childId.Value;


                        }


                    }
                    else
                    {
                        model.BotTestStepModel.ChatStepId= firstitem.id.Value;


                    }
                    model.BotTestStepModel.ChatStepPervoiusId= firstitem.parentIndex[0];
                    model.getBotFlowForViewDto=null;
                    //model.BotTestStepModel.UserParmeter=new Dictionary<string, string>();

                }

            }



            //check the user send 

            var nextstepId = 0;
            var PervoiusstepId = 0;
            if (model.getBotFlowForViewDto!=null)
            {

                if (!string.IsNullOrEmpty(model.getBotFlowForViewDto.parameter))
                {


                    if (model.getBotFlowForViewDto.type=="ScheduleNode" ||model.getBotFlowForViewDto.type=="Branches" ||  model.getBotFlowForViewDto.type == "BranchesEN" || model.getBotFlowForViewDto.type=="Reply buttons" || model.getBotFlowForViewDto.type=="List options"|| model.getBotFlowForViewDto.type=="Language")
                    {


                        int intValue;
                        if (int.TryParse(model.text.Trim(), out intValue) && intValue<=model.getBotFlowForViewDto.content.dtoContent.Length)
                        {

                            var xx = model.getBotFlowForViewDto.content.dtoContent[intValue-1];
                            // String is a valid integer
                            model.BotTestStepModel.ChatStepId=xx.childId.Value;
                            try
                            {
                                model.BotTestStepModel.ChatStepPervoiusId=xx.parentIndex[0];

                            }
                            catch
                            {

                            }

                            model.text=xx.valueEn;
                            //TODO : add parm to contact 

                            model.BotTestStepModel.UserParmeter.Remove(model.getBotFlowForViewDto.parameter);

                            if (model.getBotFlowForViewDto.type=="Branches" || model.getBotFlowForViewDto.type == "BranchesEN")
                            {
                                model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, xx.branchID);

                                //model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                            }
                            else
                            {
                                model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                            }


                        }
                        else
                        {
                            var istrue = false;
                            foreach (var xx in model.getBotFlowForViewDto.content.dtoContent)
                            {
                                if (xx.valueEn.Trim() ==model.text.Trim() || xx.valueAr.Trim() ==model.text.Trim())
                                {
                                    if (model.getBotFlowForViewDto.type=="Language")
                                    {

                                        if (xx.valueEn.Trim()=="العربية"||xx.valueAr.Trim()=="العربية")
                                        {
                                            model.BotTestStepModel.LangId=1;
                                            model.BotTestStepModel.LangString="ar";
                                        }

                                        if (xx.valueEn.Trim()=="English"||xx.valueAr.Trim()=="English")
                                        {
                                            model.BotTestStepModel.LangId=2;
                                            model.BotTestStepModel.LangString="en";
                                        }




                                    }
                                    if (xx.childId == null)
                                    {
                                        model.BotTestStepModel.ChatStepId = -1;
                                        model.BotTestStepModel.ChatStepPervoiusId = 0;
                                    }
                                    else
                                    {
                                        model.BotTestStepModel.ChatStepId = xx.childId.Value;
                                        try
                                        {
                                            model.BotTestStepModel.ChatStepPervoiusId = xx.parentIndex[0];

                                        }
                                        catch
                                        {

                                        }
                                    }

                                    //TODO : add parm to contact 
                                    istrue =true;

                                    model.BotTestStepModel.UserParmeter.Remove(model.getBotFlowForViewDto.parameter);

                                    if (model.getBotFlowForViewDto.type=="Branches" || model.getBotFlowForViewDto.type == "BranchesEN")
                                    {


                                        var branchID = model.getBotFlowForViewDto.content.dtoContent.Where(x => x.valueAr.Trim() == model.text.Trim() || x.valueEn.Trim() == model.text.Trim()).FirstOrDefault().branchID;
                                        model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, branchID);

                                        //model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                                    }
                                    else
                                    {
                                        model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                                    }
                                }

                            }
                            if (!istrue)
                            {
                                model.BotTestStepModel.ChatStepId=model.getBotFlowForViewDto.id.Value;
                                model.BotTestStepModel.ChatStepPervoiusId=model.getBotFlowForViewDto.parentIndex[0];

                            }

                        }




                      



                    }
                    else
                    {
                        //model.BotTestStepModel.ChatStepId=model.getBotFlowForViewDto.childIndex.Value;
                        //model.BotTestStepModel.ChatStepPervoiusId=model.getBotFlowForViewDto.parentIndex[0];
                        //TODO : add parm to contact 
                        model.BotTestStepModel.UserParmeter.Remove(model.getBotFlowForViewDto.parameter);
                        if (model.getBotFlowForViewDto.type=="Branches" || model.getBotFlowForViewDto.type == "BranchesEN")
                        {
                            model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.getBotFlowForViewDto.content.dtoContent.Where(x=>x.valueAr==model.text.Trim() || x.valueEn==model.text.Trim()).FirstOrDefault().branchID);

                            //model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                        }
                        else
                        {

                            var type = "Text";
                            try
                            {
                                type= botParameterGetAll(model.TenantId).Result.Items.Where(x => x.Name==model.getBotFlowForViewDto.parameter).First().Format;

                            }
                            catch
                            {


                            }

                            if (type=="Number")
                            {
                                //checked
                                bool isNumber = double.TryParse(model.text.Trim(), out double resultdouble);
                                bool isInteger = int.TryParse(model.text.Trim(), out int resultint);

                                if (isNumber || isInteger)
                                {
                                    model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());


                                }
                                else
                                {
                                    model.BotTestStepModel.ChatStepId=model.getBotFlowForViewDto.id.Value;
                                    model.BotTestStepModel.ChatStepPervoiusId=model.getBotFlowForViewDto.parentIndex[0];

                                }

                            }
                            else
                            {
                                model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                            }
                            
                            
                        

                        }

                    }


                    model.getBotFlowForViewDto=null;
                }







            }

            nextstepId = 0;
            PervoiusstepId = 0;
            bool isButton = false;
            bool iskeyword=false;

            var count =0;

            while (!isButton)
            {
                if (count==20)
                {

                    break;

                }

                if (!iskeyword)
                {      //key word 
                    var keyword = _whatsAppMessageTemplateAppService.KeyWordGetByKey(model.TenantId, model.text.ToLower().Trim());
                    if (keyword!=null && botflowModels.FlowName==keyword.action)
                    {
                        model.BotTestStepModel.ChatStepId=int.Parse(keyword.triggersBotId.ToString());
                        iskeyword=true;
                    }

                }

                var fw = flow.Where(x => x.id==model.BotTestStepModel.ChatStepId).FirstOrDefault();
                if (fw==null)
                {
                    break;
                }

                //one time 
                if (fw.isOneTimeQuestion)
                {
                    try
                    {
                        if (model.BotTestStepModel.UserParmeter.ContainsKey(fw.parameter))
                        {

                            var x = model.BotTestStepModel.UserParmeter[fw.parameter];
                            if (!string.IsNullOrEmpty(x))
                            {
                                try
                                {
                                    if (fw.content!=null)
                                    {
                                        model.BotTestStepModel.ChatStepId=fw.content.dtoContent.FirstOrDefault().childId.Value;

                                    }
                                    else
                                    {

                                        model.BotTestStepModel.ChatStepId=fw.childId.Value;
                                    }

                                }
                                catch
                                {
                                    if (fw.childId!=null)
                                    {
                                        model.BotTestStepModel.ChatStepId=fw.childId.Value;

                                    }
                                    else
                                    {
                                        break;

                                    }
                                }

                                continue;
                            }



                        }
                       
                    }
                    catch
                    {
                        model.BotTestStepModel.ChatStepId=fw.childId.Value;
                        continue;
                    }


                }



                TypeMassgeFun(model, Bot,  ref nextstepId, ref PervoiusstepId, ref isButton, fw);
                if (isButton)
                {
                    break;
                }

                count++;

            }
        }
        private  void TypeMassgeFun(GetTestBotFlowDto botStepModel, List<Activity> Bot, ref int nextstepId, ref int PervoiusstepId, ref bool isButton, GetBotFlowForViewDto fw)
        {
            if (fw!=null)
            {
                // Define the regular expression pattern
                string pattern = @"\$(\w+)";

                if (fw.type=="whatsApp template")
                {
                    botStepModel.BotTestStepModel.ChatStepId=fw.id.Value;
                    botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                }

                if (fw.type=="Send Message")
                {
                    var Summary = "";
                    var text = "";
                    if (botStepModel.BotTestStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary =fw.footerTextAr;// "الرجاء ارسال # للعودة للقائمة الرئسية";
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary =fw.footerTextEn;// "Please send # to return to the main menu";
                    }




                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {

                        string result = match.Value;

                        result=result.Replace(")", "");
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            try
                            {
                                var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                                text= text.Replace(result, diVAlue);

                            }
                            catch(Exception ex)
                            {


                            }
                    

                        }

                    }

                    text=text.Replace("$", "");

                    if (text.Contains("IMG("))
                    {

                        text=text.Replace("IMG(", "").Replace(")", "");
                        fw.urlImage.url=text;
                    }


                    List<Activity> activities = new List<Activity>();
                    Activity Botf = new Activity();


                    if (fw.urlImageArray!=null && fw.urlImageArray.Length>0)
                    {
                        foreach (var activity in fw.urlImageArray)
                        {
                            string img = activity.url;
                            string fileType = GetFileTypeFromUrl(img);

                            Botf = new Activity
                            {
                                From = new ChannelAccount("27", "27_27"),
                                Text = img,
                                Speak= activity.name,
                                Type = ActivityTypes.Message,
                                InputHint=fileType,
                                Summary = Summary,
                                Locale =   botStepModel.BotTestStepModel.LangString,
                                Attachments = null
                            };

                            Bot.Add(Botf);

                        }

                    }
                    else
                    {

                        if (fw.urlImage!=null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            string img = fw.urlImage.url;
                            string fileType = GetFileTypeFromUrl(img);

                            Botf = new Activity
                            {
                                From = new ChannelAccount("27", "27_27"),
                                Text = img,
                                Speak= fw.urlImage.name,
                                Type = ActivityTypes.Message,
                                InputHint=fileType,
                                Summary = Summary,
                                Locale =   botStepModel.BotTestStepModel.LangString,
                                Attachments = null
                            };

                            Bot.Add(Botf);
                        }
                        else
                        {
                            Botf = new Activity
                            {
                                From = new ChannelAccount("27", "27_27"),
                                Text = text,
                                Speak= text,
                                Type = ActivityTypes.Message,
                                Summary = Summary,
                                Locale =   botStepModel.BotTestStepModel.LangString,
                                Attachments = null
                            };

                            Bot.Add(Botf);
                        }
                    }





                    try
                    {
                        botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.BotTestStepModel.ChatStepId=-1;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                    }




                }
                if (fw.type=="Reply buttons")
                {
                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();


                    string img = null;


                    if (fw.urlImageArray!=null && fw.urlImageArray.Length>0)
                    {

                        img=fw.urlImageArray.FirstOrDefault().url;



                    }
                    else
                    {

                        if (fw.urlImage!=null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            img=fw.urlImage.url;

                        }



                    }

                    foreach (var bt in fw.content.dtoContent)
                    {

                        if (botStepModel.BotTestStepModel.LangString=="ar")
                        {
                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueAr,
                                Value=bt.valueAr,
                                Image=img
                            });
                            Buttons.Add(bt.valueAr);
                        }
                        else
                        {

                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueEn,
                                Value=bt.valueEn,
                                Image=img
                            });
                            Buttons.Add(bt.valueEn);
                        }


                    }
                    var Summary = "";
                    var text = "";
                    if (botStepModel.BotTestStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                    }

                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                           // botStepModel.BotTestStepModel.UserParmeter.Remove(word);
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                            text= text.Replace(result, diVAlue);

                        }

                    }

                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount("27", "27_27"),
                        Text = text,
                        Speak= text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =  botStepModel.BotTestStepModel.LangString,
                        Attachments = null
                    };


                    //isButton=true;
                    botStepModel.BotTestStepModel.IsButton=true;
                    botStepModel.BotTestStepModel.Buttons=Buttons;

                    Bot.Add(Botf);
                    // botStepModel.ChatStepId=fw.childId.Value;
                    // botStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                }
                
                if (fw.type == "List options" || fw.type == "Branches")
                {
                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();


                    botStepModel.BotTestStepModel.LangString = "en";
                    if(fw.type == "Branches")
                    {
                        botStepModel.BotTestStepModel.LangString = "ar";
                    }
                    string img = null;
                    if (!string.IsNullOrEmpty(fw.urlImage.url))
                    {
                        img = fw.urlImage.url;

                    }
                    if (fw.urlImageArray != null && fw.urlImageArray.Length > 0)
                    {

                        img = fw.urlImageArray.FirstOrDefault().url;



                    }
                    else
                    {

                        if (fw.urlImage != null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            img = fw.urlImage.url;

                        }



                    }

                    foreach (var bt in fw.content.dtoContent)
                    {

                        if (botStepModel.BotTestStepModel.LangString == "ar")
                        {
                            cardActions.Add(new CardAction()
                            {
                                Title = bt.valueAr,
                                Value = bt.valueAr,
                                Image = img
                            });
                            Buttons.Add(bt.valueAr);
                        }
                        else
                        {

                            cardActions.Add(new CardAction()
                            {
                                Title = bt.valueEn,
                                Value = bt.valueEn,
                                Image = img
                            });
                            Buttons.Add(bt.valueEn);
                        }


                    }
                    var Summary = "";
                    var text = "";
                    var InputHint = "";
                    if (botStepModel.BotTestStepModel.LangString == "ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                        InputHint = fw.InputHint; //"الرجاء الاختيار";
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                        InputHint = fw.InputHint; //"Please check";
                    }

                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text = text.Replace("$" + result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                            text = text.Replace(result, diVAlue);

                        }

                    }
                    Activity Botf = new Activity();
                    List<Attachment> ss = new List<Attachment>();
                    if (fw.isAdvance)
                    {
                        InputHint = "منطقتك";
                        if (fw.urlImageArray.Length > 0)
                        {
                            img = fw.urlImageArray[0].url;


                        }

                        Botf = new Activity
                        {
                            From = new ChannelAccount("27", "27_27"),
                            Text = text,
                            Speak = img,
                            Type = ActivityTypes.Message,
                            SuggestedActions = new SuggestedActions() { Actions = cardActions },
                            Summary = Summary,
                            Locale = botStepModel.BotTestStepModel.LangString,
                            InputHint = InputHint,
                            Attachments = ss
                        };


                    }
                    else
                    {

                        Botf = new Activity
                        {
                            From = new ChannelAccount("27", "27_27"),
                            Text = text,
                            Speak = text,
                            Type = ActivityTypes.Message,
                            SuggestedActions = new SuggestedActions() { Actions = cardActions },
                            Summary = Summary,
                            Locale = botStepModel.BotTestStepModel.LangString,
                            InputHint = InputHint,
                            Attachments = ss
                        };
                    }


                    botStepModel.BotTestStepModel.IsButton = true;
                    botStepModel.BotTestStepModel.Buttons = Buttons;

                    Bot.Add(Botf);
                }
                if (fw.type == "BranchesEN")
                {
                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();

                    string img = null;
                    if (!string.IsNullOrEmpty(fw.urlImage.url))
                    {
                        img = fw.urlImage.url;

                    }
                    if (fw.urlImageArray != null && fw.urlImageArray.Length > 0)
                    {

                        img = fw.urlImageArray.FirstOrDefault().url;



                    }
                    else
                    {

                        if (fw.urlImage != null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            img = fw.urlImage.url;

                        }



                    }

                    foreach (var bt in fw.content.dtoContent)
                    {
                        cardActions.Add(new CardAction()
                        {
                            Title = bt.valueEn,
                            Value = bt.valueEn,
                            Image = img
                        });
                        Buttons.Add(bt.valueEn);

                    }

                    var Summary = "";
                    var text = "";
                    var InputHint = "";
                    text = fw.captionEn;
                    Summary = fw.footerTextEn;
                    InputHint = fw.InputHint;

                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text = text.Replace("$" + result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                            text = text.Replace(result, diVAlue);

                        }

                    }
                    Activity Botf = new Activity();
                    List<Attachment> ss = new List<Attachment>();
                    if (fw.isAdvance)
                    {
                        InputHint = "منطقتك";
                        if (fw.urlImageArray.Length > 0)
                        {
                            img = fw.urlImageArray[0].url;


                        }

                        Botf = new Activity
                        {
                            From = new ChannelAccount("27", "27_27"),
                            Text = text,
                            Speak = img,
                            Type = ActivityTypes.Message,
                            SuggestedActions = new SuggestedActions() { Actions = cardActions },
                            Summary = Summary,
                            Locale = botStepModel.BotTestStepModel.LangString,
                            InputHint = InputHint,
                            Attachments = ss
                        };


                    }
                    else
                    {

                        Botf = new Activity
                        {
                            From = new ChannelAccount("27", "27_27"),
                            Text = text,
                            Speak = text,
                            Type = ActivityTypes.Message,
                            SuggestedActions = new SuggestedActions() { Actions = cardActions },
                            Summary = Summary,
                            Locale = botStepModel.BotTestStepModel.LangString,
                            InputHint = InputHint,
                            Attachments = ss
                        };
                    }


                    botStepModel.BotTestStepModel.IsButton = true;
                    botStepModel.BotTestStepModel.Buttons = Buttons;

                    Bot.Add(Botf);
                }
                if (fw.type=="Language")
                {
                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();

                    string img = null;
                    if (!string.IsNullOrEmpty(fw.urlImage.url))
                    {
                        img=fw.urlImage.url;

                    }
                    foreach (var bt in fw.content.dtoContent)
                    {

                        if (botStepModel.BotTestStepModel.LangString=="ar")
                        {
                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueAr,
                                Value=bt.valueAr,
                                Image=img
                            });
                            Buttons.Add(bt.valueAr);
                        }
                        else
                        {

                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueEn,
                                Value=bt.valueEn,
                                Image=img
                            });
                            Buttons.Add(bt.valueEn);
                        }


                    }
                    var Summary = "";
                    var text = "";
                    if (botStepModel.BotTestStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                    }

                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                            text= text.Replace(result, diVAlue);

                        }

                    }
                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount("27", "27_27"),
                        Text = text,
                        Speak= text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =  botStepModel.BotTestStepModel.LangString,
                        Attachments = null
                    };


                    //isButton=true;
                    botStepModel.BotTestStepModel.IsButton=true;
                    botStepModel.BotTestStepModel.Buttons=Buttons;

                    Bot.Add(Botf);

                    try
                    {
                        botStepModel.BotTestStepModel.ChatStepId = fw.childId.Value;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId = fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.BotTestStepModel.ChatStepId = -1;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId = fw.parentIndex[0];

                    }

                }
                if (fw.type=="Delay")
                {


                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount("27", "27_27"),
                        Text = (fw.dilationTime*1000).ToString(),
                        Speak= (fw.dilationTime*1000).ToString(),
                        Type = ActivityTypes.Ping,
                        Summary=(fw.dilationTime*1000).ToString(),
                        Locale =  botStepModel.BotTestStepModel.LangString,
                        Attachments = null
                    };
                    Bot.Add(Botf);
                    try
                    {
                        botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.BotTestStepModel.ChatStepId=-1;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                    }
                    //botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                    //botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                }
                if (fw.type=="Human handover")
                {

                    var text = "";// _botApis.UpdateLiveChatAsync(botStepModel.tenantModel.TenantId, botStepModel.customerModel.phoneNumber, "", "", 0, fw.listOfUsers).Result;
                    var Summary = "";



                    if (text=="")
                    {
                        if (botStepModel.BotTestStepModel.LangString=="ar")
                        {
                            text = fw.captionAr;
                            Summary =fw.footerTextAr;// "الرجاء ارسال # للعودة للقائمة الرئسية";
                        }
                        else
                        {
                            text = fw.captionEn;
                            Summary =fw.footerTextEn;// "Please send # to return to the main menu";
                        }
                    }

                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                            text= text.Replace(result, diVAlue);

                        }

                    }

                    List<Activity> activities = new List<Activity>();
                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount("27", "27_27"),
                        Text = text,
                        Speak= text,
                        Type = ActivityTypes.Message,
                        Summary = Summary,
                        Locale =   botStepModel.BotTestStepModel.LangString,
                        Attachments = null
                    };

                    Bot.Add(Botf);

                    try
                    {
                        botStepModel.BotTestStepModel.ChatStepId = fw.childId.Value;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId = fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.BotTestStepModel.ChatStepId = -1;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId = fw.parentIndex[0];

                    }
                }
                if (fw.type=="Jump")
                {

                    try
                    {
                        botStepModel.BotTestStepModel.ChatStepId=fw.jumpId;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.jumpId;
                    }
                    catch
                    {
                        botStepModel.BotTestStepModel.ChatStepId=-1;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=-1;

                    }
                    //botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                    //botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                }
                if (fw.type=="Condition")
                {
                    if (fw.conditional!=null)
                    {


                        bool rez = false;
                        foreach(var op in fw.conditional.conditionList)
                        {
                            // Use Regex.Match to find the first match
                            Match match = Regex.Match(op.op1, pattern);
                            if (match.Success)
                            {
                                // Extract the word after '$' using the captured group
                                try
                                {
                                    string wordAfterDollar = match.Groups[1].Value;
                                    var diVAlue = botStepModel.BotTestStepModel.UserParmeter[wordAfterDollar];
                                    op.op1= op.op1.Replace(wordAfterDollar, diVAlue);
                                    op.op1= op.op1.Replace("$", "");

                                }
                                catch
                                {

                                }

                            }
                            Match match2 = Regex.Match(op.op2, pattern);
                            if (match2.Success)
                            {
                                // Extract the word after '$' using the captured group
                                try
                                {
                                    string wordAfterDollar = match2.Groups[1].Value;
                                    var diVAlue = botStepModel.BotTestStepModel.UserParmeter[wordAfterDollar];
                                    op.op2= op.op2.Replace(wordAfterDollar, diVAlue);
                                    op.op2= op.op2.Replace("$", "");

                                }
                                catch
                                {

                                }

                            }

                            switch (op.operation)
                            {
                                case "Less tham":
                                    if (decimal.Parse( op.op1) < decimal.Parse(op.op2))
                                    {
                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Less or equal to":
                                    if (decimal.Parse(op.op1) <= decimal.Parse(op.op2))
                                    {
                                        rez=true;


                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Greater than":
                                    if (decimal.Parse(op.op1) > decimal.Parse(op.op2))
                                    {

                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Equal to":

                                    if (op.op1 is string)
                                    {
                                        if (op.op1.Equals(op.op2))
                                        {
                                            rez=true;


                                        }
                                        else
                                        {
                                            rez=false;

                                        }
                                    }
                                    else
                                    {
                                        if (decimal.Parse(op.op1) == decimal.Parse(op.op2))
                                        {

                                            rez=true;

                                        }
                                        else
                                        {
                                            rez=false;

                                        }
                                    }
                                                                      
                                    break;
                                case "Not equal to":
                                    if (decimal.Parse(op.op1) != decimal.Parse(op.op2))
                                    {
                                        rez=true;


                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Contains":
                                    if (op.op1.Contains(op.op2))
                                    {
                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Does not contain":
                                    if (!op.op1.Contains(op.op2))
                                    {
                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;


                            }


                        }


                        if (rez)
                        {
                            foreach (var xx in fw.content.dtoContent)
                            {

                                if(xx.valueEn.Trim() =="Yes" || xx.valueAr.Trim() =="Yes")
                                {
                                    botStepModel.BotTestStepModel.ChatStepId = xx.childId.Value;
                                    try
                                    {
                                        botStepModel.BotTestStepModel.ChatStepPervoiusId = xx.parentIndex[0];

                                    }
                                    catch
                                    {

                                    }
                                }
                                

                            }
                        }
                        else
                        {
                            foreach (var xx in fw.content.dtoContent)
                            {

                                if (xx.valueEn.Trim() =="No" || xx.valueAr.Trim() =="No")
                                {
                                    botStepModel.BotTestStepModel.ChatStepId = xx.childId.Value;
                                    try
                                    {
                                        botStepModel.BotTestStepModel.ChatStepPervoiusId = xx.parentIndex[0];

                                    }
                                    catch
                                    {

                                    }
                                }


                            }

                        }
                        isButton=false;

                    }
            
                    //botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                    //botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                }
                if (fw.type=="Http request")
                {

                    var client = new HttpClient();
                    var mathod = HttpMethod.Get;

                    if (fw.request.httpMethod=="POST")
                    {
                        mathod = HttpMethod.Post;

                    }
                    int count = fw.request.url.Count(c => c == '$');
                    for (int i = 0; i<count; i++)
                    {
                        // Use Regex.Match to find the first match
                        Match match = Regex.Match(fw.request.url, pattern);
                        if (match.Success)
                        {
                            // Extract the word after '$' using the captured group
                            try
                            {
                                string wordAfterDollar = match.Groups[1].Value;
                                var diVAlue = botStepModel.BotTestStepModel.UserParmeter[wordAfterDollar];

                                diVAlue=diVAlue.Replace("$", "");
                                fw.request.url= fw.request.url.Replace("$"+wordAfterDollar, diVAlue);

                            }
                            catch
                            {

                            }

                        }


                    }
                    fw.request.url= fw.request.url.Replace("$", "");


                    var request = new HttpRequestMessage(mathod, fw.request.url);


                    if (fw.request.httpMethod=="POST")
                    {
                        if (!string.IsNullOrEmpty(fw.request.token))
                        {
                            request.Headers.Add("Authorization", "Bearer "+fw.request.token);

                        }
                        string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                        MatchCollection matches = Regex.Matches(fw.request.body, pattern3);
                        foreach (Match match in matches)
                        {
                            string result = match.Value;
                            if (result.Contains(".") || result.Contains("["))
                            {
                                var word = result.Split(".")[0];
                                var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                                var textModel = GetTextFromModel(result, diVAlue);
                                fw.request.body= fw.request.body.Replace("$"+result, textModel);

                            }
                            else
                            {
                                var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                                fw.request.body= fw.request.body.Replace("$"+result, diVAlue);

                            }


                        }
                        var body = fw.request.body;//.Replace("\"", "\\\"");
                        var content = new StringContent(body, null, fw.request.contentType);
                        request.Content = content;
                    }


                    var response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    try
                    {
                        var rwz2 = JsonConvert.DeserializeObject<BotFlows.ResponseHttpModel>(response.Content.ReadAsStringAsync().Result);
                        if (rwz2!=null&&!string.IsNullOrEmpty(rwz2.result))
                        {
                            botStepModel.BotTestStepModel.UserParmeter.Remove(fw.parameter);
                            botStepModel.BotTestStepModel.UserParmeter.Add(fw.parameter, rwz2.result);
                        }
                        else
                        {
                            botStepModel.BotTestStepModel.UserParmeter.Remove(fw.parameter);
                            botStepModel.BotTestStepModel.UserParmeter.Add(fw.parameter, response.Content.ReadAsStringAsync().Result);
                        }

                    }
                    catch
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;

                        // Parse the response content to a JObject
                        JObject jsonResponse = JObject.Parse(responseContent);

                        // Extract the "result" property
                        JObject result = (JObject)jsonResponse["result"];

                        // Serialize the "result" object back to a formatted JSON string
                        string formattedJson = JsonConvert.SerializeObject(result, Formatting.Indented);

                        botStepModel.BotTestStepModel.UserParmeter.Remove(fw.parameter);
                        botStepModel.BotTestStepModel.UserParmeter.Add(fw.parameter, formattedJson);

                    }


                    try
                    {
                        botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentId[0];
                    }
                    catch
                    {
                        botStepModel.BotTestStepModel.ChatStepId=-1;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId=-1;

                    }
                }
                if (fw.type=="ScheduleNode")
                {
                    var Summary = "";
                    var text = "";
                    var InputHint = "";

                    var lang = "ar-EG";
                    try
                    {
                        if (string.IsNullOrEmpty(fw.InputHint))
                        {
                            fw.InputHint="Please check";
                        }

                    }
                    catch
                    {
                        fw.InputHint="Please check";
                    }

                    if (fw.InputHint.ToLower()=="please check")
                    {
                        lang = "en-US";
                    }
                    else
                    {

                        lang = "ar-SA";
                    }




                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();

                    string img = null;
                    if (!string.IsNullOrEmpty(fw.urlImage.url))
                    {
                        img=fw.urlImage.url;

                    }

                    if (fw.InputHint.Contains("API2("))
                    {
                        fw.InputHint=fw.InputHint.Replace("API2(", "").Replace(")", "");
                        var parm = fw.InputHint.Replace("$", "");
                        var diVAlue = botStepModel.BotTestStepModel.UserParmeter[parm];

                        Buttons=diVAlue.Split(",").ToList();


                        if (botStepModel.BotTestStepModel.LangString=="ar")
                        {
                            InputHint="منطقتك";
                            fw.InputHint="منطقتك";
                        }
                        else
                        {
                            InputHint="منطقتك";
                            fw.InputHint="منطقتك";
                        }

                        Buttons.RemoveAll(item => string.IsNullOrEmpty(item));

                        foreach (var bin in Buttons)
                        {
                            cardActions.Add(new CardAction()
                            {
                                Title=bin,
                                Value=bin,
                                Image=img
                            });

                        }

                    }else if (fw.InputHint.Contains("API("))
                    {
                        fw.InputHint=fw.InputHint.Replace("API(", "").Replace(")", "");
                        var parm = fw.InputHint.Replace("$", "");
                        var diVAlue = botStepModel.BotTestStepModel.UserParmeter[parm];

                        Buttons=diVAlue.Split(",").ToList();


                        if (botStepModel.BotTestStepModel.LangString=="ar")
                        {
                            InputHint="الرجاء الاختيار";
                            fw.InputHint="الرجاء الاختيار";
                        }
                        else
                        {
                            InputHint="Please Check";
                            fw.InputHint="Please Check";
                        }

                        Buttons.RemoveAll(item => string.IsNullOrEmpty(item));

                        foreach (var bin in Buttons)
                        {
                            cardActions.Add(new CardAction()
                            {
                                Title=bin,
                                Value=bin,
                                Image=img
                            });

                        }

                    }
                    else
                    {

                        if (fw.schedule.isData)
                        {
                            if (fw.schedule.isNow)
                            {
                                DateTime dateTime = DateTime.UtcNow;
                                dateTime=dateTime.AddHours(3);
                                for (var i = 0; i<fw.schedule.numberButton; i++)
                                {
                                    var date = dateTime.AddDays(i).ToString("M/dd/yyyy");
                                    CultureInfo arabicCulture = new CultureInfo(lang);




                                    var dayadd = dateTime.AddDays(i);
                                    string dayNameInArabic = dayadd.ToString("dddd", arabicCulture);

                                    if (!fw.schedule.unavailableDate.Contains(date))
                                    {

                                        cardActions.Add(new CardAction()
                                        {
                                            Title=date + dayNameInArabic,
                                            Value=date + dayNameInArabic,
                                            Image=img
                                        });
                                        Buttons.Add(date+ dayNameInArabic);
                                    }
                                    else
                                    {
                                        fw.schedule.numberButton=fw.schedule.numberButton+1;
                                    }

                                }



                            }
                            else
                            {
                                TimeSpan duration = fw.schedule.startDate.Value.AddHours(3)-fw.schedule.endDate.Value.AddHours(3);

                                // Extract the number of days from the time span
                                int numberOfDays = (int)duration.TotalDays;
                                if (numberOfDays<0)
                                {
                                    numberOfDays=numberOfDays*-1;

                                }

                                if (numberOfDays<=10)
                                {
                                    for (var i = 0; i<numberOfDays; i++)
                                    {
                                        var date = fw.schedule.startDate.Value.AddDays(i).ToString("M/dd/yyyy");

                                        if (!fw.schedule.unavailableDate.Contains(date))
                                        {

                                            cardActions.Add(new CardAction()
                                            {
                                                Title=date,
                                                Value=date,
                                                Image=img
                                            });
                                            Buttons.Add(date);
                                        }
                                        else
                                        {
                                            numberOfDays=numberOfDays+1;
                                        }

                                    }

                                }




                            }


                        }
                        else
                        {
                            if (fw.schedule.isNow)
                            {
                                DateTime dateTime = DateTime.UtcNow;

                                for (var i = 0; i<fw.schedule.numberButton; i++)
                                {
                                    var time = dateTime.AddHours(i).ToString("hh:00 tt");

                                    if (!fw.schedule.unavailableDate.Contains(time))
                                    {

                                        cardActions.Add(new CardAction()
                                        {
                                            Title=time,
                                            Value=time,
                                            Image=img
                                        });
                                        Buttons.Add(time);
                                    }
                                    else
                                    {
                                        fw.schedule.numberButton=fw.schedule.numberButton+1;
                                    }

                                }


                            }
                            else
                            {
                                TimeSpan duration = fw.schedule.startDate.Value - fw.schedule.endDate.Value;

                                // Extract the number of days from the time span
                                int numberOfDays = (int)duration.TotalHours;
                                if (numberOfDays<0)
                                {
                                    numberOfDays=numberOfDays*-1;

                                }

                                if (numberOfDays<=10)
                                {
                                    for (var i = 0; i<numberOfDays; i++)
                                    {
                                        var time = fw.schedule.startDate.Value.AddHours(i).ToString("hh:mm tt");

                                        if (!fw.schedule.unavailableDate.Contains(time))
                                        {

                                            cardActions.Add(new CardAction()
                                            {
                                                Title=time,
                                                Value=time,
                                                Image=img
                                            });
                                            Buttons.Add(time);
                                        }
                                        else
                                        {
                                            numberOfDays=numberOfDays+1;
                                        }

                                    }

                                }
                            }

                        }
                    }





                    List<GetBotFlowForViewDto.Dtocontent> lst = new List<GetBotFlowForViewDto.Dtocontent>();

                    foreach (var bt in Buttons)
                    {
                        GetBotFlowForViewDto.Dtocontent op = new GetBotFlowForViewDto.Dtocontent();
                        op.valueAr=bt;
                        op.valueEn=bt;
                        op.childId=fw.childId;
                        lst.Add(op);
                    }

                    fw.content.dtoContent= lst.ToArray();

                    if (botStepModel.BotTestStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                        InputHint=fw.InputHint; //"الرجاء الاختيار";
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                        InputHint=fw.InputHint; //"Please check";
                    }

                    int count = text.Count(c => c == '$');
                    for (int i = 0; i<count; i++)
                    {
                        // Use Regex.Match to find the first match
                        Match match = Regex.Match(text, pattern);
                        if (match.Success)
                        {
                            // Extract the word after '$' using the captured group
                            try
                            {
                                string wordAfterDollar = match.Groups[1].Value;
                                var diVAlue = botStepModel.BotTestStepModel.UserParmeter[wordAfterDollar];
                                text= text.Replace(wordAfterDollar, diVAlue);

                                //text= text.Replace("$", "");

                            }
                            catch
                            {

                            }

                        }


                    }
                    List<Attachment> ss = new List<Attachment>();
                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount("27", "27_27"),
                        Text = text,
                        Speak= text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =  botStepModel.BotTestStepModel.LangString,
                        InputHint=InputHint,
                        Attachments = ss
                    };


                    botStepModel.BotTestStepModel.IsButton=true;
                    botStepModel.BotTestStepModel.Buttons=Buttons;

                    Bot.Add(Botf);
                }
                if (fw.type=="Set Parameter")
                {
                    if (fw.parameterList!=null)
                    {

                        bool rez = false;
                        foreach (var op in fw.parameterList)
                        {

                            try
                            {
                                string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character
                                if (op.val.Contains("Push"))
                                {
                                    var val = op.val.Replace("Push(", "").Replace(")", "");

                                    // Regex pattern to dynamically match all key-value pairs
                                    pattern = "\\\"([a-zA-Z0-9_]+)\\\":\\$([a-zA-Z0-9_]+)";

                                    // Match the regex pattern
                                    MatchCollection matches = Regex.Matches(val, pattern);

                                    if (matches.Count > 0)
                                    {
                                        Console.WriteLine("Extracted Values:");

                                        // Loop through all matches and print key-value pairs
                                        foreach (Match match in matches)
                                        {
                                            string key0 = match.Groups[1].Value;   // The key (e.g., "product_id")
                                            string value0 = match.Groups[2].Value; // The value (e.g., "product_id")
                                            string result = value0;

                                            if (result.Contains(".") || result.Contains("["))
                                            {
                                                var word = result.Split(".")[0];
                                                var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                                                var textModel = GetTextFromModel(result, diVAlue);
                                                op.val= op.val.Replace("$"+result, textModel);

                                            }
                                            else
                                            {
                                                var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                                                op.val= op.val.Replace("$"+result, diVAlue);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No matches found!");
                                    }



                                }
                                else if (op.val.Contains("+")||op.val.Contains("-"))
                                {

                                    var xx = op.val.Replace("+", " ").Replace("-", "");

                                    MatchCollection matches = Regex.Matches(xx, pattern3);
                                    foreach (Match match in matches)
                                    {
                                        string result = match.Value;
                                        if (result.Contains(".") || result.Contains("["))
                                        {
                                            var word = result.Split(".")[0];
                                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                                            var textModel = GetTextFromModel(result, diVAlue);
                                            op.val= op.val.Replace("$"+result, textModel);

                                        }
                                        else
                                        {
                                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                                            op.val= op.val.Replace("$"+result, diVAlue);

                                        }
                                    }



                                    try
                                    {
                                        var result = new DataTable().Compute(op.val, null);
                                        Console.WriteLine($"The result is: {result}");

                                        op.val=result.ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error evaluating expression: {ex.Message}");
                                    }




                                }
                                else
                                {
                                    MatchCollection matches = Regex.Matches(op.val, pattern3);
                                    foreach (Match match in matches)
                                    {
                                        string result = match.Value;
                                        if (result.Contains(".") || result.Contains("["))
                                        {
                                            var word = result.Split(".")[0];
                                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[word];
                                            var textModel = GetTextFromModel(result, diVAlue);
                                            op.val = op.val.Replace("$"+result, textModel);

                                        }
                                        else
                                        {
                                            var diVAlue = botStepModel.BotTestStepModel.UserParmeter[result];
                                            op.val = op.val.Replace(result, diVAlue);

                                        }

                                    }



                                }





                                var key = op.par.Replace("$", "");

                                if (op.val.Contains("Push"))
                                {

                                    botStepModel.BotTestStepModel.UserParmeter[key]=botStepModel.BotTestStepModel.UserParmeter[key]+","+op.val.Replace("Push(", "").Replace(")", "");

                                }
                                else
                                {


                                    if (op.val.Contains("cls"))
                                    {
                                        botStepModel.BotTestStepModel.UserParmeter[key]="";
                                    }
                                    else
                                    {
                                        botStepModel.BotTestStepModel.UserParmeter[key]=op.val;

                                    }





                                }



                                try
                                {
                                    botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                                    botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                                }
                                catch
                                {
                                    botStepModel.BotTestStepModel.ChatStepId=-1;
                                    botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                                }
                            }
                            catch
                            {


                            }

                        }

                        isButton=false;

                    }
                  


                }
                if (fw.type == "Cataloge multity Product")
                {
                    if (fw.catalogTemplateDto != null)
                    {
                        string header = fw.catalogTemplateDto.Header.Text;
                        string body = fw.catalogTemplateDto.Body.Text;
                        string  footer= fw.catalogTemplateDto.Footer.Text;
                        string CatalogId = fw.catalogTemplateDto.Catalog.CatalogId;
                        var product = fw.catalogTemplateDto.Catalog.Products;
                    }
                }
                if (fw.type == "Integration")
                {
                    if(fw.googleSheetIntegration.IntegrationType == "googleSheets")
                    {
                        if(fw.googleSheetIntegration.GoogleSheetAction == "insertRow")
                        {
                            var parameters = fw.googleSheetIntegration.Parameters;
                            Dictionary<string, string> resultDict = new Dictionary<string, string>();

                            foreach (string parameter in parameters)
                            {
                                if (string.IsNullOrEmpty(parameter))
                                    continue;

                                resultDict[parameter] = botStepModel.BotTestStepModel.UserParmeter.TryGetValue(parameter, out var val) ? val : "";
                            }
                            Dictionary<string, string> row = new Dictionary<string, string>();
                            var worksheetColumns = fw.googleSheetIntegration.WorksheetColumns;

                            for (int i = 0; i < worksheetColumns.Count && i < parameters.Count; i++)
                            {
                                string col = worksheetColumns[i];
                                string param = parameters[i];

                                if (string.IsNullOrEmpty(col) || string.IsNullOrEmpty(param))
                                    continue;

                                string value = resultDict.TryGetValue(param, out var v) ? v : "";
                                row[col] = value;
                            }

                            var body = new InsertSheetRowDto()
                            {
                                spreadsheetId = fw.googleSheetIntegration.SpreadSheetId,
                                tenantId = fw.googleSheetIntegration.TenantId,
                                sheetName = fw.googleSheetIntegration.WorkSheet,
                                row = row
                            };

                            var result = InsertRow(body).Result;

                        }
                        if (fw.googleSheetIntegration.GoogleSheetAction == "getRowByValue")
                        {
                            var lookupValue = fw.googleSheetIntegration.LookupValue;
                            var filterValue = botStepModel.BotTestStepModel.UserParmeter.TryGetValue(lookupValue, out var val) ? val : "";

                            var spreadsheetId = fw.googleSheetIntegration.SpreadSheetId;
                            var sheetName = fw.googleSheetIntegration.WorkSheet;
                            var lookUpColumn = fw.googleSheetIntegration.LookupColumn;
                            var tenantId = fw.googleSheetIntegration.TenantId;

                            var result = GetSheetValues(spreadsheetId, sheetName, lookUpColumn, filterValue, tenantId).Result;
                            var paramToValue = new Dictionary<string, string>();
                            if (result != null && result.Count > 1)
                            {
                                var headers = result.ElementAtOrDefault(0)?.Select(x => x?.ToString()).ToList() ?? new List<string>();
                                var values = result.ElementAtOrDefault(1)?.Select(x => x?.ToString()).ToList() ?? new List<string>();

                                var parameters = fw.googleSheetIntegration.Parameters ?? new List<string>();
                                var worksheetColumns = fw.googleSheetIntegration.WorksheetColumns ?? new List<string>();

                                paramToValue = worksheetColumns
                                    .Select((col, i) => new
                                    {
                                        Param = parameters.ElementAtOrDefault(i),
                                        Header = col
                                    })
                                    .Where(x => !string.IsNullOrWhiteSpace(x.Param) && !string.IsNullOrWhiteSpace(x.Header))
                                    .Select(x => new
                                    {
                                        x.Param,
                                        Value = values.ElementAtOrDefault(headers.IndexOf(x.Header)) ?? ""
                                    })
                                    .ToDictionary(x => x.Param!, x => x.Value!);

                                foreach (var kvp in paramToValue)
                                {
                                    string parameter = kvp.Key;
                                    string value = kvp.Value;
                                    botStepModel.BotTestStepModel.UserParmeter[parameter] = value;
                                }

                            }
                            else
                            {
                                // no rows found >> save null into infoseed variables
                                var parameters = fw.googleSheetIntegration.Parameters ?? new List<string>();

                                foreach (var parameter in parameters)
                                {
                                    if (parameter == null)
                                        continue;
                                    botStepModel.BotTestStepModel.UserParmeter[parameter] = null!;
                                }
                            }

                        }
                    }
                    try
                    {
                        botStepModel.BotTestStepModel.ChatStepId = fw.childId.Value;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId = fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.BotTestStepModel.ChatStepId = -1;
                        botStepModel.BotTestStepModel.ChatStepPervoiusId = fw.parentIndex[0];

                    }

                }


                if (!string.IsNullOrEmpty(fw.parameter) && fw.type!="Http request")
                {

                    isButton=true;
                    botStepModel.getBotFlowForViewDto=fw;

                }

            }
        }
        private string GetTextFromModel(string parm, string result)
        {
            try
            {

                JObject jsonObject = JObject.Parse(result);

                string afterDot = "";
                string after2Dot = "";

                // Split the string by dot
                string[] parts = parm.Split('.');

                // Check if there are at least two parts
                if (parts.Length >= 2)
                {
                    afterDot = parts[1];
                }
                if (parts.Length >= 3)
                {
                    after2Dot = parts[2];
                }


                if (parm.Contains("["))//array
                {


                    string patternarr = @"\[(\d+)\]";
                    int index = -1;
                    // Match the pattern in the input string
                    Match match2 = Regex.Match(parm, patternarr);
                    // Check if a match is found
                    if (match2.Success)
                    {
                        // Get the captured number
                        index = int.Parse(match2.Groups[1].Value); //index

                    }

                    afterDot=afterDot.Replace("["+index+"]", "");

                    JArray fuelPrices = (JArray)jsonObject[afterDot];


                    // Check if the index is within bounds
                    if (index >= 0 && index < fuelPrices.Count)
                    {
                        if (!string.IsNullOrEmpty(after2Dot))
                        {
                            JObject jsonObject2 = JObject.Parse(fuelPrices[index].ToString());

                            JToken propertyValue = jsonObject2[after2Dot];

                            if (propertyValue != null)
                            {
                                return propertyValue.ToString();
                            }
                            else
                            {
                                return "Invalid index";
                            }

                        }
                        else
                        {
                            return fuelPrices[index].ToString();
                        }


                    }
                    else
                    {
                        // Index is out of bounds
                        // return "Invalid index";
                        return "Invalid index";
                    }


                }
                else
                {

                    // Check if the JSON object contains the specified property
                    JToken propertyValue = jsonObject[afterDot];
                    if (propertyValue != null)
                    {
                        return propertyValue.ToString();
                    }
                    else
                    {
                        return "Invalid index";
                    }
                }




            }
            catch (Exception ex)
            {
                return "Invalid index";
            }
        }
        static string GetFileTypeFromUrl(string url)
        {
            try
            {
                // Create a web request with a HEAD method to get the headers
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string contentType = response.ContentType;

                    if (!string.IsNullOrEmpty(contentType))
                    {

                        if (contentType=="application/pdf")
                        {
                            return "file";
                        }
                        // The Content-Type header can contain information about the file type
                        // For example, "image/jpeg" for JPEG images
                        // You can extract the file type from this header
                        return contentType.Split("/")[0];
                    }
                }
            }
            catch (WebException)
            {
                // Handle any errors, such as invalid URLs or network issues
            }

            return "Unknown";
        }
        private GetTestBotFlowDto CreateNewBotTest(int TenantId)
        {

            var itemsCollection = new DocumentCosmoseDB<GetTestBotFlowDto>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var BotTestModel = new GetTestBotFlowDto()
            {
                TenantId = TenantId,
                ItemType=4,
                BotTestStepModel=new MessagingPortal.BotFlow.Dtos.BotTestStepModel() { ChatStepId=0, ChatStepPervoiusId=0, IsLiveChat=false }
            };

            var Result = itemsCollection.CreateItemAsync(BotTestModel).Result;


            return BotTestModel;


        }

        private GetTestBotFlowDto GetBotTest(int TenantId)
        {
            var itemsCollection = new DocumentCosmoseDB<GetTestBotFlowDto>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 4 && a.TenantId == TenantId);
            return customerResult.Result;
        }

        private void UpdateBotTest(GetTestBotFlowDto model)
        {
            try
            {
                var itemsCollection = new DocumentCosmoseDB<GetTestBotFlowDto>(CollectionTypes.ItemsCollection, _IDocumentClient);
             //   var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 4  && a.TenantId == model.TenantId).Result;//&& a.TenantId== TenantId

                //if (model.BotTestStepModel.ChatStepId==-1)
                //{
                //    model.IsReStart=true;
                //}
                var Result = itemsCollection.UpdateItemAsync(model._self, model).Result;
            }
            catch
            {

            }


        }




        private long botParameterCreate(BotParameterModel mdeol)
        {
            try
            {
                var SP_Name = Constants.BotFlows.SP_BotParametrsCreate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    
                     new System.Data.SqlClient.SqlParameter("@Name",mdeol.Name)
                    ,new System.Data.SqlClient.SqlParameter("@Format",mdeol.Format)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",mdeol.TenantId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@BotParameterId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "")
                {
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

        private long botParameterDeleteById(long id)
        {
            try
            {
                var SP_Name = Constants.BotFlows.SP_BotParametrsDelete;
                int TenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Id",id)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "")
                {
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

        private async Task<PagedResultDto<BotParameterModel>> botParameterGetAll(int TenantId)
        {
            try
            {
                List<BotParameterModel> botParameterModel = new List<BotParameterModel>();



                int totalCountOut = 0;
                var SP_Name = Constants.BotFlows.SP_BotParametrsGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                botParameterModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertBotParametrsDto, AppSettingsModel.ConnectionStrings).ToList();
                totalCountOut = (int)OutputParameter.Value;

                return new PagedResultDto<BotParameterModel>(totalCountOut, botParameterModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GoogleSheets

        #region private 
        private GoogleSheetConfigDto GoogleSheetConfigGet(int? tenantId)
        {
            try
            {
                var result = new GoogleSheetConfigDto();
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GoogleSheetConfigGet", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TenantId", tenantId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                result.TenantId = reader.GetInt32(reader.GetOrdinal("TenantId"));
                                result.AccessToken = reader["AccessToken"] as string;
                                result.RefreshToken = reader["RefreshToken"] as string;
                                result.IsConnected = reader["IsConnected"] != DBNull.Value ? (bool)reader["IsConnected"] : (bool?)null;
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void GoogleSheetConfigUpdate(string accessToken, string refreshToken, bool? isConnected, int tenantId)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GoogleSheetConfigUpdate";

                        command.Parameters.AddWithValue("@TenantId", (object)tenantId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AccessToken", accessToken);
                        command.Parameters.AddWithValue("@RefreshToken", refreshToken);
                        command.Parameters.AddWithValue("@IsConnected", isConnected);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task RefreshAccessTokenAsync(int tenantId)
        {
            string clientId = AppSettingsModel.googleSheetClientId;
            string clientSecret = AppSettingsModel.googleSheetClientSecret;
            string tokenUrl = "https://oauth2.googleapis.com/token";

            var config = GoogleSheetConfigGet(tenantId);
            var refreshToken = config.RefreshToken;

            using (var httpClient = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "refresh_token", refreshToken },
                        { "grant_type", "refresh_token" }
                    };

                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(tokenUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to refresh token: {responseBody}");
                }

                var json = JsonDocument.Parse(responseBody);
                var accessToken = json.RootElement.GetProperty("access_token").GetString();
                var newRefreshToken = json.RootElement.TryGetProperty("refresh_token", out var rtElement)
                    ? rtElement.GetString()
                    : refreshToken;

                // Only update refresh token if it's actually new
                if (!string.IsNullOrEmpty(newRefreshToken) && newRefreshToken != refreshToken)
                {
                    GoogleSheetConfigUpdate(accessToken, newRefreshToken, true, tenantId);
                }
                else
                {
                    GoogleSheetConfigUpdate(accessToken, refreshToken, true, tenantId);
                }

            }
        }
        private bool IsFirstRowLikelyHeader(List<string> headers)
        {
            int validHeaderCount = headers.Count(h =>
                !string.IsNullOrWhiteSpace(h) &&
                !double.TryParse(h, out _) &&
                h.Length <= 50
            );

            return validHeaderCount >= 2;
        }


        #endregion

        [HttpGet]
        public async Task<List<IList<object>>> GetSheetValues(string spreadsheetId, string sheetName, string lookUpColumn, string filterValue, int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{Uri.EscapeDataString(sheetName)}";

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await RefreshAccessTokenAsync(tenantId);
                        config = GoogleSheetConfigGet(tenantId);
                        accessToken = config.AccessToken;

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{Uri.EscapeDataString(sheetName)}";

                        response = await httpClient.GetAsync(url);
                    }
                    else
                    {
                        throw new Exception($"Google Sheets API error: {await response.Content.ReadAsStringAsync()}");
                    }
                }

                var content = await response.Content.ReadAsStringAsync();
                dynamic sheetData = JsonConvert.DeserializeObject(content);

                var filteredRows = new List<IList<object>>();

                if (sheetData.values != null)
                {
                    var allRows = ((IEnumerable<dynamic>)sheetData.values)
                                    .Select(row => ((IEnumerable<dynamic>)row).Cast<object>().ToList())
                                    .ToList();

                    if (allRows.Count == 0)
                    {
                        return filteredRows;
                    }

                    var headerRow = allRows[0];
                    filteredRows.Add(headerRow);

                    int columnIndex = headerRow.FindIndex(h =>
                        h?.ToString().Equals(lookUpColumn, StringComparison.OrdinalIgnoreCase) == true);

                    foreach (var row in allRows.Skip(1))
                    {
                        if (row.Count > columnIndex &&
                            row[columnIndex]?.ToString().Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            filteredRows.Add(row);
                        }
                    }
                }

                return filteredRows;
            }
        }

        [HttpPost]
        public async Task<string> InsertRow([FromBody] InsertSheetRowDto rowDto)
        {
            var config = GoogleSheetConfigGet(rowDto.tenantId);
            var accessToken = config.AccessToken;

            using (var httpClient = new HttpClient())
            {
                //first get all column names ((headers))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var headerUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{rowDto.spreadsheetId}/values/{rowDto.sheetName}!1:1";

                var headerResponse = await httpClient.GetAsync(headerUrl);

                if (!headerResponse.IsSuccessStatusCode)
                {
                    if (headerResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await RefreshAccessTokenAsync(rowDto.tenantId);
                        config = GoogleSheetConfigGet(rowDto.tenantId);
                        accessToken = config.AccessToken;
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        headerUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{rowDto.spreadsheetId}/values/{rowDto.sheetName}!1:1";
                        headerResponse = await httpClient.GetAsync(headerUrl);

                        if (!headerResponse.IsSuccessStatusCode)
                        {
                            return $"Failed to get headers: {await headerResponse.Content.ReadAsStringAsync()}";
                        }
                    }
                    else
                    {
                        return $"Failed to get headers: {await headerResponse.Content.ReadAsStringAsync()}";
                    }
                }

                var headerContent = await headerResponse.Content.ReadAsStringAsync();
                dynamic headerData = JsonConvert.DeserializeObject(headerContent);
                var headers = ((IEnumerable<dynamic>)headerData.values[0]).Select(h => h.ToString()).ToList();

                //order the row to match the order of column names
                var orderedRow = headers.Select(h => rowDto.row.ContainsKey(h) ? rowDto.row[h] : "").ToList();

                var appendUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{rowDto.spreadsheetId}/values/{rowDto.sheetName}!A1:append?valueInputOption=RAW";

                var body = new
                {
                    values = new List<IList<object>> { orderedRow }
                };
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var appendResponse = await httpClient.PostAsync(appendUrl, content);

                var appendResult = await appendResponse.Content.ReadAsStringAsync();

                if (!appendResponse.IsSuccessStatusCode)
                {
                    return $"Insert failed: {appendResult}";
                }

                return "Row inserted successfully.";

            }
        }

        [HttpGet]
        public async Task<List<string>> GetWorkSheets(string spreadsheetId, int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;
            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await RefreshAccessTokenAsync(tenantId);
                    config = GoogleSheetConfigGet(tenantId);
                    accessToken = config.AccessToken;

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}";

                    response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return new List<string> { $"Google Sheets API error: {await response.Content.ReadAsStringAsync()}" };
                    }
                }
                else
                {
                    return new List<string>{$"Google Sheets API error: {await response.Content.ReadAsStringAsync()}"};
                }
            }

            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);

            var sheetTitles = doc.RootElement
                                 .GetProperty("sheets")
                                 .EnumerateArray()
                                 .Select(sheet => sheet.GetProperty("properties").GetProperty("title").GetString())
                                 .ToList();

            return sheetTitles;
        }

        [HttpGet]
        public async Task<List<string>> GetLookupHeaders(string spreadsheetId, string sheetName, int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var headerUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{sheetName}!1:1";

                HttpResponseMessage headerResponse = await httpClient.GetAsync(headerUrl);

                if (!headerResponse.IsSuccessStatusCode)
                {
                    if (headerResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await RefreshAccessTokenAsync(tenantId);
                        config = GoogleSheetConfigGet(tenantId);
                        accessToken = config.AccessToken;
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        headerResponse = await httpClient.GetAsync(headerUrl);
                    }

                    if (!headerResponse.IsSuccessStatusCode)
                    {
                        return new List<string> { $"Failed to get headers: {await headerResponse.Content.ReadAsStringAsync()}" };
                    }
                }

                var headerContent = await headerResponse.Content.ReadAsStringAsync();
                var headerData = JsonConvert.DeserializeObject<SheetValueResponse>(headerContent);
                var headers = headerData?.Values?.FirstOrDefault()?.Select(h => h?.ToString())?.ToList() ?? new List<string>();

                if (!IsFirstRowLikelyHeader(headers))
                {
                    return new List<string> { "Invalid sheet structure: Expected column names in the first row." };
                }

                return headers;
            }
        }


        [HttpGet]
        public async Task<GetSpreadSheetsDto> GetSpreadSheets(int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;
            var spreadSheetResult = new GetSpreadSheetsDto();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var url = "https://www.googleapis.com/drive/v3/files?q=mimeType='application/vnd.google-apps.spreadsheet'";

                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {

                        await RefreshAccessTokenAsync(tenantId);
                        config = GoogleSheetConfigGet(tenantId);
                        accessToken = config.AccessToken;

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        url = "https://www.googleapis.com/drive/v3/files?q=mimeType='application/vnd.google-apps.spreadsheet'";

                        response = await httpClient.GetAsync(url);

                        if (!response.IsSuccessStatusCode)
                        {
                            spreadSheetResult.ErrorMsg = $"Google Sheets API error: {await response.Content.ReadAsStringAsync()}";
                            return spreadSheetResult;
                        }
                    }
                    else
                    {
                        spreadSheetResult.ErrorMsg = $"{await response.Content.ReadAsStringAsync()}";
                        return spreadSheetResult;
                    }
                }

                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = JsonConvert.DeserializeObject<DriveApiResponse>(content);
                    spreadSheetResult.Files = json?.Files ?? new List<GoogleDriveFile>();
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    spreadSheetResult.ErrorMsg = $"Invalid JSON returned from API: {content}";
                }

                return spreadSheetResult;
            }
        }


        #endregion
    }
}
