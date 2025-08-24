using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
   public class AppSettingsModel
    {
        public static string ConnectionStrings { get; set; }

        public static string EngineAPIURL { get; set; }
        public static string BlobServiceConnectionStrings { get; set; }

        public static string GoogleMapsKey { get; set; }

        public static string TranslateKey { get; set; }
        public static int AddHour { get; set; }
        public static int DivHour { get; set; }
        public static string BotApi { get; set; }

        public static string connectionStringMongoDB { get; set; }
        public static string collectionName { get; set; }
        public static string databaseName { get; set; }


        public static string urlSendCampaignProject { get; set; }
        public static string googleSheetClientId { get; set; }
        public static string googleSheetClientSecret { get; set; }
        public static string googleSheetRedirectUri { get; set; }


        public static string EngineApi { get; set; }
    }
}
