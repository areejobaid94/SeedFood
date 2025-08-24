using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Evaluations.Dtos
{
   public  class GetEvaluationForViewDto
    {
        public EvaluationDto evaluation { get; set; }

        public string UserId { get; set; }
        public string CreatTime { get; set; }

        public string CreatDate { get; set; }
    }
}
