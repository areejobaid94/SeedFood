using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class CategoryTypeUsageEnum
    {
        public enum TransactionTypeUsage
        {
            MarketingConversation,
            ServiceConversation,
            UtilityConversation,
            Deposit,
            FrozenAccount,
            DepositFromFrozenAccount,
        }
    }
}
