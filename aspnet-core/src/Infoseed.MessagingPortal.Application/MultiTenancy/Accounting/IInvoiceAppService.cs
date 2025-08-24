using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Accounting.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
