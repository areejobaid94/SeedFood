using Abp.Application.Services;
using Infoseed.MessagingPortal.BotFlow.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Wallet
{
    public interface IWalletAppService : IApplicationService
    {
        void CreateWallet(int TenantId);
    }
}
