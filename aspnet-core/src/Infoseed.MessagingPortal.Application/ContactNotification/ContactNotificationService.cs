using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ContactNotification
{
    public class ContactNotificationService : MessagingPortalAppServiceBase, IContactNotification
    {
        private readonly IRepository<ContactNotification> _contactNotification;

        public ContactNotificationService(IRepository<ContactNotification> contactNotification)
        {
            this._contactNotification = contactNotification;
        }
        public async Task CreateContactNotificationAsync(int? tenantId, string contactId, string notificationId, DateTime notificationCreateDate, string notificationText, int userId)
        {
            ContactNotification tenantInformation = new ContactNotification
            {
                TenantId = tenantId,
                ContactId = contactId,
                NotificationId = notificationId,
                NotificationCreateDate = notificationCreateDate,
                NotificationText = notificationText,              
                UserId = userId

            };
            _contactNotification.Insert(tenantInformation);
        }

        public async Task DeleteAsync(int? tenantId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ContactNotification>> GetContactNotificationAsync(int? tenantId, string contactId)
        {
            var contactNotification = _contactNotification.GetAllList().Where(x => x.TenantId == tenantId && x.ContactId== contactId).ToList();
            return contactNotification;
        }

        public async Task UpdateContactNotificationAsync(int? tenantId, string contactId, string notificationId, DateTime notificationCreateDate, string notificationText, int userId)
        {
             var contactNotification = _contactNotification.GetAllList().Where(x => x.TenantId == tenantId && x.ContactId == contactId && x.NotificationId== notificationId).FirstOrDefault();

            if (contactNotification != null)
            {

                contactNotification.TenantId = tenantId;
                contactNotification.ContactId = contactId;
               contactNotification.NotificationId = notificationId;
                contactNotification.NotificationCreateDate = notificationCreateDate;
               contactNotification.NotificationText = notificationText;             
               contactNotification.UserId = userId;

                var list = _contactNotification.UpdateAsync(contactNotification);
            }
        }
    }
}
