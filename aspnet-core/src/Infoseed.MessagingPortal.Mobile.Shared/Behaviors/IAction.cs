using Xamarin.Forms.Internals;

namespace Infoseed.MessagingPortal.Behaviors
{
    [Preserve(AllMembers = true)]
    public interface IAction
    {
        bool Execute(object sender, object parameter);
    }
}