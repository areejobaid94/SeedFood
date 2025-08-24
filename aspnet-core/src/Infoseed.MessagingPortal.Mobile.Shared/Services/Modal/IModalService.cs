using System.Threading.Tasks;
using Infoseed.MessagingPortal.Views;
using Xamarin.Forms;

namespace Infoseed.MessagingPortal.Services.Modal
{
    public interface IModalService
    {
        Task ShowModalAsync(Page page);

        Task ShowModalAsync<TView>(object navigationParameter) where TView : IXamarinView;

        Task<Page> CloseModalAsync();
    }
}
