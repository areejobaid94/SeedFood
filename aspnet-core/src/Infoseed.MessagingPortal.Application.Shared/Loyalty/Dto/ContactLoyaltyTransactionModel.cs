using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Loyalty.Dto
{
    public class ContactLoyaltyTransactionModel
    {
        public int ContactId { get; set; }
        public decimal Points { get; set; }
        public long OrderId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long LoyaltyDefinitionId { get; set; }
        public int Year { get; set; }

        public decimal CreditPoints { get; set; }
        public int TransactionTypeId { get; set; }
        
    }
}
