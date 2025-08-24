using System.ComponentModel;
using Abp.AutoMapper;
using Infoseed.MessagingPortal.MultiTenancy.Dto;

namespace Infoseed.MessagingPortal.Models.Tenants
{
    [AutoMapFrom(typeof(TenantEditDto))]
    public class TenantEditModel : TenantEditDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}