using Abp.Application.Services;
using Infoseed.MessagingPortal.TargetReach.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.TargetReach
{
    public interface ITargetReachAppService : IApplicationService
    {
        long CreateTargetReachMessage(TargetReachModel targetReachModel);
        void SetTargetReachMessageInQueue(List<TargetReachEntity> lstTargetReachEntity);
    }
}
