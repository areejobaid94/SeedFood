using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditions.Dtos
{
    public class GetItemAdditionDetailsModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NameEnglish { get; set; }


        public bool IsInServes { get; set; }

    }
}
