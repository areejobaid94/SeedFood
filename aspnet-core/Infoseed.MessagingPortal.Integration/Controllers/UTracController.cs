
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using Infoseed.MessagingPortal.Web.Controllers;
//using Infoseed.MessagingPortal.UTracOrder;
//using Infoseed.MessagingPortal.UTracOrder.Dto;
//using Framework.Data;
//using Microsoft.Azure.Documents;
//using Infoseed.MessagingPortal.Web.Sunshine;

//namespace Infoseed.MessagingPortal.Bot.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UTracController : MessagingPortalControllerBase
//    {
//        //  private string serverKey = "AAAAKInP3fI:APA91bHUaWCGK8ea8261aoGceJ4uGd7mJLHAl5vUMDiUFskG_t5iImtYqYMaLHZpiRHyFNAA2Cl4zEQ18Gnq6pRbP57PEhw1Z8DYIL9G07wqvZcaW1Gg7gbqqOvY403IzoJfQXx-CqSi";
//        //  private string senderId = "174110793202";
//        // private string webAddr = "https://fcm.googleapis.com/fcm/send";

//        private readonly IUTrackOrderAppService _uTrackOrderAppService;
//         IDocumentClient _documentClient;
//         IDBService _dbService;

//        public UTracController(IUTrackOrderAppService uTrackOrderAppService, IDocumentClient documentClient, IDBService dbService)
//        {
//            _uTrackOrderAppService=uTrackOrderAppService;
//            _documentClient = documentClient;
//            _dbService = dbService;

//        }

//        //[Route("AddOrder")]
//        //[HttpPost]
//        //public async Task<long> AddOrderAsync(UTracOrderModel uTracOrderModel)
//        //{
//        //    var Tenant = await _dbService.GetTenantInfoById(uTracOrderModel.TenantId);
//        //    var itemsCollection = new DocumentCosmoseDB<Web.Models.Sunshine.CustomerModel>(CollectionTypes.ItemsCollection, _documentClient);

//        //    string userId = uTracOrderModel.TenantId + "_" + uTracOrderModel.PhoneNumber;
//        //    var Customer = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == uTracOrderModel.TenantId).Result;

//        //    if (Customer == null)
//        //    {
//        //        var name = uTracOrderModel.ContactName;

//        //        Customer = _dbService.CreateNewCustomer(uTracOrderModel.PhoneNumber, uTracOrderModel.ContactName, "text", Tenant.botId, uTracOrderModel.TenantId, Tenant.D360Key);

//        //    }
//        //    return _uTrackOrderAppService.CreateNewUTracOrder(uTracOrderModel);

//        //}

//        //[Route("UpdateOrder")]
//        //[HttpPost]
//        //public void UpdateOrder(UTracOrderModel uTracOrderModel)
//        //{
//        //    _uTrackOrderAppService.UpdateUTracOrder(uTracOrderModel);
//        //}


//        //[Route("SubmitOrder")]
//        //[HttpPost]
//        //public void SubmitOrder(SubmitUTracOrder order)
//        //{
//        //    _uTrackOrderAppService.SubmitUtracOrder(order);
//        //}

//        //[Route("CancelOrder")]
//        //[HttpPost]
//        //public void CancelOrder(UTracOrderModel model)
//        //{
//        //    _uTrackOrderAppService.CancelUtracOrder(model);
//        //}


//        //[Route("CancelUtracOrder")]
//        //[HttpPost]
//        //public void CancelUtracOrder(SubmitUTracOrder model)
//        //{


//        //}

//    }
//}