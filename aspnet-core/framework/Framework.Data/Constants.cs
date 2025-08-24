using Microsoft.Extensions.Configuration;

namespace Framework.Data
{
    internal class Constants
    {
       internal static IConfiguration Configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json"/*, optional: true, reloadOnChange: true*/)
                        //.AddEnvironmentVariables()
                        //.AddCommandLine(args)
                        .Build();

        public static class AzureConfiguration
        {
            public static string AzureConnectionString = Configuration["AzureStorageConnectionString"];
            public static string AzureBlobConnectionString = Configuration["BlobStorageConnectionString"];
        }

        public static class DocumentDBConfiguration
        {
            public static string EndPoint = Configuration["CosmosDBSettings:EndPoint"];
            public static string AuthKey = Configuration["CosmosDBSettings:AuthKey"];
            public static string Database = Configuration["CosmosDBSettings:Database"];
            public static string TemplateDefinitionCollection = Configuration["CosmosDBSettings:TemplateDefinitionCollection"];
            public static string TemplateValuesCollection = Configuration["CosmosDBSettings:TemplateValuesCollection"];
            public static string ItemsCollection = Configuration["CosmosDBSettings:ItemsCollection"];
            public static string DMSCollection = Configuration["CosmosDBSettings:DMSCollection"];
            public static string ConversationsCollection = Configuration["CosmosDBSettings:ConversationsCollection"];
            public static string CustomersCollection = Configuration["CosmosDBSettings:CustomersCollection"];
            public static string UsersCollection = Configuration["CosmosDBSettings:UsersCollection"];
        }
    }
}
