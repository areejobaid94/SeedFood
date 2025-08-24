using System;

namespace Infoseed.MessagingPortal.AccountBillings
{
    public  class ActiveContactPerMonthModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime LastMessageDateTime { get; set; }

        public int ContactID { get; set; }
    }
}
