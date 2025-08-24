using System;
using Abp.AutoMapper;
using Infoseed.MessagingPortal.Sessions.Dto;

namespace Infoseed.MessagingPortal.Models.Common
{
    [AutoMapFrom(typeof(ApplicationInfoDto)),
     AutoMapTo(typeof(ApplicationInfoDto))]
    public class ApplicationInfoPersistanceModel
    {
        public string Version { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}