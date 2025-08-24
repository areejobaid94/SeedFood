﻿using System.Threading.Tasks;
using Abp.Webhooks;

namespace Infoseed.MessagingPortal.WebHooks
{
    public interface IWebhookEventAppService
    {
        Task<WebhookEvent> Get(string id);
    }
}
