namespace InfoSeedAzureFunction
{
    public static class Constants
    {
        public static string SP_ConservationMeasurementUpdate = "[dbo].[ConversationMeasurementUpdate]";
        public static string SP_ConversationBundleValidateGet = "[dbo].[ConversationBundleValidateGet]";
        public static string SP_ContactsCampaignMessageUpdate = "[dbo].[ContactsCampaignMessageUpdate]";
        public static class Contacts
        {
            public static string SP_ContactsAdd = "[dbo].[ContactsAdd]";

            public static string SP_ContactbyIdGet = "[dbo].[ContactbyIdGet]";
            public static string SP_ContactUpdate = "[dbo].[ContactUpdate]";
            public static string SP_ContactsExternalBulkAdd = "[dbo].[ContactsExternalBulkAdd]";
            public static string SP_ContactsExternalGet = "[dbo].[ContactsExternalGet]";
            public static string SP_ContactsExternalByCampaignGet = "[dbo].[ContactsExternalByCampaignGet]";
            public static string SP_ContactsCampaignlBulkAdd = "[dbo].[ContactsCampaignBulkAdd]";
            public static string SP_ContactsFreeMessageBulkAdd = "[dbo].[ContactsFreeMessageBulkAdd]";
            public static string SP_ContactsInterrestedOfAdd = "[dbo].[ContactsInterrestedOfAdd]";
            public static string SP_CustomerBehaviourUpdate = "[dbo].[CustomerBehaviourUpdate]";
            public static string SP_ContactsInterestedOfGet = "[dbo].[ContactsInterestedOfGet]";
            public static string SP_ContactCheckIfExistGet = "[dbo].[ContactCheckIfExistGet]";
            public static string SP_ContactsCampaignlBulkAddNew = "[dbo].[ContactsCampaignBulkAddNew]";
        }
        public static class Dashboard
        {
            public static string SP_ConversationMeasurementsGet = "[dbo].[ConversationMeasurementsGet]";
            public static string SP_ConversationMeasurementBIUpdate = "[dbo].[ConversationMeasurementBIUpdate]";

        }
        public static class LiveChat
        {
            public static string SP_TicketsStatisticsGet = "[dbo].[TicketsStatisticsGet]";
            public static string SP_GetAllTenantStatisticsToExportExcel = "[dbo].[GetAllTenantStatisticsToExportExcel]";
        }
        public static class Booking
        {

            public static string SP_BookingsGet = "[dbo].[BookingsGet]";
            public static string SP_BookingByIdGet = "[dbo].[BookingByIdGet]";
            public static string SP_BookingAdd = "[dbo].[BookingAdd]";
            public static string SP_BookingUpdate = "[dbo].[BookingUpdate]";
            public static string SP_BookingDelete = "[dbo].[BookingDelete]";
            public static string SP_BookingCurrentNumberGet = "[dbo].[BookingCurrentNumberGet]";
            public static string SP_BookingTemplateCheckGet = "[dbo].[BookingTemplateCheckGet]";
            public static string SP_BookingTemplateGet = "[dbo].[BookingTemplateGet]";
            public static string SP_BookingCapacityCheck = "[dbo].[BookingCapacityCheck]";
            public static string SP_BookingContactAdd = "[dbo].[BookingContactAdd]";
            public static string SP_BookingConfirmedGet = "[dbo].[BookingConfirmedGet]";
            public static string SP_BookingReminderHourGet = "[dbo].[BookingReminderHourGet]";
            public static string SP_BookingReminderSentUpdate = "[dbo].[BookingReminderSentUpdate]";

        }

