using System.Collections.Generic;
using MvvmHelpers;
using Infoseed.MessagingPortal.Models.NavigationMenu;

namespace Infoseed.MessagingPortal.Services.Navigation
{
    public interface IMenuProvider
    {
        ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}