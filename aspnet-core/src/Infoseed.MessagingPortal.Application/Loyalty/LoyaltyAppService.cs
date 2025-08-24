using Abp.Runtime.Caching;
using Framework.Data;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Infoseed.MessagingPortal.Loyalty
{
    public class LoyaltyAppService : MessagingPortalAppServiceBase, ILoyaltyAppService
    {
        public  ICacheManager _cacheManager;
        public LoyaltyAppService(ICacheManager cacheManager)
        {
            _cacheManager=cacheManager;
        }

        public LoyaltyAppService()
        {
            
        }


        public LoyaltyModel GetAll(int? tenantId = null)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }

            LoyaltyModel model = new LoyaltyModel();

            var objLoyalty = _cacheManager.GetCache("CacheLoyalty").Get("Loyalty_"+ tenantId.ToString(), cache => cache);
            if (objLoyalty.Equals("Loyalty_"+ tenantId.ToString()))
            {
                model = LoyaltyGet(tenantId.Value);
                if (model != null)
                    _cacheManager.GetCache("CacheLoyalty").Set("Loyalty_"+ tenantId.ToString(), model);
                else
                    model = new LoyaltyModel() {StartDate=DateTime.Now,EndDate=DateTime.Now, CustomerCurrencyValue=1 , CustomerPoints=100, ItemsCurrencyValue=1, ItemsPoints=100, IsLoyalityPoint=false };
            }
            else
            {
                model = (LoyaltyModel)objLoyalty;
            }

 

            return model;
        }
        public bool IsLoyalTenant()
        {
            try
            {
                return GetAll().IsLoyalityPoint;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ItemsLoyaltyLogModel GetItemLoyaltyLog(long? id,int? tenantId = null)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }
            ItemsLoyaltyLogModel model = new ItemsLoyaltyLogModel();
            if (id==null)
            {
                model.LoyaltyDefinitionId=0;

            }
            else
            {
                model = ItemLoyaltyGet(tenantId.Value, id);
            }

           
               
             
            return model;
        }

        public SpecificationsLoyaltyLogModel GetSpecificationsLoyaltyLog(long? id, int? tenantId = null)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }
            SpecificationsLoyaltyLogModel model = new SpecificationsLoyaltyLogModel();
            if (id==null)
            {
                model.LoyaltyDefinitionId=0;

            }
            else
            {
                model = SpecificationsLoyaltyGet(tenantId.Value, id);
            }




            return model;
        }

        public AdditionsLoyaltyLogModel GetAdditionsLoyaltyLog(long? id, int? tenantId = null)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }
            AdditionsLoyaltyLogModel model = new AdditionsLoyaltyLogModel();
            if (id==null)
            {
                model.LoyaltyDefinitionId=0;
                model.OriginalLoyaltyPoints = 0;
            }
            else
            {
                model = AdditionsLoyaltyGet(tenantId.Value, id);
            }




            return model;
        }


        public LoyaltyModel GetLoyaltyForMenu(int tenantId )
        {

            LoyaltyModel model = LoyaltyGet(tenantId);
          return model;
        }

        public decimal ConvertPriceToPoint(decimal? price)

        {
            decimal result = price.Value;

            if (price.HasValue) {
                var LoyaltyModel = GetAll();
                 if (LoyaltyModel.IsLoyalityPoint&& LoyaltyModel.IsOverrideUpdatedPrice)
                 result = (LoyaltyModel.ItemsPoints * price.Value)/(LoyaltyModel.ItemsCurrencyValue);
}
            return result;
        }

        public decimal ConvertPriceToPoints(decimal? price, decimal loyaltyPoints, decimal originalLoyaltyPoints, long loyaltyDefinitionId, LoyaltyModel loyaltyModel = null)

        {
            decimal result = 0;

            if (price.HasValue)
            {

                if (loyaltyModel==null)
                {
                    loyaltyModel = GetAll();
                }

                if (loyaltyModel.IsLoyalityPoint&& loyaltyModel.IsOverrideUpdatedPrice)
                    result = (loyaltyModel.ItemsPoints * price.Value)/(loyaltyModel.ItemsCurrencyValue);
                else
                    result=loyaltyPoints;


               // if (loyaltyDefinitionId == 0)
               // {
               //     if (loyaltyModel.IsLoyalityPoint)
               //         result = (loyaltyModel.ItemsPoints * price.Value)/(loyaltyModel.ItemsCurrencyValue);
               // }
               //else if (loyaltyModel.Id == loyaltyDefinitionId)
               // {
               //     if (loyaltyModel.IsLoyalityPoint&& loyaltyModel.IsOverrideUpdatedPrice)
               //         result = (loyaltyModel.ItemsPoints * price.Value)/(loyaltyModel.ItemsCurrencyValue);
               //     else
               //         result=loyaltyPoints;
               // }
               // else
               // {
               //     if (loyaltyModel.IsLoyalityPoint&& loyaltyModel.IsOverrideUpdatedPrice)
               //         result = (loyaltyModel.ItemsPoints * price.Value)/(loyaltyModel.ItemsCurrencyValue);
               //     else
               //         result=loyaltyPoints;
               // }
     
            }
           
           return Decimal.Ceiling(result);
        }
        public decimal ConvertPriceToCustomerPoints(decimal? price, decimal loyaltyPoints, decimal originalLoyaltyPoints, long loyaltyDefinitionId, LoyaltyModel loyaltyModel = null)
        {
            decimal result = 0;
            if (price.HasValue)
            {
                if (loyaltyModel == null)
                {
                    loyaltyModel = GetAll();
                }
                if (loyaltyModel.IsLoyalityPoint && loyaltyModel.IsOverrideUpdatedPrice)
                    result = (loyaltyModel.CustomerPoints * price.Value) / (loyaltyModel.CustomerCurrencyValue);
                else
                    result = loyaltyPoints;
            }
            return Decimal.Ceiling(result);
        }
        public decimal ConvertCustomerPriceToPoint(decimal? price, int? tenantId = null, LoyaltyModel LoyaltyModel=null)

        {
            decimal result = price.Value;

            if (LoyaltyModel == null)
            {
                LoyaltyModel = GetAll(tenantId);
            }

            if (price.HasValue)
            {
               //  LoyaltyModel = GetAll(tenantId);
                if (LoyaltyModel.IsLoyalityPoint)
                    result = (LoyaltyModel.CustomerPoints * price.Value)/(LoyaltyModel.CustomerCurrencyValue);
            }
            return result;
        }
        public decimal ConvertItemsPriceToPoint(decimal? price, decimal loyaltyPoints ,int? tenantId = null, LoyaltyModel LoyaltyModel = null)

        {
            decimal result = price.Value;

            if (LoyaltyModel == null)
            {
                LoyaltyModel = GetAll(tenantId);
            }

            if (price.HasValue)
            {
                //  LoyaltyModel = GetAll(tenantId);
                if (LoyaltyModel.IsLoyalityPoint && LoyaltyModel.IsOverrideUpdatedPrice)
                    result = (LoyaltyModel.ItemsPoints * price.Value) / (LoyaltyModel.ItemsCurrencyValue);
                else
                    result = loyaltyPoints;
            }
            return result;
        }



        public long CreateOrEdit(LoyaltyModel loyaltyModel)
        {
            _cacheManager.GetCache("CacheLoyalty").Remove("Loyalty_"+AbpSession.TenantId.ToString());

            return LoyaltyAdd(loyaltyModel);
        }
        public void CreateContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty)
        {
             createContactLoyaltyTransaction(contactLoyalty);
        }  
        public void UpdateContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty)
        {
            updateContactLoyaltyTransaction(contactLoyalty);
        }

       #region private
        private long LoyaltyAdd(LoyaltyModel model)
        {
            long id = 0;
            try
            {
                var SP_Name = Constants.Loyalty.SP_LoyaltyAdd;

                var sqlParameters = new List<SqlParameter>(){


                   new System.Data.SqlClient.SqlParameter("@CustomerPoints", model.CustomerPoints),
                   new System.Data.SqlClient.SqlParameter("@CustomerCurrencyValue", model.CustomerCurrencyValue),
                   new System.Data.SqlClient.SqlParameter("@ItemsPoints",model.ItemsPoints),
                   new System.Data.SqlClient.SqlParameter("@ItemsCurrencyValue", model.ItemsCurrencyValue),
                   new System.Data.SqlClient.SqlParameter("@IsOverrideUpdatedPrice", model.IsOverrideUpdatedPrice),
                  new System.Data.SqlClient.SqlParameter("@IsLatest", true),
                  new System.Data.SqlClient.SqlParameter("@IsLoyalityPoint", model.IsLoyalityPoint),
                   new System.Data.SqlClient.SqlParameter("@OrderType", model.OrderType),
                     new System.Data.SqlClient.SqlParameter("@LoyaltyExpiration", model.LoyaltyExpiration),
                      new System.Data.SqlClient.SqlParameter("@StartDate", model.StartDate),
                          new System.Data.SqlClient.SqlParameter("@EndDate", model.EndDate),
                   new System.Data.SqlClient.SqlParameter("@CreatedDate", DateTime.UtcNow),
                   new System.Data.SqlClient.SqlParameter("@CreatedBy", AbpSession.UserId),
                   new System.Data.SqlClient.SqlParameter("@TenantId", AbpSession.TenantId)
                    };

                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutputParameter.ParameterName = "@IdLoyalty";
                OutputParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);
                id=(long)OutputParameter.Value;

                return id;

            }
            catch (Exception ex)
            {
                throw ex;
               
            }
               
        }
        private void createContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty)
        {
            try
            {

                long CreatedBy = 0;

                if (contactLoyalty.CreatedBy==0)
                {
                    CreatedBy= (long)AbpSession.UserId;

                }
                int year = DateTime.UtcNow.Year;
                var SP_Name = Constants.Loyalty.SP_ContactLoyaltyTransactionAdd;

                var sqlParameters = new List<SqlParameter>{
                     new SqlParameter("@ContactId",contactLoyalty.ContactId)
                    ,new SqlParameter("@Points",contactLoyalty.Points)
                    ,new SqlParameter("@OrderId",contactLoyalty.OrderId)
                    ,new SqlParameter("@CreditPoints",contactLoyalty.CreditPoints)
                    ,new SqlParameter("@CreatedBy",contactLoyalty.CreatedBy)
                    ,new SqlParameter("@CreatedDate",DateTime.UtcNow)
                    ,new SqlParameter("@LoyaltyDefinitionId",contactLoyalty.LoyaltyDefinitionId)
                    ,new SqlParameter("@Year",year)
                    ,new SqlParameter("@TransactionTypeId",contactLoyalty.TransactionTypeId)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void updateContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty)
        {
            try
            {

                long CreatedBy = 0;

                if (contactLoyalty.CreatedBy == 0)
                {
                    CreatedBy = (long)AbpSession.UserId;

                }
                var SP_Name = Constants.Loyalty.SP_ContactLoyaltyTransactionUpdate;

                var sqlParameters = new List<SqlParameter>{
                     new SqlParameter("@ContactId",contactLoyalty.ContactId)
                    ,new SqlParameter("@OrderId",contactLoyalty.OrderId)
                    ,new SqlParameter("@CreatedBy",contactLoyalty.CreatedBy)
                    ,new SqlParameter("@TransactionTypeId",contactLoyalty.TransactionTypeId)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private void LoyaltyUpdate(LoyaltyModel model)
        {
            try
            {
                var SP_Name = Constants.Loyalty.SP_LoyaltyUpdate;

                var sqlParameters = new List<SqlParameter>(){
                      new SqlParameter("@Id", model.Id),
                   new SqlParameter("@CustomerPoints", model.CustomerPoints),
                   new SqlParameter("@CustomerCurrencyValue", model.CustomerCurrencyValue),
                   new SqlParameter("@ItemsPoints", model.ItemsPoints),
                   new SqlParameter("@ItemsCurrencyValue", model.ItemsCurrencyValue),
                   new SqlParameter("@IsOverrideUpdatedPrice", model.IsOverrideUpdatedPrice),
                   new SqlParameter("@IsLatest", model.IsLatest),
                   new SqlParameter("@TenantId", AbpSession.TenantId)
                    };

           

              
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);


              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoyaltyModel LoyaltyGet(int tenantId)
        {
            try
            {
                var SP_Name = Constants.Loyalty.SP_LoyaltyGet;

                var sqlParameters = new List<SqlParameter>(){
                   new SqlParameter("@TenantId",tenantId )
                    };

                LoyaltyModel loyalty = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                  DataReaderMapper.ConvertToLoyaltyDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return loyalty;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SpecificationsLoyaltyLogModel SpecificationsLoyaltyGet(int tenantId, long? id)
        {
            try
            {
                var SP_Name = Constants.Loyalty.SP_SpecificationsLoyaltyGet;

                var sqlParameters = new List<SqlParameter>(){
                   new SqlParameter("@TenantId",tenantId ),
                   new SqlParameter("@id",id )
                    };
                SpecificationsLoyaltyLogModel loyalty = new SpecificationsLoyaltyLogModel();

                loyalty = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToSpecificationLoyaltyDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                if (loyalty==null)
                {
                    loyalty=new SpecificationsLoyaltyLogModel();
                    loyalty.LoyaltyDefinitionId=0;
                    loyalty.OriginalLoyaltyPoints=0;
                }

                return loyalty;

            }
            catch (Exception ex)
            {


                throw ex;
            }
        }


        private AdditionsLoyaltyLogModel AdditionsLoyaltyGet(int tenantId, long? id)
        {
            try
            {
                var SP_Name = Constants.Loyalty.SP_AdditionsLoyaltyGet;

                var sqlParameters = new List<SqlParameter>(){
                   new SqlParameter("@TenantId",tenantId ),
                   new SqlParameter("@id",id )
                    };
                AdditionsLoyaltyLogModel loyalty = new AdditionsLoyaltyLogModel();

                loyalty = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToAdditionsLoyaltyDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                if (loyalty==null)
                {
                    loyalty=new AdditionsLoyaltyLogModel();
                    loyalty.LoyaltyDefinitionId=0;
                    loyalty.OriginalLoyaltyPoints=0;
                }

                return loyalty;

            }
            catch (Exception ex)
            {


                throw ex;
            }
        }

        private ItemsLoyaltyLogModel ItemLoyaltyGet(int tenantId,long? id)
        {
            try
            {
                var SP_Name = Constants.Loyalty.SP_ItemLoyaltyGet;

                var sqlParameters = new List<SqlParameter>(){
                   new SqlParameter("@TenantId",tenantId ),
                   new SqlParameter("@id",id )
                    };
                ItemsLoyaltyLogModel loyalty = new ItemsLoyaltyLogModel();

                 loyalty = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                  DataReaderMapper.ConvertToItemLoyaltyDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                if(loyalty==null)
                {
                    loyalty=new ItemsLoyaltyLogModel();
                    loyalty.LoyaltyDefinitionId=0;
                    loyalty.OriginalLoyaltyPoints=0;
                }

                return loyalty;

            }
            catch (Exception ex)
            {


               throw ex;
            }
        }





        #endregion
    }
}
