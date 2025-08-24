using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class AppSettingsCoreModel
    {
        public static string ConnectionStrings { get; set; }
        public static string BlobServiceConnectionStrings { get; set; }
        public static string StorageServiceURL { get; set; }

        public static string GoogleMapsKey { get; set; }


        public static string AzureWebJobsStorage { get; set; }
        public static string SocketIOURL { get; set; }
        public static string SocketIOToken { get; set; }

    }

   
}
