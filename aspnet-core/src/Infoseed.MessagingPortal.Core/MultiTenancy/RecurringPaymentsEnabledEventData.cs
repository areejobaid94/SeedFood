using Abp.Events.Bus;

namespace Infoseed.MessagingPortal.MultiTenancy
{
    public class RecurringPaymentsEnabledEventData : EventData
    {
        public int TenantId { get; set; }
    }
}