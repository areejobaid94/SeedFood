using Infoseed.MessagingPortal.Models.Tenants;
using Infoseed.MessagingPortal.ViewModels;
using Xamarin.Forms;

namespace Infoseed.MessagingPortal.Views
{
    public partial class TenantsView : ContentPage, IXamarinView
    {
        public TenantsView()
        {
            InitializeComponent();
        }

        private async void ListView_OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            await ((TenantsViewModel)BindingContext).LoadMoreTenantsIfNeedsAsync(e.Item as TenantListModel);
        }
    }
}