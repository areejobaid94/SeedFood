using Framework.Data;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.SealingReuest.Dto;
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

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@Stop", behaviourModel.Stop),
                    new SqlParameter("@Start", behaviourModel.Start),
                    new SqlParameter("@ContactId", behaviourModel.ContactId),
                    new SqlParameter("@TenantID", behaviourModel.TenantID),
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

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@levleOneId", interestedOf.levleOneId),
                    new SqlParameter("@levelTwoId", interestedOf.levelTwoId),
                    new SqlParameter("@levelThreeId", interestedOf.levelThreeId),
                    new SqlParameter("@ContactId", interestedOf.ContactId),
                    new SqlParameter("@TenantID", interestedOf.TenantID),
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
                var sqlParameters = new List<SqlParameter>
                {
               new SqlParameter("@Id",id)
               ,new SqlParameter("@ContactDisplayName",name)
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
                var sqlParameters = new List<SqlParameter>
                {
               new SqlParameter("@Id",id)
               ,new SqlParameter("@ContactkinshipName",name)
                 };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

     private void updateCustomerBehaviorByUser(CustomerBehaviourModel behaviourModel)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_CustomerBehaviourUpdateByUser;

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantID", behaviourModel.TenantID),
                    new SqlParameter("@ContactId", behaviourModel.ContactId),
                    new SqlParameter("@CustomerOPT", behaviourModel.CustomerOPt)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
    }
