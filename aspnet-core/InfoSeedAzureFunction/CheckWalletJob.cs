using InfoSeedAzureFunction.Model.WalletJopModel;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace InfoSeedAzureFunction
{
    public class CheckWalletJob
    {
//        [FunctionName("CheckWalletFunction")]
//        public static async Task RunAsync([TimerTrigger("0 0 0 * * * ", RunOnStartup = true)] TimerInfo myTimer, ILogger log)//every day (daily)
//        {


////var itemsCollection38 = new DocumentDBHelper<CustomerModel>(CollectionTypes.CustomersCollection);
//           // var customerResult38888 = await itemsCollection38.GetItemAsync(a => a.ItemType == 0 && a.userId == "27_962779746365" && a.TenantId == 27);
//            //The cron expression "0 0 0 * * *" corresponds to midnight(00:00:00) in Coordinated Universal Time(UTC).
//            CheckFunction(log).Wait();
//        }
        public static async Task CheckFunction(ILogger log)
        {
            try
            {
                var transactionsList = GetTransactionInfo();
                var walletsList = GetWalletInfo();

                DateTime dateNow = DateTime.UtcNow;
                TimeSpan sevenDays = TimeSpan.FromDays(7);

                transactionsList = transactionsList.Where(t => (dateNow - t.TransactionDate).TotalDays > 7).ToList();
                
                if (transactionsList != null)
                {
                    foreach (var transaction in transactionsList)
                    {
                        var SucssesTransactionsList = GetTransactionSucsses(transaction.CampaignId);

                        if (SucssesTransactionsList != null && SucssesTransactionsList.Any())
                        {
                            var firstTransaction = SucssesTransactionsList.FirstOrDefault();

                            // Now 'firstTransaction' contains the first transaction from SucssesTransactionsList
                            if (firstTransaction != null && SucssesTransactionsList.Count != 0)
                            {
                                // Do something with 'firstTransaction'
                                if (transaction.Quantity == SucssesTransactionsList.Count)
                                {

                                }
                                else
                                {
                                    // Calculate the absolute difference
                                    int absoluteDifference = Math.Abs(transaction.Quantity - SucssesTransactionsList.Count);
                                    decimal difference = firstTransaction.TotalTransaction * absoluteDifference;

                                    var sameWallet = walletsList.Where(w => w.WalletId == transaction.WalletId).FirstOrDefault();
                                    if (sameWallet.OnHold > 0 && sameWallet.OnHold >= difference)
                                    {
                                        sameWallet.OnHold -= difference;
                                        WalletUpdate(sameWallet.WalletId, sameWallet.OnHold, transaction.Id, dateNow, difference);
                                    }
                                }
                            }
                            else
                            {
                                var sameWallet = walletsList.Where(w => w.WalletId == transaction.WalletId).FirstOrDefault();
                                if (sameWallet.OnHold > 0 && sameWallet.OnHold >= transaction.TotalTransaction)
                                {
                                    sameWallet.OnHold -= transaction.TotalTransaction;
                                    WalletUpdate(sameWallet.WalletId, sameWallet.OnHold, transaction.Id, dateNow, transaction.TotalTransaction);
                                }
                            }
                        }
                        else
                        {
                            var sameWallet = walletsList.Where(w => w.WalletId == transaction.WalletId).FirstOrDefault();
                            if (sameWallet.OnHold > 0 && sameWallet.OnHold >= transaction.TotalTransaction)
                            {
                                sameWallet.OnHold -= transaction.TotalTransaction;
                                WalletUpdate(sameWallet.WalletId, sameWallet.OnHold, transaction.Id, dateNow, transaction.TotalTransaction);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region private
        private static List<TransactionModel> GetTransactionInfo()
        {
            try
            {
                List<TransactionModel> model = new List<TransactionModel>();
                var SP_Name = Constants.Transaction.SP_GetTransactionsForWalletJob; //GetTransactionsForWalletJob
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> { };

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapTransactionInfo, Constants.ConnectionString).ToList();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static List<TransactionModel> GetTransactionSucsses(long CampaignId)
        {
            try
            {
                List<TransactionModel> model = new List<TransactionModel>();
                var SP_Name = Constants.Transaction.SP_GetTransactionSucssesJob; //GetTransactionSucssesJob
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> 
                {
                    new System.Data.SqlClient.SqlParameter("@CampaignId",CampaignId)
                };

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapTransactionInfo, Constants.ConnectionString).ToList();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static List<WalletModel> GetWalletInfo()
        {
            try
            {
                List<WalletModel> model = new List<WalletModel>();
                var SP_Name = Constants.Wallet.SP_GetAllWalletJob; //GetAllWalletJob
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> { };

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapWalletInfo, Constants.ConnectionString).ToList();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void WalletUpdate(long walletId, decimal totalReduction,long transactionId,DateTime dateNow, decimal totalTransaction)
        {
            try
            {
                var SP_Name = Constants.Wallet.SP_WalletAndTransUpdateJob; //WalletAndTransUpdateJob
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> 
                {
                    new System.Data.SqlClient.SqlParameter("@walletId",walletId)
                    ,new System.Data.SqlClient.SqlParameter("@totalReduction",totalReduction)
                    ,new System.Data.SqlClient.SqlParameter("@transactionId",transactionId)
                    ,new System.Data.SqlClient.SqlParameter("@dateNow",dateNow)
                    ,new System.Data.SqlClient.SqlParameter("@totalTransaction",totalTransaction)

                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

                //var OutputParameter = new System.Data.SqlClient.SqlParameter
                //{
                //    SqlDbType = SqlDbType.BigInt,
                //    ParameterName = "@Output",
                //    Direction = ParameterDirection.Output
                //};
                //sqlParameters.Add(OutputParameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region Mapper

        public static TransactionModel MapTransactionInfo(IDataReader dataReader)
        {
            try
            {
                TransactionModel model = new TransactionModel();

                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                model.DoneBy = SqlDataHelper.GetValue<string>(dataReader, "DoneBy");
                model.TotalTransaction = SqlDataHelper.GetValue<decimal>(dataReader, "TotalTransaction");
                model.TransactionDate = SqlDataHelper.GetValue<DateTime>(dataReader, "TransactionDate");
                model.CategoryType = SqlDataHelper.GetValue<string>(dataReader, "CategoryType");
                model.TotalRemaining = SqlDataHelper.GetValue<decimal>(dataReader, "TotalRemaining");
                model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
                model.Country = SqlDataHelper.GetValue<string>(dataReader, "Country");
                model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                model.InvoiceId = SqlDataHelper.GetValue<string>(dataReader, "invoiceId");
                model.InvoiceUrl = SqlDataHelper.GetValue<string>(dataReader, "invoiceUrl");
                model.IsPayed = SqlDataHelper.GetValue<bool>(dataReader, "IsPayed");
                model.Note = SqlDataHelper.GetValue<string>(dataReader, "Note");
                model.CampaignName = SqlDataHelper.GetValue<string>(dataReader, "campaignName");
                model.TemplateName = SqlDataHelper.GetValue<string>(dataReader, "templateName");
                model.Quantity = SqlDataHelper.GetValue<int>(dataReader, "quantity");
                model.Countries = SqlDataHelper.GetValue<int>(dataReader, "countries");
                model.CampaignId = SqlDataHelper.GetValue<long>(dataReader, "campaignId");
                model.IsOnHold = SqlDataHelper.GetValue<bool>(dataReader, "IsOnHold");

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static WalletModel MapWalletInfo(IDataReader dataReader)
        {
            try
            {
                WalletModel model = new WalletModel();

                model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
                model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                model.TotalAmount = SqlDataHelper.GetValue<decimal>(dataReader, "TotalAmount");
                model.OnHold = SqlDataHelper.GetValue<decimal>(dataReader, "OnHold");
                model.DepositDate = SqlDataHelper.GetValue<DateTime>(dataReader, "DepositDate");

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion
    }
}
