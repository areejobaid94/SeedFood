namespace Infoseed.MessagingPortal
{
    public static class Constants
    {
        public static class Dashboard
        {
            public static string SP_ConversationMeasurementsGet = "[dbo].[ConversationMeasurementsGet]";
            public static string SP_ConversationMeasurementBIUpdate = "[dbo].[ConversationMeasurementBIUpdate]";
            public static string SP_DashboardNumbersGet = "[dbo].[DashboardNumbersGet]";
            public static string SP_DashboardNumberAdd = "[dbo].[DashboardNumberAdd]";
            public static string SP_BestSellingGetTen = "[dbo].[BestSellingGetTen]";
            public static string UsageDetailsGet = "[dbo].[UsageDetailsGet]";
            public static string UsageStatisticsGet = "[dbo].[UsageStatisticsGet]";

        }
        public static class Evaluation
        {
            public static string SP_EvaluationAdd = "[dbo].[EvaluationAdd]";
            public static string SP_EvaluationGet = "[dbo].[EvaluationGet]";
        }
        public static class Order
        {
            public static string SP_OrdersGet = "[dbo].[OrdersGet]";
            public static string SP_OrderByIdGet = "[dbo].[OrderByIdGet]";
            public static string SP_OrderStatusUpdate = "[dbo].[OrderStatusUpdate]";
            public static string SP_OrdersArchivedGet = "[dbo].[OrdersArchivedGet]";
            public static string SP_GetOrderStatusSummaryByTenantAndDate = "[dbo].[GetOrderStatusSummaryByTenantAndDate]";
            public static string SP_OrderExtraDetailsGet = "[dbo].[OrderExtraDetailsGet]";
            public static string SP_OrderAdd = "[dbo].[OrderAdd]";
            public static string SP_OrderDetailsExtraDetailsAdd = "[dbo].[OrderDetailsExtraDetailsAdd]";
            public static string SP_AreasByTenantIdGet = "[dbo].[AreasByTenantIdGet]";
            public static string SP_OrderStatusHistoryGet = "[dbo].[OrderStatusHistoryGet]";
            public static string SP_OrderStatusHistoryAdd = "[dbo].[OrderStatusHistoryAdd]";
            public static string SP_OrderStatusHistoryValidationGet = "[dbo].[OrderStatusHistoryValidationGet]";
            public static string SP_OrderMenuDetailExtraDetailsGet = "[dbo].[OrderMenuDetailExtraDetailsGet]";

            public static string SP_OrderDetailsAdd = "[dbo].[OrderDetailsAdd]";
            public static string SP_OrderDetailsExtraAdd = "[dbo].[OrderDetailsExtraAdd]";
            public static string SP_OrderDetailsSpecificationAdd = "[dbo].[OrderDetailsSpecificationAdd]";
            public static string SP_ContactOrdersGet = "[dbo].[ContactOrdersGet]";
            public static string SP_OrderDetailsExtraDetailsGet = "[dbo].[OrderDetailsExtraDetailsGet]";
            public static string SP_LoyaltyRemainingdays = "[dbo].[LoyaltyRemainingdays]";
            public static string SP_OrdersGetAll = "[dbo].[OrdersGetAll]";
            public static string SP_OrdersStatisticsGet = "[dbo].[OrdersStatisticsGet]";



        }
        public static class SellingRequest
        {
            public static string SP_SellingRequestFormAdd = "[dbo].[SellingRequestFormAdd]";
            public static string SP_SellingRequestAdd = "[dbo].[SellingRequestAdd]";
            public static string SP_SellingRequestUpdate = "[dbo].[SellingRequestUpdate]";
            public static string SP_SellingRequestGet = "[dbo].[SellingRequestGet]";
            public static string SP_SellingRequestByIDGet = "[dbo].[SellingRequestByIDGet]";
            public static string SP_SellingRequestDelete = "[dbo].[SellingRequestDelete]";
            public static string SP_SellingRequestDone = "[dbo].[SellingRequestDone]";
            public static string SP_SignUpRequestFormAdd = "[dbo].[SignUpRequestFormAdd]";
            public static string SP_TicketUpdateStatus = "[dbo].[TicketUpdateStatus]";
            public static string SP_TicketStatusGet = "[dbo].[TicketStatusGet]";
        }

        public static class Asset
        {
            public static string SP_AssetAdd = "[dbo].[AssetAdd]";
            public static string SP_AssetUpdate = "[dbo].[AssetUpdate]";
            public static string SP_AssetGet = "[dbo].[AssetGet]";
            public static string SP_AssetOfferGet = "[dbo].[AssetOfferGet]";
            public static string SP_AssetByIDGet = "[dbo].[AssetByIDGet]";
            public static string SP_AssetDelete = "[dbo].[AssetDelete]";
            public static string SP_LevelAssetGet = "[dbo].[LevelAssetGet]";
            public static string SP_LevelAssetDistinctGet = "[dbo].[LevelAssetDistinctGet]";
            public static string SP_AssetByLevelsAndTypeGet = "[dbo].[AssetByLevelsAndTypeGet]";
            public static string SP_AssetGetOffer = "[dbo].[AssetGetOffer]";
            public static string SP_MgMotorsGetAsset = "[dbo].[MgMotorsGetAsset]";

        }
        public static class DeliveryCost
        {
            public static string SP_DeliveryCostAdd = "[dbo].[DeliveryCostAdd]";
            public static string SP_DeliveryCostUpdate = "[dbo].[DeliveryCostUpdate]";
            public static string SP_DeliveryCostGet = "[dbo].[DeliveryCostGet]";
            public static string SP_DeliveryCostByIDGet = "[dbo].[DeliveryCostByIDGet]";
            public static string SP_DeliveryCostDelete = "[dbo].[DeliveryCostDelete]";
            public static string SP_DeliveryCostByAreaIDGet = "[dbo].[DeliveryCostByAreaIDGet]";
            public static string SP_DeliveryCostPerAreaGet = "[dbo].[DeliveryCostPerAreaGet]";
        }


        public static class Contacts
        {
            public static string SP_ContactbyIdGet = "[dbo].[ContactbyIdGet]";
            public static string SP_ContactUpdate = "[dbo].[ContactUpdate]";
            public static string SP_ContactkinshipUpdate = "[dbo].[ContactkinshipUpdate]";
            public static string SP_ContactsExternalBulkAdd = "[dbo].[ContactsExternalBulkAdd]";
            public static string SP_ContactExternalAdd = "[dbo].[ContactExternalAdd]";
            public static string SP_ContactsExternalGet = "[dbo].[ContactsExternalGet]";
            public static string SP_ContactsExternalByCampaignGet = "[dbo].[ContactsExternalByCampaignGet]";
            public static string SP_ContactsCampaignlBulkAdd = "[dbo].[ContactsCampaignBulkAdd]";
            public static string SP_ContactsInterrestedOfAdd = "[dbo].[ContactsInterrestedOfAdd]";
            public static string SP_CustomerBehaviourUpdate = "[dbo].[CustomerBehaviourUpdate]";
            public static string SP_ContactsInterestedOfGet = "[dbo].[ContactsInterestedOfGet]";
            public static string SP_ContactsAdd = "[dbo].[ContactsAdd]";
            public static string SP_ContactInfoUpdate = "[dbo].[ContactInfoUpdate]";
            public static string SP_ContactsGet = "[dbo].[ContactsGet]";
            public static string SP_ContactDelete = "[dbo].[ContactDelete]";
            public static string SP_ContactCheckIfExistGet = "[dbo].[ContactCheckIfExistGet]";
            public static string SP_ContactsCampaignToExcelGet = "[dbo].[ContactsCampaignToExcelGet]";
            public static string SP_ContactsOptOutByTenantIdGet = "[dbo].[ContactsOptOutByTenantIdGet]";
            public static string SP_ContactsStatisticsGet = "[dbo].[ContactsStatisticsGet]";
            public static string SP_SendCampaignTeamInbox = "[dbo].[SendCampaignTeamInbox]";
            public static string SP_NickNameUpdate = "[dbo].[NickNameUpdate]";
            public static string SP_ContactsTeamInpoxGetAll = "[dbo].[ContactsTeamInpoxGetAll]";
            public static string SP_CheckPhoneNumberFormat = "[dbo].[CheckPhoneNumberFormat]";
            public static string SP_SendCampaignFromGroup = "[dbo].[SendCampaignFromGroup]";
            public static string SP_SendCampaignNew = "[dbo].[SendCampaignNew]";
            public static string SP_DeleteContactFailed = "[dbo].[DeleteContactFailed]";
            public static string SP_DeleteContactFromGroup = "[dbo].[DeleteContactFromGroup]";
            public static string SP_ContactUpdatePhoneNumber = "[dbo].[ContactUpdatePhoneNumber]";
            public static string SP_ContactGetFromGroup = "[dbo].[ContactGetFromGroup]";

            public static string SP_ContactNameUpdate = "[dbo].[ContactNameUpdate]";
            public static string SP_ContactLocationUpdate = "[dbo].[ContactLocationUpdate]";
            public static string SP_CustomerBehaviourUpdateByUser = "[dbo].[CustomerBehaviourUpdateByUser]";
        }
        public static class MenuContactKey
        {

            public static string SP_MenuContactKeyAdd = "[dbo].[MenuContactKeyAdd]";
            public static string SP_MenuContactKeyGet = "[dbo].[MenuContactKeyGet]";
        }
        public static class Currencies
        {
            public static string SP_CurrenciesGet = "[dbo].[CurrenciesGet]";
            public static string SP_CurrencyGetByISOName = "[dbo].[CurrencyGetByISOName]";
        }
        public static class ItemCategory
        {
            public static string SP_ItemWithCategoryGet = "[dbo].[ItemWithCategoryGet]";
            public static string SP_ItemCategoryDelete = "[dbo].[ItemCategoryDelete]";
        }
        public static class ItemSubCategory
        {
            public static string SP_ItemSubCategoryDelete = "[dbo].[ItemSubCategoryDelete]";
        }
        public static class Item
        {
            public static string SP_ItemDelete = "[dbo].[ItemDelete]";
            public static string SP_ItemImagesDelete = "[dbo].[ItemImagesDelete]";
            public static string SP_ItemImagesAdd = "[dbo].[ItemImagesAdd]";
            public static string SP_ItemByCategoryAndSubCategoryGet = "[dbo].[ItemByCategoryAndSubCategoryGet]";
        }
        public static class LiveChat
        {
            public static string SP_UpdateConversationsCount = "[dbo].[UpdateConversationsCount]";

            public static string SP_LiveChatGet = "[dbo].[LiveChatGet]";
            public static string SP_newLiveChatGet = "[dbo].[newLiveChatGet]";
            public static string SP_CustomersLiveAdd = "[dbo].[LiveChatAdd]";
            public static string SP_CustomersLiveUpdate = "[dbo].[LiveChatUpdate]";
            public static string SP_UpdateIsOpenLiveChat = "[dbo].[UpdateIsOpenLiveChat]";
            public static string SP_UpdateNote = "[dbo].[UpdateNote]";

            public static string SP_TicketUpdate = "[dbo].[TicketUpdate]";
            public static string SP_LiveChatToExcelGet = "[dbo].[LiveChatToExcelGet]";
            public static string SP_LiveChatAssignToUserUpdate = "[dbo].[LiveChatAssignToUserUpdate]";
            public static string SP_TicketsGetAll = "[dbo].[TicketsGetAll]";
            public static string SP_TicketsStatisticsGet = "[dbo].[TicketsStatisticsGet]";
            public static string SP_TickitAddAndRequest = "[dbo].[TickitAddAndRequest]";

            public static string SP_GetLiveChatData = "[dbo].[GetLiveChatData]";
            public static string SP_GetTicketCountsByTenant = "[dbo].[GetTicketCountsByTenant]";
            public static string SP_GetAllTenantStatisticsToExportExcel = "[dbo].[GetAllTenantStatisticsToExportExcel]";

        }
        public static class JoPetrolBot
        {
            public static string SP_JoPetrolGetAllPhoneNumber = "[dbo].[JoPetrolGetAllPhoneNumber]";
            public static string SP_JoPetrolGetLastFiveRequest = "[dbo].[JoPetrolGetLastFiveRequest]";

        }

        public static class Loyalty
        {
            public static string SP_LoyaltyGet = "[dbo].[LoyaltyGet]";
            public static string SP_LoyaltyAdd = "[dbo].[LoyaltyAdd]";
            public static string SP_LoyaltyUpdate = "[dbo].[LoyaltyUpdate]";
            public static string SP_ContactLoyaltyTransactionAdd = "[dbo].[ContactLoyaltyTransactionAdd]";
            public static string SP_ContactLoyaltyTransactionUpdate = "[dbo].[ContactLoyaltyTransactionUpdate]";



            public static string SP_ItemLoyaltyGet = "[dbo].[ItemLoyaltyGet]";

            public static string SP_SpecificationsLoyaltyGet = "[dbo].[SpecificationsLoyaltyGet]";
            public static string SP_AdditionsLoyaltyGet = "[dbo].[AdditionsLoyaltyGet]";

        }
        public static class PermissionsUser
        {
            public static string SP_PermissionsUser = "[dbo].[PermissionsUserGet]";

        }

        public static class WhatsAppTemplates //
        {
            public static string InstagramUrl = "https://graph.instagram.com/v22.0/";
            public static string WhatsAppApiUrl = "https://graph.facebook.com/v17.0/";
            //public static string WhatsAppApiUrl = "https://graph.facebook.com/v17.0/";

            public static string WhatsAppApiUrlNew = "https://graph.facebook.com/v18.0/";
            public static string SP_WhatsAppTemplatesAdd = "[dbo].[WhatsAppTemplateAdd]";
            public static string SP_WhatsAppTemplateUpdate = "[dbo].[WhatsAppTemplateUpdate]";
            public static string SP_WhatsAppTemplatesDelete = "[dbo].[WhatsAppTemplateDelete]";
            public static string SP_WhatsAppTemplatesGet = "[dbo].[WhatsAppMessageTemplateGet]";
            public static string SP_WhatsAppTemplatesGetById = "[dbo].[WhatsAppMessageTemplateGetById]";
            public static string SP_TemplateGetByWhatsAppId = "[dbo].[TemplateGetByWhatsAppId]";
            public static string SP_WhatsAppTemplateByNameGet = "[dbo].[WhatsAppTemplateByNameGet]";
            public static string SP_GetTemplatesCategory = "[dbo].[GetTemplatesCategory]";
            public static string SP_IntegrationGetTemplatesInfo = "[dbo].[IntegrationGetTemplatesInfo]";
        }

        public static class WhatsAppCampaign //
        {
            public static string SP_WhatsAppCampaignAdd = "[dbo].[WhatsAppCampaignAdd]";
            public static string SP_WhatsAppCampaignDelete = "[dbo].[WhatsAppCampaignDelete]";
            public static string SP_WhatsAppCampaignUpdate = "[dbo].[WhatsAppCampaignUpdate]";
            public static string SP_WhatsAppCampaignGet = "[dbo].[WhatsAppCampaignGet]";
            public static string SP_WhatsAppCampaignHistoryGet = "[dbo].[WhatsAppCampaignHistoryGet]";
            public static string SP_WhatsAppContactFilterCountGet = "[dbo].[WhatsAppContactFilterCountGet]";
            public static string SP_WhatsAppContactFilterGet = "[dbo].[WhatsAppContactFilterGet]";
            public static string SP_ContactFilterGetCmap = "[dbo].[ContactFilterGetCmap]";
            public static string SP_WhatsAppScheduledCampaignAdd = "[dbo].[WhatsAppScheduledCampaignAdd]";
            public static string SP_ScheduledCampaignAddOnDB = "[dbo].[ScheduledCampaignAddOnDB]";
            public static string SP_SendCampaignAddOnDB = "[dbo].[SendCampaignAddOnDB]";

            public static string SP_WhatsAppScheduledCampaignGet = "[dbo].[WhatsAppScheduledCampaignGet]";
            public static string SP_WhatsAppCampaignByTemplateGet = "[dbo].[WhatsAppCampaignByTemplateGet]";
            public static string SP_WhatsAppCampaignActivationUpdate = "[dbo].[WhatsAppCampaignActivationUpdate]";

            public static string SP_WhatsAppContactCountFilterGet = "[dbo].[WhatsAppContactCountFilterGet]";
            public static string SP_WhatsAppScheduledCampaignByCampaignIdGet = "[dbo].[WhatsAppScheduledCampaignByCampaignIdGet]";
            public static string SP_WhatsAppSendCampaignValidationGet = "[dbo].[WhatsAppSendCampaignValidationGet]";
            public static string SP_WhatsAppCampaignByNameGet = "[dbo].[WhatsAppCampaignByNameGet]";

            public static string SP_CampaignGetAll = "[dbo].[CampaignGetAll]";
            public static string SP_CampaignStatisticsGet = "[dbo].[CampaignStatisticsGet]";
            public static string SP_ChangeCampaignActive = "[dbo].[ChangeCampaignActive]";
            public static string SP_DailylimitGetCount = "[dbo].[DailylimitGetCount]";
            public static string SP_DailylimitCount = "[dbo].[DailylimitCount]";
            public static string SP_TitleCompaignCheck = "[dbo].[TitleCompaignCheck]";
            public static string SP_GetCampaignsByCampaignId = "[dbo].[GetCampaignsByCampaignId]";

        }
        public static class Tenant
        {

            public static string SP_TenantsByIdGet = "[dbo].[TenantsByIdGet]";
            public static string SP_TenantByIdGet = "[dbo].[TenantByIdGet]";
            public static string SP_TenantByIdGetInfo = "[dbo].[TenantByIdGetInfo]";
            public static string SP_TenantsGetById = "[dbo].[TenantsGetById]";
            public static string SP_ConversationMeasurementsAdd = "[dbo].[ConversationMeasurementsAdd]";
            public static string SP_TenantsInfoGet = "[dbo].[TenantsInfoGet]";
            public static string SP_HostTenantsInfoGet = "[dbo].[HostTenantsInfoGet]";
            public static string SP_TenantUpdate = "[dbo].[TenantUpdate]";
            public static string SP_TenantConversationMeasurementUpdate = "[dbo].[TenantConversationMeasurementUpdate]";
            public static string SP_TenantConversationMeasurementGet = "[dbo].[TenantConversationMeasurementGet]";
            public static string SP_UpdateTenantSettings = "[dbo].[UpdateTenantSettings]";
            public static string ZohoCustomerIdsGet = "[dbo].[ZohoCustomerIdsGet]";
            public static string SP_getSettingsTenantHost = "[dbo].[getSettingsTenantHost]";
            public static string SP_GetDailyLimit = "[dbo].[GetDailyLimitByTenantId]";
            public static string SP_RequestMessagesUpdate = "[dbo].[RequestMessagesUpdate]";
            public static string SP_TenantInfoGet = "[dbo].[TenantInfoGet]";
            public static string SP_TenancyName = "[dbo].[TenancyName]";
            public static string SP_GetTenantById = "[dbo].[GetTenantById]";
            public static string sp_GetExportToExcelHost_LastDay = "[dbo].[sp_GetExportToExcelHost_LastDay]";



        }
        public static class Salla
        {
            public static string SP_GetSallaTokenByMerchantId = "[dbo].[GetSallaTokenByMerchantId]";

        }
        public static class ConversationSession
        {
            public static string SP_WhatsAppFreeMessageAdd = "[dbo].[WhatsAppFreeMessageAdd]";
            public static string SP_ConversationOpenSessionGet = "[dbo].[ConversationOpenSessionGet]";
            public static string SP_WhatsAppFreeMessageGet = "[dbo].[WhatsAppFreeMessageGet]";
            public static string SP_WhatsAppFreeMessageGetById = "[dbo].[WhatsAppFreeMessageGetById]";
            public static string SP_WhatsAppFreeMessageDelete = "[dbo].[WhatsAppFreeMessageDelete]";
            public static string SP_WhatsAppScheduledFreeMessageAdd = "[dbo].[WhatsAppScheduledFreeMessageAdd]";
            public static string SP_WhatsAppFreeMessageScheduleValidationAdd = "[dbo].[WhatsAppFreeMessageScheduleValidationAdd]";


        }

        public static class LocationsPinned
        {
            public static string SP_LocationsPinnedNearbyGet = "[dbo].[LocationsPinnedNearbyGet]";
            public static string SP_LocationsNearbyGet = "[dbo].[LocationsNearbyGet]";

        }
        public static class Location
        {
            public static string SP_LocationDeliveryCostGet = "[dbo].[LocationDeliveryCostGet]";
            public static string SP_LocationsAllDeliveryCostGet = "[dbo].[LocationsAllDeliveryCostGet]";
            public static string SP_LocationsGetAll = "[dbo].[LocationsGetAll]";
            public static string SP_LocationsGet = "[dbo].[LocationsGet]";
            public static string SP_LocationAdd = "[dbo].[LocationAdd]";
            public static string SP_LocationsGetRoots = "[dbo].[LocationsGetRoots]";
            public static string SP_LocationsGetCountry = "[dbo].[LocationsGetCountry]";
            public static string SP_LocationsGetByParentId = "[dbo].[LocationsGetByParentId]";
            public static string SP_LocationDeliveryCostDetailsGet = "[dbo].[LocationDeliveryCostDetailsGet]";
            public static string SP_LocationDeliveryCostDetailsAdd = "[dbo].[LocationDeliveryCostDetailsAdd]";
            public static string SP_LocationDeliveryCostDetailsDelete = "[dbo].[LocationDeliveryCostDetailsDelete]";
            public static string SP_LocationDeliveryCostDefaultAdd = "[dbo].[LocationDeliveryCostDefaultAdd]";

        }

        public static class Menu
        {

            public static string SP_SpecificationsCategoryGet = "[dbo].[SpecificationsCategoryGet]";
            public static string SP_MenuDelete = "[dbo].[MenuDelete]";
            public static string SP_MenusGet = "[dbo].[MenusGet]";
            public static string SP_MenuImagesUpdate = "[dbo].[MenuImagesUpdate]";
            public static string SP_MenuReminderMessageAdd = "[dbo].[MenuReminderMessageAdd]";
            public static string SP_MenuReminderMessagesGet = "[dbo].[MenuReminderMessagesGet]";
            public static string SP_MenuReminderMessagesUpdate = "[dbo].[MenuReminderMessagesUpdate]";
            public static string SP_SpecificationConnectedToItemCheck = "[dbo].[SpecificationConnectedToItemCheck]";
            public static string SP_SpecificationDelete = "[dbo].[SpecificationDelete]";



        }
        public static class ItemAdditions
        {
            public static string SP_ItemAdditionsValidationDelete = "[dbo].[ItemAdditionsValidationDelete]";
            public static string SP_ItemAdditionsCategoryDelete = "[dbo].[ItemAdditionsCategoryDelete]";
            public static string SP_ItemAdditionsCategorysGet = "[dbo].[ItemAdditionsCategorysGet]";
            public static string SP_ItemAdditionDelete = "[dbo].[ItemAdditionDelete]";

        }
        public static class Area
        {
            public static string SP_AreasByIdGet = "[dbo].[AreasByIdGet]";
            public static string SP_AreasByTenantIdGet = "[dbo].[AreasByTenantIdGet]";
            public static string SP_AreaDelete = "[dbo].[AreaDelete]";
            public static string SP_AreasGet = "[dbo].[AreasGet]";
            public static string SP_BranchSettingGet = "[dbo].[BranchSettingGet]";

            public static string SP_BranchsGetAll = "[dbo].[BranchsGetAll]";
        }


        public static class Caption
        {
            public static string SP_CaptionGet = "[dbo].[CaptionGet]";
            public static string SP_CaptionGetById = "[dbo].[CaptionGetById]";
            public static string SP_CaptionUpdateById = "[dbo].[CaptionUpdateById]";
        }
        public static class Bot
        {
            public static string SP_BotTemplatesGet = "[dbo].[BotTemplatesGet]";
            public static string SP_BotReservedWordsGet = "[dbo].[BotReservedWordsGet]";
            public static string SP_ActionsGet = "[dbo].[ActionsGet]";
            public static string SP_BotReservedWordsGetById = "[dbo].[BotReservedWordsGetById]";
            public static string SP_BotReservedWordsAdd = "[dbo].[BotReservedWordsAdd]";
            public static string SP_BotReservedWordsDelete = "[dbo].[BotReservedWordsDelete]";
            public static string SP_BotReservedWordsUpdate = "[dbo].[BotReservedWordsUpdate]";
            public static string SP_BotReservedWordByActionIdGet = "[dbo].[BotReservedWordByActionIdGet]";
            public static string Sp_KeyWordGetByTenantId = "[dbo].[KeyWordGetByTenantId]";
            public static string Sp_KeyWordGetByTenantIdInUpdated = "[dbo].[KeyWordGetByTenantIdInUpdate]";
            public static string Sp_KeyWordAdd = "[dbo].[KeyWordAdd]";
            public static string Sp_KeyWordUpdate = "[dbo].[KeyWordUpdate]";
            public static string Sp_KeyWordGetById = "[dbo].[KeyWordGetById]";
            public static string Sp_KeyWordGetAll = "[dbo].[KeyWordGetAll]";
            public static string Sp_KeyWordDelete = "[dbo].[KeyWordDelete]";
        }


        public static class UTrac
        {

            public static string SP_UTracOrderAdd = "[dbo].[UTracOrderAdd]";
            public static string SP_UTracOrderStatusUpdate = "[dbo].[UTracOrderStatusUpdate]";
            public static string UTracLiveBaseUrl = "https://development.utracadmin.net/api/integrators/";
            public static string UTracTestBaseUrl = "https://api.utracadmin.net/api/integrators/";



        }

        public static class TargetReach
        {
            public static string SP_TargetReachMessageAdd = "[dbo].[TargetReachMessageAdd]";
        }
        public static class Depatrment
        {

            public static string SP_DepartmentsGet = "[dbo].[DepartmentsGet]";
            public static string SP_DepartmentUpdate = "[dbo].[DepartmentUpdate]";
            public static string SP_DepartmentByDepartmentIdGet = "[dbo].[DepartmentByDepartmentIdGet]";



        }

        public static class Billing
        {

            public static string SP_BillingGet = "[dbo].[BillingGet]";
            public static string SP_BillingUpdate = "[dbo].[BillingUpdate]";
            public static string SP_BillingAdd = "[dbo].[BillingAdd]";
            public static string SP_ZohoAccessTokenUpdate = "[dbo].[ZohoAccessTokenUpdate]";

        }
        public static class Booking
        {

            public static string SP_BookingsGet = "[dbo].[BookingsGet]";
            public static string SP_BookingByIdGet = "[dbo].[BookingByIdGet]";
            public static string SP_BookingContactGet = "[dbo].[BookingContactGet]";
            public static string SP_BookingAdd = "[dbo].[BookingAdd]";
            public static string SP_BookingUpdate = "[dbo].[BookingUpdate]";
            public static string SP_BookingDelete = "[dbo].[BookingDelete]";
            public static string SP_BookingCurrentNumberGet = "[dbo].[BookingCurrentNumberGet]";
            public static string SP_BookingTemplateCheckGet = "[dbo].[BookingTemplateCheckGet]";
            public static string SP_BookingTemplateGet = "[dbo].[BookingTemplateGet]";
            public static string SP_BookingCapacityCheck = "[dbo].[BookingCapacityCheck]";
            public static string SP_BookingContactAdd = "[dbo].[BookingContactAdd]";
            public static string SP_BookingCapacityGet = "[dbo].[BookingCapacityGet]";
            public static string SP_CaptionByTextResourceIdGet = "[dbo].[CaptionByTextResourceIdGet]";
            public static string SP_BookingUpdateIsNew = "[dbo].[BookingUpdateIsNew]";
            public static string SP_BookingOffDaysGet = "[dbo].[BookingOffDaysGet]";
            public static string SP_BookingOffDaysCreate = "[dbo].[BookingOffDaysCreate]";
            public static string SP_BookingOffDaysUpdate = "[dbo].[BookingOffDaysUpdate]";
            public static string SP_BookingOffDaysDelete = "[dbo].[BookingOffDaysDelete]";

            public static string SP_BookingGetAll = "[dbo].[BookingGetAll]";
            public static string SP_BookingStatisticsGet = "[dbo].[BookingStatisticsGet]";
        }

        public static class TemplateMessage
        {
            public static string SP_TemplateMessageByIdGet = "[dbo].[TemplateMessageByIdGet]";

            //QA 
            public static string SP_GetAllTemplateMessagesNoFilter = "[dbo].[GetAllTemplateMessagesNoFilter]";

        }

        public static class Role
        {
            public static string SP_GetUserRole = "[dbo].[GetUserRole]";
            public static string SP_GetRole = "[dbo].[GetRole]";


        }
        public static class BotFlows
        {
            public static string SP_BotFlowsCreate = "[dbo].[BotFlowsCreate]";
            public static string SP_BotFlowsDelete = "[dbo].[BotFlowsDelete]";
            public static string SP_BotFlowsGetAll = "[dbo].[BotFlowsGetAll]";
            public static string SP_BotFlowsGetByTempId = "[dbo].[BotFlowsGetByTempId]";
            public static string SP_BotFlowsGetById = "[dbo].[BotFlowsGetById]";
            public static string SP_BotFlowUpdateJson = "[dbo].[BotFlowUpdateJson]";
            public static string SP_BotFlowsUpdateIsPublished = "[dbo].[BotFlowsUpdateIsPublished]";
            public static string SP_BotParametrsCreate = "[dbo].[BotParametersCreate]";
            public static string SP_BotParametrsDelete = "[dbo].[BotParametersDeleteById]";
            public static string SP_BotParametrsGetAll = "[dbo].[BotParametersGetAll]";
        }

        public static class Wallet
        {
            public static string SP_WalletCreate = "[dbo].[WalletCreate]";
            public static string SP_WalletAdd = "[dbo].[WalletAdd]";
            public static string SP_WalletDeposit = "[dbo].[WalletDeposit]";
            public static string SP_WalletGet = "[dbo].[WalletGet]";
        }
        public static class Transaction
        {
            public static string SP_TransactionAdd = "[dbo].[TransactionAdd]";
            public static string SP_TransactionGetLastFour = "[dbo].[TransactionGetLastFour]";
            public static string SP_TransactionGetByInvoceId = "[dbo].[TransactionGetByInvoceId]";
            public static string SP_UpdeteTransaction = "[dbo].[UpdeteTransaction]";
            public static string SP_CampaignMinusPrice = "[dbo].[CampaignMinusPrice]";
        }
        public static class Country
        {
            public static string SP_CountryCodeGet = "[dbo].[CountryCodeGet]";
            public static string SP_CountryGetAll = "[dbo].[CountryGetAll]";
            public static string SP_CountryISOCodeGet = "[dbo].[CountryISOCodeGet]";


        }
        public static class User
        {
            public static string SP_UsersGetInfo = "[dbo].[UsersGetInfo]";
            public static string SP_UsersGetAll = "[dbo].[UsersGetAll]";
            public static string SP_GetUserByName = "[dbo].[GetUserByName]";
            public static string SP_GetUsersByIds = "[dbo].[GetUsersByIds]";



        }
        public static class RZIntegration
        {

            //RZ Integration AR
            public static string SP_GetItemByPath = "[dbo].[GetRZItemsByPath]";
            public static string SP_GetItemCode = "[dbo].[GetRZItemCode]";

            public static string SP_GetItemByPath2 = "[dbo].[GetRZItemsByPath2]";
            public static string SP_GetItemCode2 = "[dbo].[GetRZItemCode2]";

            //RZ Integration EN
            public static string SP_GetItemByPath_EN = "[dbo].[GetRZItemsByPath_EN]";
            public static string SP_GetRZItemCode_EN = "[dbo].[GetRZItemCode_EN]";

            public static string SP_GetItemByPath_EN2 = "[dbo].[GetRZItemsByPath_EN2]";
            public static string SP_GetRZItemCode_EN2 = "[dbo].[GetRZItemCode_EN2]";


        }
        public static class Groups
        {
            public static string SP_GroupsGetAll = "[dbo].[GroupsGetAll]";
            public static string SP_GroupsLogGetAll = "[dbo].[GroupsLogGetAll]";

            public static string SP_GroupsGetAllNew = "[dbo].[GroupsGetAllNew]";
            public static string SP_GroupsGetAllPerUser = "[dbo].[GroupsGetAllPerUser]";

            public static string SP_GroupsGetById = "[dbo].[GroupsGetById]";
            public static string SP_GroupsGetByIdNew = "[dbo].[GroupsGetByIdNew]";

            public static string SP_MembersGetAll = "[dbo].[MembersGetAll]";
            public static string SP_GroupDelete = "[dbo].[GroupDelete]";
            public static string SP_GroupsCreate = "[dbo].[GroupsCreate]";
            public static string SP_GroupsCreateNew = "[dbo].[GroupsCreateNew]";
            public static string SP_GroupsSetQueueDB = "[dbo].[GroupsSetQueueDB]";
            public static string SP_AddMembersOnGroup = "[dbo].[AddMembersOnGroup]";
            public static string SP_ContactFilter = "[dbo].[ContactFilter]";
            public static string SP_GroupsUpdate = "[dbo].[GroupsUpdate]";
            public static string SP_GroupCheck = "[dbo].[GroupCheck]";

            public static string SP_RemoveMembers = "[dbo].[RemoveMembers]";
            public static string SP_MembersGetAllForCamp = "[dbo].[MembersGetAllForCamp]";
            public static string SP_MovingMembersFromGroup = "[dbo].[MovingMembersFromGroup]";
        }
        public static class Teams
        {
            public static string SP_TeamsGetAll = "[dbo].[TeamsGetAll]";
            public static string SP_TeamsDelete = "[dbo].[TeamsDelete]";
            public static string SP_TeamsCreate = "[dbo].[TeamsCreate]";
            public static string SP_TeamsEdit = "[dbo].[TeamsEdit]";
        }
    }
}