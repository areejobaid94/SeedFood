using Infoseed.MessagingPortal.Currencies.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MultiTenancy.Dto
{
    public class SettingsTenantHostModel
    {
        public int TenantId { get; set; }
        public string TimeZone { get; set; }
        public string Currency { get; set; }
        public List<CurrencyDto> currencyList { get; set; }
        public string ZohoCustomerId { get; set; }
        public int CautionDays { get; set; }
        public int WarningDays { get; set; }
        public bool IsPreOrder { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
        public decimal TotalCustomerWallet { get; set; }
        public string ClientIpAddress { get; set; }
    }
}
