using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Services
{
    public class Scheduler
    {
        private static Scheduler _instance;
        private List<Timer> timers = new List<Timer>();

        private Scheduler() { }

        public static Scheduler Instance => _instance ?? (_instance = new Scheduler());

        public void ScheduleTask(int hour, int min, double intervalInHour, int pls, string tt, Action task)
        {
            DateTime now = DateTime.Now;// time now            
            string Time = string.Empty;
            Format(hour, min, tt, out Time);//time first Run

            DateTime firstRun = DateTime.ParseExact(Time, "hh:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture);
            TimeSpan timeToGo = firstRun - now;

            while (timeToGo <= TimeSpan.Zero)
            {
                firstRun = DateTime.ParseExact(Time, "hh:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture).AddHours(pls);
                timeToGo = firstRun - now;
                tt = firstRun.ToString("tt");
                Format(firstRun.Hour, firstRun.Minute, tt, out Time);
            }

            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));

            timers.Add(timer);
        }

        private static void Format(int hour, int min, string tt, out string Time)
        {
            string HourS = string.Empty;
            string MinS = string.Empty;
            if (hour < 10)
                HourS = "0" + hour.ToString();
            else
                HourS = hour.ToString();

            if (min < 10)
                MinS = "0" + min.ToString();
            else
                MinS = min.ToString();


            Time = HourS + ":" + MinS + ":00 " + tt;
        }
    }
}
