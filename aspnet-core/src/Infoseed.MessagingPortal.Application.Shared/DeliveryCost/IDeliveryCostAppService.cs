using Infoseed.MessagingPortal.DeliveryCost.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryCost
{
    public interface IDeliveryCostAppService
    {
        long AddDeliveryCost(DeliveryCostDto deliveryCostDto);
        void UpdateDeliveryCost(DeliveryCostDto deliveryCostDto);
        bool DeleteDeliveryCost(long deliveryCostId);
        DeliveryCostEntity GetDeliveryCost(int? tenantId = null, int pageNumber = 0, int pageSize = 50);
        DeliveryCostDto GetDeliveryCostById(long deliveryCostId, int? tenantId);
        DeliveryCostDto GetDeliveryCostByAreaId(int tenantId, long areaId);
        LocationAddressDto GetDeliveryCostPerArea(int tenantId,  float latitude, float longitude, string city, string area, string distric);
    }
}
