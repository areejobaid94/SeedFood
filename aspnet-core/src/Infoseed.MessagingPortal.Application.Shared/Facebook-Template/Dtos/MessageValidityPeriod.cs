using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class MessageValidityPeriod
    {
        public bool IsCustomValidity { get; set; }
        public ValidityPeriod? ValidityPeriod { get; set; } 
    }
    public enum ValidityPeriod
    {
        Seconds30,   
        Minutes1,    
        Minutes2,    
        Minutes3,    
        Minutes5,    
        Minutes10,   
        Minutes15,   
        Minutes30,   
        Hours1,      
        Hours3,      
        Hours6,      
        Hours12      
    }

}
