using Infoseed.MessagingPortal.Models.Users;
using Infoseed.MessagingPortal.ViewModels;
using Xamarin.Forms;

namespace Infoseed.MessagingPortal.Views
{
    public partial class UsersView : ContentPage, IXamarinView
    {
        public UsersView()
        {
            InitializeComponent();
        }

        public async void ListView_OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            await ((UsersViewModel) BindingContext).LoadMoreUserIfNeedsAsync(e.Item as UserListModel);
        }
    }
}