        public static class WhatsAppTemplates //
        {
            public static string SP_WhatsAppTemplatesAdd = "[dbo].[WhatsAppTemplateAdd]";
            public static string SP_WhatsAppTemplatesDelete = "[dbo].[WhatsAppTemplateDelete]";
            public static string SP_WhatsAppTemplatesGet = "[dbo].[WhatsAppMessageTemplateGet]";
            public static string SP_WhatsAppTemplatesGetById = "[dbo].[WhatsAppMessageTemplateGetById]";
            public static string SP_TemplateGetByWhatsAppId = "[dbo].[TemplateGetByWhatsAppId]";
            public static string SP_WhatsAppTemplateByNameGet = "[dbo].[WhatsAppTemplateByNameGet]";

        }
        public static class ConversationSession
        {
            public static string SP_WhatsAppMessageAdd = "[dbo].[WhatsAppMessageAdd]";
            public static string SP_ConversationOpenSessionGet = "[dbo].[ConversationOpenSessionGet]";
            public static string SP_WhatsAppMessageGet = "[dbo].[WhatsAppMessageGet]";
            public static string SP_WhatsAppMessageGetById = "[dbo].[WhatsAppMessageGetById]";
            public static string SP_WhatsAppMessageDelete = "[dbo].[WhatsAppMessageDelete]";


        }
        public static class WhatsAppCampaign //
        {
            public static string SP_WhatsAppCampaignAdd = "[dbo].[WhatsAppCampaignAdd]";
            public static string SP_WhatsAppCampaignDelete = "[dbo].[WhatsAppCampaignDelete]";
            public static string SP_WhatsAppCampaignUpdate = "[dbo].[WhatsAppCampaignUpdate]";
            public static string SP_WhatsAppFreeMessageUpdate = "[dbo].[WhatsAppFreeMessageUpdate]";
            public static string SP_WhatsAppCampaignGet = "[dbo].[WhatsAppCampaignGet]";
            public static string SP_WhatsAppContactFilterCountGet = "[dbo].[WhatsAppContactFilterCountGet]";
            public static string SP_WhatsAppContactFilterGet = "[dbo].[WhatsAppContactFilterGet]";
            public static string SP_WhatsAppScheduledCampaignAdd = "[dbo].[WhatsAppScheduledCampaignAdd]";
            public static string SP_WhatsAppScheduledCampaignGet = "[dbo].[WhatsAppScheduledCampaignGet]";
            public static string SP_WhatsAppScheduledFreeMessageGet = "[dbo].[WhatsAppScheduledFreeMessageGet]";
            public static string SP_WhatsAppMessageCampaignHistory = "[dbo].[WhatsAppMessageCampaignHistory]";
            public static string SP_WhatsAppContactCountFilterGet = "[dbo].[WhatsAppContactCountFilterGet]";
            public static string SP_WhatsAppScheduledDashCampaignGet = "[dbo].[WhatsAppScheduledDashCampaignGet]";


            public static string SP_ContactsFailedCampaignBulkAdd = "[dbo].[ContactsFailedCampaignBulkAdd]";
            public static string SP_DailyLimitByTenantId = "[dbo].[DailyLimitByTenantId]";
        }

        public static class TargetReach //
        {
            public static string SP_TargetReachMessageBulkAdd = "[dbo].[TargetReachMessageBulkAdd]";

        }
        public static class Menu
        {
            public static string SP_MenuReminderMessagesGet = "[dbo].[MenuReminderMessagesGet]";
            public static string SP_MenuReminderMessagesUpdate = "[dbo].[MenuReminderMessagesUpdate]";

        }
        public static class User
        {
            public static string SP_UsersGetInfo = "[dbo].[UsersGetInfo]";
        }
        public static class Transaction
        {
            public static string SP_GetTransactionsForWalletJob = "[dbo].[GetTransactionsForWalletJob]";
            public static string SP_GetTransactionSucssesJob = "[dbo].[GetTransactionSucssesJob]";

        }
        public static class Campaign
        {
            public static string SP_GetCampaignName = "[dbo].[GetCampaignName]";
        }
        public static class Country
        {
            public static string SP_CountryISOCodeGet = "[dbo].[CountryISOCodeGet]";
        }
        public static class Wallet
        {
            public static string SP_WalletGet = "[dbo].[WalletGet]";
            public static string SP_ConversationMinus = "[dbo].[ConversationMinus]";
            public static string SP_GetAllWalletJob = "[dbo].[GetAllWalletJob]";
            public static string SP_WalletAndTransUpdateJob = "[dbo].[WalletAndTransUpdateJob]";
        }
        public static class Tenant
        {
            public static string SP_TenantGetInfo = "[dbo].[TenantGetInfo]";
            public static string SP_TenantUpdateDailyLimit = "[dbo].[TenantUpdateDailyLimit]";
            public static string SP_HostTenantsInfoGet = "[dbo].[HostTenantsInfoGet]";

        }
        public static double AddHour = 3;// double.Parse(System.Environment.GetEnvironmentVariable("InfoSeedTime:AddHour"));

        public static string TemplateDefinitionCollection = System.Environment.GetEnvironmentVariable("CosmosDBSettings:TemplateDefinitionCollection");
        public static string TemplateValuesCollection = System.Environment.GetEnvironmentVariable("CosmosDBSettings:TemplateValuesCollection");
        public static string UsersCollection = System.Environment.GetEnvironmentVariable("CosmosDBSettings:UsersCollection");

