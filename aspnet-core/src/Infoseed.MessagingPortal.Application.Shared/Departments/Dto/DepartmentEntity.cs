using Infoseed.MessagingPortal.Asset.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Departments.Dto
{
    public class DepartmentEntity
    {
        public List<DepartmentModel> lstDepartments { get; set; }
        public int TotalCount { get; set; }
    }
}
