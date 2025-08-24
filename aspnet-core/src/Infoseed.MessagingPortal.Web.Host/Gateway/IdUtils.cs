using System;

namespace Infoseed.MessagingPortal.Web.Host.Utils
{
    public class IdUtils
    {
        public static string generateSampleId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 10);
        }
    }
}