        public static string ItemsCollection = "Items";//System.Environment.GetEnvironmentVariable("CosmosDBSettings:ItemsCollection");
        public static string DMSCollection = System.Environment.GetEnvironmentVariable("CosmosDBSettings:DMSCollection");

        public static string ConversationsCollection = "Conversations"; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:ConversationsCollection");
        public static string CustomersCollection = "CustomersCollection"; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:CustomersCollection");




        //public static string ConnectionString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";// System.Environment.GetEnvironmentVariable("ConnectionStrings:Default");
        //public static string EndPoint = "https://infoseed-cosmosdb-prod.documents.azure.com:443/";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:EndPoint");
        //public static string AuthKey = "oQCAv3tSssTwitSya2dthggCXbtWfPtLbiHrLN6cmf0i5HryI5aMVPRkKcyxzk9vbQZKNxL9HJXeF0tTMsk4Jw=="; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:AuthKey");
        //public static string Database = "teaminbox";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:Database");
        //public static string BotApi = "https://infoseedbotapi-prod.azurewebsites.net/";


        //public static string ConnectionString = "Server=tcp:info-seed-db-server-qa.database.windows.net,1433;Initial Catalog=InfoSeedDB-QA;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";// System.Environment.GetEnvironmentVariable("ConnectionStrings:Default");
        //public static string EndPoint = "https://teaminbox.documents.azure.com:443/";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:EndPoint");
        //public static string AuthKey = "V0fNg9dCGFkYRxDC7Ffdh9FCzTFTcN7GWEYQE6QJgjg7tAfpePQzzAUskz5jDMlJT65CYNu1oCOwACDbU7NnAw=="; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:AuthKey");
        //public static string Database = "InfoSeed";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:Database");
        //public static string BotApi = "https://infoseedbotapiqa.azurewebsites.net/";


        public static string ConnectionString = "Server=tcp:info-seed-db-server-stg.database.windows.net,1433;Initial Catalog=InfoSeedDB-Stg;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";// System.Environment.GetEnvironmentVariable("ConnectionStrings:Default");
        public static string AuthKey = "oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8GesNqaqfk2Xk5WlyBvmaInirAQkkOqnFX6W3lU829LHYod5xpA=="; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:AuthKey");
        public static string Database = "InfoSeed";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:Database");
        public static string EndPoint = "https://infoseed-cosmosdb-stg.documents.azure.com:443/";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:EndPoint");
        public static string BotApi = "https://infoseedbotapistg.azurewebsites.net/";

    }



    public static class Constant
    {

        public static string MgMotorKey = "pat-na1-0772c044-8365-4ce5-95be-529519d19f41";
        public static string MgMotorbaseUrl = "https://api.hubapi.com/";
        public static string WhatsAppApiUrl = "https://graph.facebook.com/v17.0/";
        public static string WhatsAppApi360DailogUrl = "https://waba-sandbox.360dialog.io/v1/messages";
        public static double AddHour = 3;
        public static string ItemsCollection = "Items";//System.Environment.GetEnvironmentVariable("CosmosDBSettings:ItemsCollection");
        public static string ConversationsCollection = "Conversations"; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:ConversationsCollection");
        public static string CustomersCollection = "CustomersCollection"; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:CustomersCollection");
        public static string SubscriptionID = "4408da24-aad7-4802-a1f1-46f5ad9e2074";



        /// <summary>
        /// /prod
        /// </summary>
        //public static string EndPoint = "https://infoseed-cosmosdb-prod.documents.azure.com:443/";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:EndPoint");
        //public static string AuthKey = "oQCAv3tSssTwitSya2dthggCXbtWfPtLbiHrLN6cmf0i5HryI5aMVPRkKcyxzk9vbQZKNxL9HJXeF0tTMsk4Jw=="; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:AuthKey");  
        //public static string ConnectionString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //public static string Database = "teaminbox";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:Database");
        //public static string SocketIOToken = "2bb33e3ae845db0b32dd1c5efdd9f35c";
        //public static string SocketIOURL = "https://infoseedsocketioserver-prod.azurewebsites.net/";
        //public static string AzureStorageAccount = "DefaultEndpointsProtocol=https;AccountName=infoseedmediastorageprod;AccountKey=H9ZtPbCTB32FioP7waMAdrGV02pRFPeCTyyFPq3+z8Zz2Py0qxFM3upLGhwRXj6jU9MTSxZAGFke/6zGOhW5MA==;EndpointSuffix=core.windows.net";// System.Environment.GetEnvironmentVariable("Values:AzureWebJobsStorage");     //AppSettingsModel.BlobServiceConnectionStrings;
        //public static string EngineAPIURL = "https://infoseedengineapi-prod.azurewebsites.net/";// System.Environment.GetEnvironmentVariable("Values:AzureWebJobsStorage");     //AppSettingsModel.BlobServiceConnectionStrings;

