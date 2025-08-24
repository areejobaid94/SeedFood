using System;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto
{
    public class RecentTenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int ReadConversationCount { get; set; }
        public int TotalOfConversation { get; set; }
        public int NewConversationCount { get; set; }
        public int SendMessagesCount { get; set; }
        public int ReceivedMessagesCount { get; set; }

        public int TotalNumberOfCustomers { get; set; }
        public int TotalOfClose { get; set; }
    }
}