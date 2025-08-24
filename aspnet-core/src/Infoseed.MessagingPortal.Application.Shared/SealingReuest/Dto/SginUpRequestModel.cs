using System;
using System.Collections.Generic;
using System.Text;
using static Infoseed.MessagingPortal.SealingReuest.Dto.RequestEnums;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public class SginUpRequestModel
    {
        public ServiceTypeEnum serviceTypeEnum { get; set; }
        public int ServiceTypeId
        {
            get { return (int)this.serviceTypeEnum; }
            set { this.serviceTypeEnum = (ServiceTypeEnum)value; }
        }
        public string ServiceTypeName
        {
            get { return Enum.GetName(typeof(ServiceTypeEnum), ServiceTypeId); }
        }
        public string Country { get; set; }
        public string OwnerName { get; set; }
        public string ContactNumber { get; set; }
        public long Latitude { get; set; }
        public long longitude { get; set; }


        public PricingEndPlanEnum pricingEndPlanEnum { get; set; }
        public int PricingPlanId
        {
            get { return (int)this.pricingEndPlanEnum; }
            set { this.pricingEndPlanEnum = (PricingEndPlanEnum)value; }
        }
        public string PricingPlanName 
        {
            get { return Enum.GetName(typeof(PricingEndPlanEnum), PricingPlanId); }
        }


        public OnlineStoreRequestModel onlineStoreRequestModel { get; set; }
        public CampaignRequestModel campaignRequestModel { get; set; }
        public BookingRequestModel bookingRequestModel { get; set; }
        public EnterpriseRequestModel enterpriseRequestModel { get; set; }
        
    }
}
