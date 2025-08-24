using Abp.Auditing;
using Abp.Domain.Repositories;
using Framework.Data;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Evaluations;
using Infoseed.MessagingPortal.Wallet;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Data;

namespace Infoseed.MessagingPortal.Tenants.Dashboard
{
    [DisableAuditing]
    public class WalletAppService : MessagingPortalAppServiceBase, IWalletAppService
    {
        private readonly IRepository<Evaluation, long> _evaluationRepository;

        private readonly IRepository<Contact> _contactRepository;
        private readonly IDocumentClient _IDocumentClient;

        public WalletAppService(
            IRepository<Evaluation, long> evaluationRepository
            , IRepository<Contact> contactRepository
            , IDocumentClient iDocumentClient  
            )
        {
            _evaluationRepository = evaluationRepository;
            _contactRepository = contactRepository;
            _IDocumentClient = iDocumentClient;
        }
        #region public
        public void CreateWallet(int TenantId)
        {
            try
            {
                createWallet(TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region private
        private void createWallet(int TenantId)
        {
            try
            {
                var SP_Name = Constants.Wallet.SP_WalletCreate;
                DateTime date = DateTime.UtcNow;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                     ,new System.Data.SqlClient.SqlParameter("@date",date)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutputId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                //if (OutputParameter.Value.ToString() != "" || OutputParameter.Value.ToString() != null)
                //{
                //    return (long)OutputParameter.Value;
                //}
                //else
                //{
                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}