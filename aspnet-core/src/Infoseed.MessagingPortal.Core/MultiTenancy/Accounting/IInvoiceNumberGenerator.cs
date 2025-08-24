using System.Threading.Tasks;
using Abp.Dependency;

namespace Infoseed.MessagingPortal.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}