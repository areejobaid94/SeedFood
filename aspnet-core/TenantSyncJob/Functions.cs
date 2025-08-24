using Framework.Data;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobEntities;

namespace TenantSyncJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("tenant-sync")] string message, TextWriter log)
        {
            //log.WriteLine(message);
            TenantSyncMessage obj = JsonConvert.DeserializeObject<TenantSyncMessage>(message);
            Sync(Constants.ConnectionString, obj).Wait();

        }
        public static async Task Sync(string connectionString, TenantSyncMessage tenantMessage)
        {
           
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Id",tenantMessage.TenantId),
            };


            IList<TenantModel> result =
                   SqlDataHelper.ExecuteReader(
                       "[dbo].[TenantsGetById] ",
                       sqlParameters.ToArray(),
                       MapTenantModel,
                      connectionString);


            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);

            foreach (var item in result)
            {
                var tenant = itemsCollection.GetItemAsync(p => p.TenantId == item.TenantId && p.ItemType == ContainerItemTypes.Tenant).Result;
                if (tenant == null)
                {
                    await itemsCollection.CreateItemAsync(new TenantModel()
                    {
                        ItemType = ContainerItemTypes.Tenant,
                        SmoochAppID = item.SmoochAppID,
                        SmoochSecretKey = item.SmoochSecretKey,
                        SmoochAPIKeyID = item.SmoochAPIKeyID,
                        TenantId = item.TenantId,

                        DirectLineSecret = item.DirectLineSecret,
                        botId = item.botId,
                        IsBotActive = item.IsBotActive,
                        IsCancelOrder = item.IsCancelOrder,
                        CancelTime = item.CancelTime,

                        //StartDate = item.StartDate,
                        //EndDate = item.EndDate,

                        //WorkText = item.WorkText,
                        IsWorkActive = item.IsWorkActive,
                        workModel = new WorkModel
                        {
                            EndDateFri = item.workModel.EndDateFri,
                            EndDateMon = item.workModel.EndDateMon,
                            EndDateSat = item.workModel.EndDateSat,
                            EndDateSun = item.workModel.EndDateSun,
                            EndDateThurs = item.workModel.EndDateThurs,
                            EndDateTues = item.workModel.EndDateTues,
                            EndDateWed = item.workModel.EndDateWed,

                            IsWorkActiveFri = item.workModel.IsWorkActiveFri,
                            IsWorkActiveMon = item.workModel.IsWorkActiveMon,
                            IsWorkActiveSat = item.workModel.IsWorkActiveSat,
                            IsWorkActiveSun = item.workModel.IsWorkActiveSun,
                            IsWorkActiveThurs = item.workModel.IsWorkActiveThurs,
                            IsWorkActiveTues = item.workModel.IsWorkActiveTues,
                            IsWorkActiveWed = item.workModel.IsWorkActiveWed,

                            StartDateFri = item.workModel.StartDateFri,
                            StartDateMon = item.workModel.StartDateMon,
                            StartDateSat = item.workModel.StartDateSat,
                            StartDateSun = item.workModel.StartDateSun,
                            StartDateThurs = item.workModel.StartDateThurs,
                            StartDateTues = item.workModel.StartDateTues,
                            StartDateWed = item.workModel.StartDateWed,

                            WorkTextFri = item.workModel.WorkTextFri,
                            WorkTextMon = item.workModel.WorkTextMon,
                            WorkTextSat = item.workModel.WorkTextSat,
                            WorkTextSun = item.workModel.WorkTextSun,
                            WorkTextThurs = item.workModel.WorkTextThurs,
                            WorkTextTues = item.workModel.WorkTextTues,
                            WorkTextWed = item.workModel.WorkTextWed

                        },

                        IsEvaluation = item.IsEvaluation,
                        EvaluationText = item.EvaluationText,
                        EvaluationTime = item.EvaluationTime,

                        IsLoyalityPoint = item.IsLoyalityPoint,
                        Points = item.Points,

                        D360Key = item.D360Key,
                        isOrderOffer = item.isOrderOffer,
                        PhoneNumber = item.PhoneNumber,
                        Image = item.Image,
                        ImageBg = item.ImageBg




                    });
                }
                else
                {
                    tenant.ItemType = ContainerItemTypes.Tenant;
                    tenant.TenantId = item.TenantId;
                    tenant.DirectLineSecret = item.DirectLineSecret;
                    tenant.botId = item.botId;
                    tenant.PhoneNumber = item.PhoneNumber;
                    tenant.D360Key = item.D360Key;
                    tenant.Image = item.Image;
                    tenant.ImageBg = item.ImageBg;
                    await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                }
            }
        }
        private static TenantModel MapTenantModel(IDataReader dataReader)
        {
            TenantModel catalogue = new TenantModel
            {
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                SmoochAppID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAppID"),
                SmoochSecretKey = SqlDataHelper.GetValue<string>(dataReader, "SmoochSecretKey"),
                SmoochAPIKeyID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAPIKeyID"),

                DirectLineSecret = SqlDataHelper.GetValue<string>(dataReader, "DirectLineSecret"),
                botId = SqlDataHelper.GetValue<string>(dataReader, "botId"),
                IsBotActive = SqlDataHelper.GetValue<bool>(dataReader, "IsBotActive"),

                IsCancelOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsCancelOrder"),
                CancelTime = SqlDataHelper.GetValue<int>(dataReader, "CancelTime"),

                //StartDate = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDate"),
                //EndDate = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDate"),
                //WorkText = SqlDataHelper.GetValue<string>(dataReader, "WorkText"),
                IsWorkActive = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActive"),
                IsBundleActive  = SqlDataHelper.GetValue<bool>(dataReader, "IsBundleActive"),
                workModel = new WorkModel
                {
                    IsWorkActiveSun = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSun"),
                    IsWorkActiveFri = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveFri"),
                    IsWorkActiveMon = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveMon"),
                    IsWorkActiveSat = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSat"),
                    IsWorkActiveThurs = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveThurs"),
                    IsWorkActiveTues = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveTues"),
                    IsWorkActiveWed = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveWed"),

                    WorkTextMon = SqlDataHelper.GetValue<string>(dataReader, "WorkTextMon"),
                    WorkTextSat = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSat"),
                    WorkTextSun = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSun"),
                    WorkTextThurs = SqlDataHelper.GetValue<string>(dataReader, "WorkTextThurs"),
                    WorkTextTues = SqlDataHelper.GetValue<string>(dataReader, "WorkTextTues"),
                    WorkTextWed = SqlDataHelper.GetValue<string>(dataReader, "WorkTextWed"),
                    WorkTextFri = SqlDataHelper.GetValue<string>(dataReader, "WorkTextFri"),

                    StartDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSat"),
                    StartDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateFri"),
                    StartDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateMon"),
                    StartDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSun"),
                    StartDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateThurs"),
                    StartDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateTues"),
                    StartDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateWed"),


                    EndDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateFri"),
                    EndDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateMon"),
                    EndDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSat"),
                    EndDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSun"),
                    EndDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateThurs"),
                    EndDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateTues"),
                    EndDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateWed"),

                },

                IsEvaluation = SqlDataHelper.GetValue<bool>(dataReader, "IsEvaluation"),
                EvaluationText = SqlDataHelper.GetValue<string>(dataReader, "EvaluationText"),
                EvaluationTime = SqlDataHelper.GetValue<int>(dataReader, "EvaluationTime"),
                D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key"),

                isOrderOffer = SqlDataHelper.GetValue<bool>(dataReader, "isOrderOffer"),

                IsLoyalityPoint = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyalityPoint"),
                Points = SqlDataHelper.GetValue<int>(dataReader, "Points "),

                PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
                Image = SqlDataHelper.GetValue<string>(dataReader, "Image"),
                ImageBg = SqlDataHelper.GetValue<string>(dataReader, "ImageBg"),

                id = SqlDataHelper.GetValue<string>(dataReader, "id"),
                _rid = SqlDataHelper.GetValue<string>(dataReader, "_rid"),
                _self = SqlDataHelper.GetValue<string>(dataReader, "_self"),
                _etag = SqlDataHelper.GetValue<string>(dataReader, "_etag"),
                _attachments = SqlDataHelper.GetValue<string>(dataReader, "_attachments"),
                _ts = SqlDataHelper.GetValue<int>(dataReader, "_ts"),


            };
            return catalogue;
        }
    }
}
