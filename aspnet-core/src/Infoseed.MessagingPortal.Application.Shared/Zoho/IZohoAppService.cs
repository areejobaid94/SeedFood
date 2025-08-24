using Abp.Application.Services;
using Infoseed.MessagingPortal.Zoho.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Zoho
{
    public interface IZohoAppService : IApplicationService
    {
        StatementsModel GetStatementsFillter(string fillter);
        InvoicesModel InvoicesGet( int pageNumber, int pageSize);
        ZohoContactsModel GetContacts(int? TenantId);
        GenerateAccessTokenModel GenerateAccessToken(string code);
        Task SyncBillingAsync(int pageNumber, int pageSize, int? tenantId = null);
    }
}
