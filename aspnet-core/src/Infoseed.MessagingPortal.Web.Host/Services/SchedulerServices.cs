using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Services
{
    public class SchedulerServices : ISchedulerServices
    {
        public SchedulerServices()
        {
            

        }

        public void Strat(string StartTime, string Hour)
        {

            DateTime date = DateTime.ParseExact(StartTime, "hh:mm tt", System.Globalization.CultureInfo.CurrentCulture);
            var tt = date.ToString("tt");

            int H = date.Hour;
            if (date.Hour > 12)
                H = date.Hour - 12;

            IntervalInSeconds(H, date.Minute, int.Parse(Hour), int.Parse(Hour), tt,
                 () =>
                 {
                    
                 });


        }

        public void IntervalInSeconds(int hour, int sec, double interval, int pls, string tt, Action task)
        {
            interval = interval / 3600;
            Scheduler.Instance.ScheduleTask(hour, sec, interval, pls, tt, task);
        }

        public void IntervalInMinutes(int hour, int min, double interval, int pls, string tt, Action task)
        {
            interval = interval / 60;
            Scheduler.Instance.ScheduleTask(hour, min, interval, pls, tt, task);
        }

        public void IntervalInHours(int hour, int min, double interval, int pls, string tt, Action task)
        {
            Scheduler.Instance.ScheduleTask(hour, min, interval, pls, tt, task);
        }

        public void IntervalInDays(int hour, int min, double interval, int pls, string tt, Action task)
        {
            interval = interval * 24;
            Scheduler.Instance.ScheduleTask(hour, min, interval, pls, tt, task);
        }
    }
}
