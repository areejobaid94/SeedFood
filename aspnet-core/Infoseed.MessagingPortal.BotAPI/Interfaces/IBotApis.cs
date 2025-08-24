using Azure;
using Azure.Communication.Email;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.BotAPI.Models;
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Interfaces
{
    public interface IBotApis
    {

        Task<TenantModel> GetTenantAsync(int TenantID);
        Area GetAreasID2(string TenantID, string AreaName, int menu, string local);
        GetLocationInfoModel GetlocationUserModel( SendLocationUserModel input);
        List<CaptionDto> GetAllCaption(int TenantID, string local);
        OrderAndDetailsModel GetOrderAndDetails(GetOrderAndDetailModel input);
        OrderAndDetailsModel GetOrderAndDetailsFlowsBot(GetOrderAndDetailModel input);

        CancelOrderModel UpdateCancelOrder(int? TenantID, string OrderNumber, int ContactId, string CanatCancelOrderText);
        List<string> GetAreasWithPage(string TenantID, string local, int menu, int pageNumber, int pageSize, bool isDelivery);
        string AddMenuContcatKey(MenuContcatKeyModel model);
        string UpdateOrderAsync(UpdateOrderModel updateOrderModel);
        void DeleteOrderDraft(int tenantID, int orderId);

        void UpdateCustomerBehavior(CustomerBehaviourModel behaviourModel);
        Task<string> UpdateLiveChatAsync(int? TenantID, string phoneNumber, string Department1 = null, string Department2 = null, int DepartmentId = 0, string UserIds = "");
        Task<string> UpdateNewLiveChatAsync(int? TenantID, string phoneNumber, string Department1 = null, string Department2 = null, int DepartmentId = 0, string UserIds = "", string DepartmentInfo = "");

        List<string> GetDay(string local);
        List<string> GetTime(int TenantID, string selectDay, string local);
        void CreateEvaluations(int TenantId, string phoneNumber, string displayName, string EvaluationsText, string orderID, string EvaluationsReat);


        Task<List<string>> GetUserByBranches(int TenantId, string UserIds);
        Task<UserListDto> GetUserModelByBranches(int TenantId, string UserName);
        Dictionary<string, string> GetBookingDay(long userId, int languageId);
        List<string> GetBookingTime(string date, int tenantId, long userId);
        Task<string> CreateBooking(BookingModel booking);
        List<BookingModel> GetContactBooking(int contactId, int tenantId, int languageId);
        Task<string> CancelBooking(string bookingId);
        Task<string> ConfirmBooking(string bookingId);


        string checkIsFillDisplayName(int id);
        void updateContcatDisplayName(int id, string contactDisplayName);
        Task<List<GetListPDFModel>> GetAssetOffers(int tenantID, string phoneNumber = "");
        Task SendPrescription(UpdateSaleOfferModel updateOrderModel);
        void SendNewPrescription(UpdateSaleOfferModel updateOrderModel, string JolInformation = null);
        List<GetAssetModel> GetAssetLevel(int tenantId, string local, int stepId, int levelId = 0);
        Task<List<GetListPDFModel>> GetAsset(int tenantID, string phoneNumber, int? typeId = null, long? levleOneId = 0, long? levelTwoId = 0, long? levelThreeId = 0, long? levelFourId = 0, bool isOffer = false);
        void CreateInterestedOf(CustomerInterestedOf interestedOf);



        void UpdaateDisplayName(string contactID, string displayName = "");
        void UpdaateLocation(string contactID, string location = "");


        Task SendEmailAsync(string toEmail, string subject, string body,string userIds);
        Task<string> EndDialog(string phonenumber, string teanantId);
        Task<List<IList<object>>> GetSheetValues(string spreadsheetId, string sheetName, string lookUpColumn, string filterValue, int tenantId);
        Task<string> InsertRow(InsertSheetRowDto rowDto);
        Task<List<string>> GetWorkSheets(string spreadsheetId, int tenantId);
        Task<List<string>> GetLookupHeaders(string spreadsheetId, string sheetName, int tenantId);
        Task<GetSpreadSheetsDto> GetSpreadSheets(int tenantId);







    }
}
