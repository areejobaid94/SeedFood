using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Specifications.Dtos
{
    public class SpecificationsDto
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }

        public string SpecificationDescription { get; set; }
        public string SpecificationDescriptionEnglish { get; set; }
        public bool IsMultipleSelection { get; set; }
        public int LanguageBotId { get; set; }
        public DateTime LastModificationTime { get; set; }

        public int MaxSelectNumber { get; set; }

        public int CreatorUserId { get; set; }
        public int LastModifierUserId { get; set; }

        public bool IsDeleted { get; set; }

        public int DeleterUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime DeletionTime { get; set; }
        public int Priority { get; set; }
    }
}
