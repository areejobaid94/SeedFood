using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models
{
    public class AppsettingsModel
    {
        public static Dictionary<string, string> AttacmentTypesAllowed { get; set; }
        public static Dictionary<string, string> AttacmentTypesAllowedM { get; set; }

        public static string ConnectionStrings { get; set; }
      

 
        public static string StorageServiceConnectionStrings { get; set; }
        public static  string StorageServiceURL { get; set; }
    }
   



}
