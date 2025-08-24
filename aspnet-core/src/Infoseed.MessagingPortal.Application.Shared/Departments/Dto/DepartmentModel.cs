using System;

namespace Infoseed.MessagingPortal.Departments.Dto
{
    public class DepartmentModel
    {
        public long Id { get; set; }
        public int DepartmentId { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string UserIds { get; set; }
        public string UserNames { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsLiveChat { get; set; }
        public bool IsSellingRequest { get; set; }
    }
}
