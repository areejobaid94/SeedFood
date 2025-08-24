using Framework.Data;
using Infoseed.MessagingPortal.UTracOrder;
using Infoseed.MessagingPortal.UTracOrder.Dto;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Infoseed.MessagingPortal.UTrackOrder;
using Infoseed.MessagingPortal.TargetReach.Dto;

namespace Infoseed.MessagingPortal.TargetReach
{
    public class TargetReachAppService : MessagingPortalAppServiceBase, ITargetReachAppService
    {

        public long CreateTargetReachMessage(TargetReachModel targetReachModel)
        {
         //   SetTargetReachMessageInQueue(WhatsAppFunModel message)
            return createTargetReachMessage(targetReachModel);
        }
        
        public void SetTargetReachMessageInQueue(List<TargetReachEntity> lstTargetReachEntity)
        {
         //   SetTargetReachMessageInQueue(WhatsAppFunModel message)
             setTargetReachMessageInQueue(lstTargetReachEntity);
        }


        #region Private Methods
        private long createTargetReachMessage(TargetReachModel UtracOrder)
        {
            try
            {
                var SP_Name = Constants.TargetReach.SP_TargetReachMessageAdd;

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@OrderId",UtracOrder.SenderPhoneNumber),
                    new SqlParameter("@TenantId",UtracOrder.ReciverName),
                    new SqlParameter("@TenantId",UtracOrder.ReciverPhoneNumber),
                    new SqlParameter("@CreatedDate",DateTime.UtcNow)
                
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@Id";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),AppSettingsModel.ConnectionStrings);



                return (long)OutputParameter.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void setTargetReachMessageInQueue(List<TargetReachEntity> lstTargetReachEntity)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("targetreach-sync");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(lstTargetReachEntity));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception ex)
            {
                throw;
              //  var Error = JsonConvert.SerializeObject(message);
            }


        }
        #endregion

    }
}
