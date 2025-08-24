using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public static class InfoSeedSettingService
    {
        #region  DateTime 
        public static DateTime ToInfoSeedDateTime(this string strDate)
        {
            try
            { 
                DateTime dateTime = new DateTime();
                dateTime = DateTime.Parse(strDate, CultureInfo.InvariantCulture).AddHours(AppSettingsModel.DivHour);
                return dateTime;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 
    }
}
