using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class CreateOrUpdateUserInput
    {
        [Required]
        public UserEditDto User { get; set; }

        [Required]
        public string[] AssignedRoleNames { get; set; }

        public bool SendActivationEmail { get; set; }

        public bool SetRandomPassword { get; set; }

        public List<long> OrganizationUnits { get; set; }


        public long? AreaId { get; set; }

        public string AreaIds { get; set; }

        public CreateOrUpdateUserInput()
        {
            OrganizationUnits = new List<long>();
        }
    }
}