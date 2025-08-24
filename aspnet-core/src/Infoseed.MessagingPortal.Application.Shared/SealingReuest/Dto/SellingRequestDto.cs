using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.SealingReuest.Dto
{
    public class SellingRequestDto
    { 

        public long Id { get; set; }
        public string UserIds { get; set; }
        public long? AreaId { get; set; }
        public int TenantId { get; set; }
        public int ContactId { get; set; }
        public string RequestDescription { get; set; }
        public string SginUpRequestDescription { get; set; }
        public decimal? Price { get; set; }

        public bool IsRequestForm { get; set; } = false;
        public bool IsSginUpForm { get; set; }
        public string ContactInfo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DepartmentId { get; set; }
        
        public string DepartmentUserIds { get; set; }


        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public SellingRequestStatus SellingRequestStatus { get; set; }
        public int SellingStatusId
        {
            get { return (int)this.SellingRequestStatus; }
            set { this.SellingRequestStatus = (SellingRequestStatus)value; }
        } 
        
        public string CreatedOnString
        {
            get { return (String)this.CreatedOn.ToString(); }
            set { }
        }


        public List<SellingRequestDetailsDto> lstSellingRequestDetailsDto { get; set; }
        public SellingRequestFormModel RequestForm { get; set; }

        public SginUpRequestModel SginUpRequest { get; set; }

    }
}
