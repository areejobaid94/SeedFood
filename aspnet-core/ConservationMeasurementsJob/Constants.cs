using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConservationMeasurementsJob
{
   
       
   

    internal class Constants
    {

        public static string SP_ConservationMeasurementUpdate = "[dbo].[ConversationMeasurementUpdate]";
        public static string SP_ConversationBundleValidateGet = "[dbo].[ConversationBundleValidateGet]";
        public static string SP_ContactsCampaignMessageUpdate = "[dbo].[ContactsCampaignMessageUpdate]";




        //internal static IConfiguration Configuration = new ConfigurationBuilder()
        //                 .AddConfiguration("appsettings.json"/*, optional: true, reloadOnChange: true*/)
        //                 //.AddEnvironmentVariables()
        //                 //.AddCommandLine(args)
        //                 .Build();


        //public static class AzureConfiguration
        //{
        //    public static string AzureConnectionString = System.Configuration.ConfigurationManager.AppSettings["AzureStorageConnectionString"];
        //    public static string AzureBlobConnectionString = System.Configuration.ConfigurationManager.AppSettings["BlobStorageConnectionString"];
        //}

        public static class DocumentDBConfiguration
        {
            public static string EndPoint = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsEndPoint"];
            public static string AuthKey = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsAuthKey"];
            public static string Database = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsDatabase"];
            public static string TemplateDefinitionCollection = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsTemplateDefinitionCollection"];
            public static string TemplateValuesCollection = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsTemplateValuesCollection"];
            public static string ItemsCollection = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsItemsCollection"];
            public static string DMSCollection = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsDMSCollection"];
            public static string ConversationsCollection = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsConversationsCollection"];
            public static string CustomersCollection = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsCustomersCollection"];
            public static string UsersCollection = System.Configuration.ConfigurationManager.AppSettings["CosmosDBSettingsUsersCollection"];
        }
    }


}
