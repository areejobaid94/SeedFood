using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Group.Dto
{
    public class MoveMembersDto
    {
        public long OldGroupId { get; set; }
        public long NewGroupId { get; set; }
        public List<MembersDto> membersDto { get; set; }
    }
}
