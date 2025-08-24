using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Services
{
    public interface ISchedulerServices
    {
        void IntervalInSeconds(int hour, int sec, double interval, int pls, string tt, Action task);
        void IntervalInMinutes(int hour, int min, double interval, int pls, string tt, Action task);
        void IntervalInHours(int hour, int min, double interval, int pls, string tt, Action task);
        void IntervalInDays(int hour, int min, double interval, int pls, string tt, Action task);
        void Strat(string StartTime, string houer);     
    }
}
