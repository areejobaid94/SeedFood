using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class WorkingHoursModel
    {

        public bool IsWorkActiveSun { get; set; }
        public string WorkTextSun { get; set; }
        public TimeSpan StartDateSun { get; set; }
        public TimeSpan EndDateSun { get; set; }



        public bool IsWorkActiveMon { get; set; }
        public string WorkTextMon { get; set; }
        public TimeSpan StartDateMon { get; set; }
        public TimeSpan EndDateMon { get; set; }


        public bool IsWorkActiveTues { get; set; }
        public string WorkTextTues { get; set; }
        public DateTime StartDateTues { get; set; }
        public DateTime EndDateTues { get; set; }



        public bool IsWorkActiveWed { get; set; }
        public string WorkTextWed { get; set; }
        public DateTime StartDateWed { get; set; }
        public DateTime EndDateWed { get; set; }


        public bool IsWorkActiveThurs { get; set; }
        public string WorkTextThurs { get; set; }
        public DateTime StartDateThurs { get; set; }
        public DateTime EndDateThurs { get; set; }


        public bool IsWorkActiveFri { get; set; }
        public string WorkTextFri { get; set; }
        public DateTime StartDateFri { get; set; }
        public DateTime EndDateFri { get; set; }



        public bool IsWorkActiveSat { get; set; }
        public string WorkTextSat { get; set; }
        public DateTime StartDateSat { get; set; }
        public DateTime EndDateSat { get; set; }


    }

}
