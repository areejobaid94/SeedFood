using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models
{
    public class SettingsModel
    {
        public static string ConnectionStrings { get; set; }
        public static string BlobServiceConnectionStrings { get; set; }
        public static string GoogleMapsKey { get; set; }

        public static string AzureWebJobsStorage { get; set; }

        public static string MgKey { get; set; }

        public static string MgUrl { get; set; }
        public static string GoogleAuth { get; set; }

    }
}
