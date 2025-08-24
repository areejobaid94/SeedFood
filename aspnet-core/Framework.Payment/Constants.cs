namespace Framework.Payment
{
    internal class Constants
    {
        
        //public static string ConnectionString =System.Configuration.ConfigurationManager.AppSettings["Storage.DB"];
        /// <summary>
        /// TODO: move this settings into database table
        /// </summary>
        //public static string GRPServiceBaseUrl = System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.APIBaseUri"];
        //public static string IPAASOuthUri = System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.IPAASOuthUri"];
        //public static string ClientID = System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.ClientID"];
        //public static string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.ClientSecret"];
        //public static bool RemoteCertificateValidationCallback =bool.Parse(System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.RemoteCertificateValidationCallback"]);
        //public static bool IsDevelopmentEnv = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.IsDevelopmentEnv"]);
        // public static string CertificatePath =System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.CertificatePath"];
        //public static string PublicKey =System.Configuration.ConfigurationManager.AppSettings["GRPIntegration.PublicKey"];

        public static string SP_ActionLogInsert = "[Integration].[ActionLogInsert]";
        public static string SP_ActionLogUpdate = "[Integration].[ActionLogUpdate]";

        public class EntryResult
        {
            public const string Success = "S";
            public const string Error = "E";
        }
       
    }
}
