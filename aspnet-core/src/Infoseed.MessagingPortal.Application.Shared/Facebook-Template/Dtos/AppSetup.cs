using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class AppSetup
    {
        public List<AppSetupDTO> Apps { get; set; } = new List<AppSetupDTO>();

    }
    public class AppSetupDTO
    {
        public string PackageName { get; set; }
        public string AppSignature { get; set; }
    }
}
