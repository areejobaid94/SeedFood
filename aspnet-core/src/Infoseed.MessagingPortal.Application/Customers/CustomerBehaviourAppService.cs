using Framework.Data;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using static Infoseed.MessagingPortal.Customers.Dtos.CustomerBehaviourEnums;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;

namespace Infoseed.MessagingPortal.Customers
{
    public class CustomerBehaviourAppService : MessagingPortalAppServiceBase, ICustomerBehaviourAppService
    {
        private readonly IDashboardUIAppService _dashboardUIAppService;
        private readonly IDocumentClient _IDocumentClient;

        public CustomerBehaviourAppService(IDashboardUIAppService dashboardUIAppService, IDocumentClient iDocumentClient)
        {
            _dashboardUIAppService = dashboardUIAppService;
            _IDocumentClient = iDocumentClient;

        }

        public CustomerBehaviourAppService(IDashboardUIAppService dashboardUIAppService)
        {
            _dashboardUIAppService = dashboardUIAppService;
        }

        public void UpdateCustomerBehavior(CustomerBehaviourModel behaviourModel)
        {
            updateCustomerBehavior(behaviourModel);

        }
        public void UpdateCustomerBehaviorByUser(CustomerBehaviourModel behaviourModel)
        {
            updateCustomerBehaviorByUser(behaviourModel);

        }
        public void CreateInterestedOf(CustomerInterestedOf interestedOf)
        {
            createInterestedOf(interestedOf);
        }
        public void UpdateContactName(int id, string name)
        {
            updateContactName(id, name);
        }
        public void UpdateContactkinship(int id, string name)
        {
            updateContactkinship(id, name);
        }

        #region Private Methods

        private void updateCustomerBehavior(CustomerBehaviourModel behaviourModel)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_CustomerBehaviourUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Stop", behaviourModel.Stop),
                    new System.Data.SqlClient.SqlParameter("@Start", behaviourModel.Start),
                    new System.Data.SqlClient.SqlParameter("@ContactId", behaviourModel.ContactId),
                    new System.Data.SqlClient.SqlParameter("@TenantID", behaviourModel.TenantID),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                _dashboardUIAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = behaviourModel.TenantID.Value,
                    TypeId = (int)DashboardTypeEnum.Contact,
                    StatusId = behaviourModel.CustomerOPt,
                    StatusName = Enum.GetName(typeof(SellingRequestStatus), behaviourModel.CustomerOPt)
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void createInterestedOf(CustomerInterestedOf interestedOf)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsInterrestedOfAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@levleOneId", interestedOf.levleOneId),
                    new System.Data.SqlClient.SqlParameter("@levelTwoId", interestedOf.levelTwoId),
                    new System.Data.SqlClient.SqlParameter("@levelThreeId", interestedOf.levelThreeId),
                    new System.Data.SqlClient.SqlParameter("@ContactId", interestedOf.ContactId),
                    new System.Data.SqlClient.SqlParameter("@TenantID", interestedOf.TenantID),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void updateContactName(int id, string name)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
               new System.Data.SqlClient.SqlParameter("@Id",id)
               ,new System.Data.SqlClient.SqlParameter("@ContactDisplayName",name)
                 };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void updateContactkinship(int id, string name)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactkinshipUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
               new System.Data.SqlClient.SqlParameter("@Id",id)
               ,new System.Data.SqlClient.SqlParameter("@ContactkinshipName",name)
                 };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private async void updateCustomerBehaviorByUser(CustomerBehaviourModel behaviourModel)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_CustomerBehaviourUpdateByUser;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantID", behaviourModel.TenantID),
                    new System.Data.SqlClient.SqlParameter("@ContactId", behaviourModel.ContactId),
                    new System.Data.SqlClient.SqlParameter("@CustomerOPT", behaviourModel.CustomerOPt)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                // Get the collection
                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                // Find the customer in Cosmos DB by phone number (or userId)
                var customerResult = await itemsCollection.GetItemAsync(c =>
                          c.TenantId == behaviourModel.TenantID &&
                          c.ContactID == behaviourModel.ContactId.ToString()
                      );

                var customer = customerResult;
                if (customer != null)
                {
                    // Apply the same logic as the SQL SP
                    if (behaviourModel.CustomerOPt == 1)
                    {
                        customer.CustomerOPT = 0;
                    }
                    else if (behaviourModel.CustomerOPt == 0 || behaviourModel.CustomerOPt == 2)
                    {
                        customer.CustomerOPT = 1;
                    }

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
    }