        /// <summary>
        /// /QA
        /// </summary>
        //public static string EndPoint = "https://teaminbox.documents.azure.com:443/";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:EndPoint");
        //public static string AuthKey = "V0fNg9dCGFkYRxDC7Ffdh9FCzTFTcN7GWEYQE6QJgjg7tAfpePQzzAUskz5jDMlJT65CYNu1oCOwACDbU7NnAw=="; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:AuthKey");  
        //public static string ConnectionString = "Server=tcp:info-seed-db-server-qa.database.windows.net,1433;Initial Catalog=InfoSeedDB-QA;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //public static string Database = "InfoSeed";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:Database");
        //public static string SocketIOToken = "a3901a01-e947-46d3-b347-b081a2fd1230";
        //public static string SocketIOURL = "https://infoseedsocketioserverqa.azurewebsites.net/";
        //public static string AzureStorageAccount = "DefaultEndpointsProtocol=https;AccountName=infoseedmediastorageqa;AccountKey=JS3p9lX/nSALmF2TLvn8fWZWX3wRXjUkmajon7srbM93DKeSWblZJfuk0664Qbp/sw0lEPigVokg+AStkRNt1Q==;EndpointSuffix=core.windows.net";// System.Environment.GetEnvironmentVariable("Values:AzureWebJobsStorage");     //AppSettingsModel.BlobServiceConnectionStrings;
        //public static string EngineAPIURL = "https://infoseedengineapi-qa.azurewebsites.net/";// System.Environment.GetEnvironmentVariable("Values:AzureWebJobsStorage");     //AppSettingsModel.BlobServiceConnectionStrings;


        /// <summary>
        /// stg
        /// </summary>
        public static string EndPoint = "https://infoseed-cosmosdb-stg.documents.azure.com:443/";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:EndPoint");
        public static string AuthKey = "oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8GesNqaqfk2Xk5WlyBvmaInirAQkkOqnFX6W3lU829LHYod5xpA=="; //System.Environment.GetEnvironmentVariable("CosmosDBSettings:AuthKey");
        public static string ConnectionString = "Server=tcp:info-seed-db-server-stg.database.windows.net,1433;Initial Catalog=InfoSeedDB-Stg;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public static string Database = "InfoSeed";// System.Environment.GetEnvironmentVariable("CosmosDBSettings:Database");
        public static string SocketIOToken = "313cb6e6-caad-4457-a0e4-669415d93250";
        public static string SocketIOURL = "https://infoseedsocketioserverstg.azurewebsites.net/";
        public static string AzureStorageAccount = "DefaultEndpointsProtocol=https;AccountName=infoseedstoragestg;AccountKey=sc089/Ku+IUBAbCwGnlumuK72RultGBqL/TwHS36SJHlCx3uC9dtEKjJJJHPRiRrAMwrIs2FyP6Z+ASt8j6gWg==;EndpointSuffix=core.windows.net";// System.Environment.GetEnvironmentVariable("Values:AzureWebJobsStorage");     //AppSettingsModel.BlobServiceConnectionStrings;                                                                                                                                                                                                                                              //  public static string EngineAPIURL = "https://localhost:44310/";// System.Environment.GetEnvironmentVariable("Values:AzureWebJobsStorage");     //AppSettingsModel.BlobServiceConnectionStrings;
        public static string EngineAPIURL = "https://infoseedengineapi-stg.azurewebsites.net/";






        //public static string ZohoClientID = "1000.R9KLA1VR11Z4NJRJ9BV9UDZ30HMIOH";
        //public static string ZohoClientSecret = "f5e807d73d42317a918592a1d49be99fc0143b4cae";
        //public static string ZohoRedirectUri = "https://waapi.info-seed.com/";

        public static string ZohoClientID = "1000.IP8N0O0U8F844MS2WH2NJGL9QBF8XR";
        public static string ZohoClientSecret = "4cc4c9d9f0c6c408401b8c201832a662efbfe3be22";
        public static string ZohoRedirectUri = "https://teaminbox-stg.azurewebsites.net/";



    }
}
