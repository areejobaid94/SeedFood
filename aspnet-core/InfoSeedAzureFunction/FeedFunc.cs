using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InfoSeedAzureFunction
{
    public static class FeedFunc
    {
        //[FunctionName("FeedFunc")]
        //public static void Run([CosmosDBTrigger(
        //    databaseName: "InfoSeedTeamInbox",
        //    collectionName: "Items",
        //    ConnectionStringSetting = "CosmosDBConnection"
        //    ,CreateLeaseCollectionIfNotExists =true
        //  )]IReadOnlyList<Document> input, ILogger log)
        //{
        //    if (input != null && input.Count > 0)
        //    {
        //        dynamic doc = JsonConvert.DeserializeObject(input[0].ToString());
        //        if (doc.ItemType != null && doc.ItemType == 1)//message
        //        {
        //            var createDate = (DateTime)doc.CreateDate;
        //            var userId = (string)doc.userId;

        //            var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
        //            {
        //                new System.Data.SqlClient.SqlParameter("@UserId",userId),
        //                new System.Data.SqlClient.SqlParameter("@MessageDate",createDate)
        //            };
        //            SqlDataHelper.ExecuteNoneQuery(
        //                "dbo.ActiveContactPerMonthUpdate",
        //                sqlParameters.ToArray(), Environment.GetEnvironmentVariable("DBConnectionString")
        //                );
        //            log.LogInformation("This is Tenant document: " + (int)doc.TenantId.Value);

        //        }
        //        else
        //        {
        //            //log.LogInformation("Documents modified " + input.Count);
        //            //log.LogInformation("First document Id " + input[0].Id);
        //        }
        //    }
        //}
    }
}
