using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.CaptionBot
{
    public interface ICaptionBot
    {
        Task<Caption> GetCaptionAsync(int? tenantId);
    }
}
