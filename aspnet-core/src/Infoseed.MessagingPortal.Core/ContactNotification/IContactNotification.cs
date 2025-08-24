using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ContactNotification
{
    public interface IContactNotification
    {
        Task<List<ContactNotification>> GetContactNotificationAsync(int? tenantId , string contactId);
        Task CreateContactNotificationAsync(int? tenantId, string contactId , string notificationId, DateTime notificationCreateDate, string notificationText, int userId);
        Task UpdateContactNotificationAsync(int? tenantId, string contactId, string notificationId, DateTime notificationCreateDate, string notificationText, int userId);

        Task DeleteAsync(int? tenantId);
    }
}
