//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using Infoseed.MessagingPortal.Web.Controllers;
//using Infoseed.MessagingPortal.UTracOrder;
//using Infoseed.MessagingPortal.UTracOrder.Dto;
//using Framework.Data;
//using Microsoft.Azure.Documents;
//using Infoseed.MessagingPortal.Web.Sunshine;
//using Infoseed.MessagingPortal.TargetReach.Dto;
//using Infoseed.MessagingPortal.TargetReach;
//using System.Collections.Generic;

//namespace Infoseed.MessagingPortal.Integration.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TargetReachController : MessagingPortalControllerBase
//    {
  

//        IDocumentClient _documentClient;
//        IDBService _dbService;
//        ITargetReachAppService _ITargetReachAppService;

//        public TargetReachController(IDocumentClient documentClient, IDBService dbService, ITargetReachAppService iTargetReachAppService)
//        {
            
//            _documentClient = documentClient;
//            _dbService = dbService;
//            _ITargetReachAppService = iTargetReachAppService;

//        }

//        [Route("SendMessage")]
//        [HttpPost]
//        public async Task<string> SendMessageAsync(List<TargetReachModel> lstTargetReachModel, string key)
//        {
//            string result = string.Empty;

           
//            if (lstTargetReachModel.Count > 10)
//            {
//                return  "ExceededLimit";

//            }
//            if (lstTargetReachModel != null)
//            {
//                List<TargetReachEntity> lstTargetReachEntity = new List<TargetReachEntity>();
//                foreach (var item in lstTargetReachModel)
//                {

//                    var Tenant = await _dbService.GetTenantInfoByPhoneNumber(item.SenderPhoneNumber);


//                    if (key!="InfoSeed"+"_"+Tenant.PhoneNumber)
//                    {
//                        return "WrongData";

//                    }
//                    if (Tenant != null)
//                    {
//                        var itemsCollection = new DocumentCosmoseDB<Web.Models.Sunshine.CustomerModel>(CollectionTypes.ItemsCollection, _documentClient);

//                        string userId = Tenant.TenantId + "_" + item.ReciverPhoneNumber;
//                        var Customer = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId).Result;

//                        if (Customer == null)
//                        {
//                            var name = item.ReciverName;

//                            Customer = _dbService.CreateNewCustomer(item.ReciverPhoneNumber, item.ReciverName, "text", Tenant.botId, Tenant.TenantId.Value, Tenant.D360Key);

//                        }

//                        TargetReachEntity targetReachEntity = new TargetReachEntity();
//                        targetReachEntity.Message = item.MssageContent;
//                        targetReachEntity.TenantId = Tenant.TenantId.Value;
//                        targetReachEntity.ContactId = int.Parse(Customer.ContactID);
//                        targetReachEntity.PhoneNumber = item.ReciverPhoneNumber;
//                        targetReachEntity.CustomerName = item.ReciverName;
//                        targetReachEntity.UserId = userId;
//                        targetReachEntity.TemplateName = TargetReachTemplateEnum.target_reach.ToString();

//                        lstTargetReachEntity.Add(targetReachEntity);
//                        result ="ReceivedData";
//                    }
//                    else
//                    {
//                        result = "WrongData";
//                    }
//                }

//                if (lstTargetReachEntity.Count> 0)
//                _ITargetReachAppService.SetTargetReachMessageInQueue(lstTargetReachEntity);

//            }
//            return result;
//        }


//    }
//}