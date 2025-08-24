using Framework.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace Framework.Payment
{
    internal class PaymentRepository
    {
        public static void InsertActionLog(ActionLog actionlog,IConfiguration configuration)
        {
            try
            {
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@RequestorIPAddress",actionlog.RequestorIPAddress),
                    new SqlParameter("@MachineName",actionlog.MachineName),
                    new SqlParameter("@UserName",actionlog.UserName),
                    new SqlParameter("@RequestUri",actionlog.RequestUri),
                    new SqlParameter("@RequestMethod",actionlog.RequestMethod),
                    new SqlParameter("@RequestTimestamp",actionlog.RequestTimestamp),
                    new SqlParameter("@RequestContentType",actionlog.RequestContentType),
                    new SqlParameter("@RequestHeaders",actionlog.RequestHeaders),
                    new SqlParameter("@RequestContent",actionlog.RequestContent),
                    new SqlParameter("@RequestRawData",actionlog.RequestRawData),
                    new SqlParameter("@ResponseStatusCode",actionlog.ResponseStatusCode),
                    new SqlParameter("@ResponseTimestamp",actionlog.ResponseTimestamp),
                    new SqlParameter("@ResponseContentType",actionlog.ResponseContentType),
                    new SqlParameter("@ResponseHeaders",actionlog.ResponseHeaders),
                    new SqlParameter("@ResponseContent",actionlog.ResponseContent),
                    new SqlParameter("@ResponseRawData",actionlog.ResponseRawData)
                };


                var outActionLogID = new SqlParameter();
                outActionLogID.Direction = ParameterDirection.Output;
                outActionLogID.SqlDbType = SqlDbType.BigInt;
                outActionLogID.ParameterName = "@ID";
                sqlParameters.Add(outActionLogID);

                SqlDataHelper.ExecuteNoneQuery(
                           Constants.SP_ActionLogInsert,
                           sqlParameters.ToArray(), configuration.GetConnectionString("Default"));
  
                    actionlog.ID = (long)outActionLogID.Value;
              
                
            }
            catch (SqlException )
            {
                //NLogManager.LogError("SQL ERROR WHILE  INSERTING ActionLogs", sqlex);
            }
            catch (Exception )
            {
                //NLogManager.LogError("UNEXPECTED EXCEPTION WHILE INSERTING ActionLogs", ex);
            }
        }

        public static void UpdateActionLog(ActionLog actionlog, IConfiguration configuration)
        {
            try
            {
                var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@ID",actionlog.ID),
                new SqlParameter("@RequestorIPAddress",actionlog.RequestorIPAddress),
                new SqlParameter("@MachineName",actionlog.MachineName),
                new SqlParameter("@UserName",actionlog.UserName),
                new SqlParameter("@RequestUri",actionlog.RequestUri),
                new SqlParameter("@RequestMethod",actionlog.RequestMethod),
                new SqlParameter("@RequestTimestamp",actionlog.RequestTimestamp),
                new SqlParameter("@RequestContentType",actionlog.RequestContentType),
                new SqlParameter("@RequestHeaders",actionlog.RequestHeaders),
                new SqlParameter("@RequestContent",actionlog.RequestContent),
                new SqlParameter("@RequestRawData",actionlog.RequestRawData),
                new SqlParameter("@ResponseStatusCode",actionlog.ResponseStatusCode),
                new SqlParameter("@ResponseTimestamp",actionlog.ResponseTimestamp),
                new SqlParameter("@ResponseContentType",actionlog.ResponseContentType),
                new SqlParameter("@ResponseHeaders",actionlog.ResponseHeaders),
                new SqlParameter("@ResponseContent",actionlog.ResponseContent),
                new SqlParameter("@ResponseRawData",actionlog.ResponseRawData)
            };

                SqlDataHelper.ExecuteNoneQuery(
                              Constants.SP_ActionLogUpdate,
                              sqlParameters.ToArray(), configuration.GetConnectionString("Default"));

            }
            catch (SqlException)
            {
                //NLogManager.LogError("SQL ERROR WHILE  UPDATEING ActionLogs", sqlex);
            }
            catch (Exception)
            {
                //NLogManager.LogError("UNEXPECTED EXCEPTION WHILE UPDATEING ActionLogs", ex);
            }
        }

        private static ActionLog MapActionLog(IDataReader dataReader)
        {
            ActionLog actionlog = new ActionLog
            {
                ID = SqlDataHelper.GetValue<long>(dataReader, "ID"),
                RequestorIPAddress = SqlDataHelper.GetValue<string>(dataReader, "RequestorIPAddress"),
                RequestUri = SqlDataHelper.GetValue<string>(dataReader, "RequestUri"),
                RequestMethod = SqlDataHelper.GetValue<string>(dataReader, "RequestMethod"),
                RequestTimestamp = SqlDataHelper.GetValue<DateTime>(dataReader, "RequestTimestamp"),
                RequestContentType = SqlDataHelper.GetValue<string>(dataReader, "RequestContentType"),
                RequestHeaders = SqlDataHelper.GetValue<string>(dataReader, "RequestHeaders"),
                RequestContent = SqlDataHelper.GetValue<string>(dataReader, "RequestContent"),
                RequestRawData = SqlDataHelper.GetValue<string>(dataReader, "RequestRawData"),
                ResponseStatusCode = SqlDataHelper.GetValue<int>(dataReader, "ResponseStatusCode"),
                ResponseTimestamp = SqlDataHelper.GetValue<DateTime>(dataReader, "ResponseTimestamp"),
                ResponseContentType = SqlDataHelper.GetValue<string>(dataReader, "ResponseContentType"),
                ResponseHeaders = SqlDataHelper.GetValue<string>(dataReader, "ResponseHeaders"),
                ResponseContent = SqlDataHelper.GetValue<string>(dataReader, "ResponseContent"),
                ResponseRawData = SqlDataHelper.GetValue<string>(dataReader, "ResponseRawData"),
            };
            return actionlog;
        }
    }
}
