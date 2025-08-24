using Abp.Application.Services;
using Infoseed.MessagingPortal.Loyalty.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Loyalty
{
    public interface   ILoyaltyAppService: IApplicationService
    {
        LoyaltyModel GetAll(int? tenantId = null);
        long CreateOrEdit(LoyaltyModel loyaltyModel);
        decimal ConvertPriceToPoint(decimal? price);
        decimal ConvertCustomerPriceToPoint(decimal? price, int? tenantId = null, LoyaltyModel LoyaltyModel = null);
        decimal ConvertItemsPriceToPoint(decimal? price, decimal loyaltyPoints, int? tenantId = null, LoyaltyModel LoyaltyModel = null);
        decimal ConvertPriceToPoints(decimal?  Price, decimal  LoyaltyPoints, decimal  OriginalLoyaltyPoints, long LoyaltyDefinitionId, LoyaltyModel loyaltyModel=null);
        decimal ConvertPriceToCustomerPoints(decimal? Price, decimal LoyaltyPoints, decimal OriginalLoyaltyPoints, long LoyaltyDefinitionId, LoyaltyModel loyaltyModel = null);
        void CreateContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty);
        LoyaltyModel GetLoyaltyForMenu(int tenantId);


        ItemsLoyaltyLogModel GetItemLoyaltyLog(long? id, int? tenantId = null);

        SpecificationsLoyaltyLogModel GetSpecificationsLoyaltyLog(long? id, int? tenantId = null);
        AdditionsLoyaltyLogModel GetAdditionsLoyaltyLog(long? id, int? tenantId = null);

        void UpdateContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty);
    }
}
