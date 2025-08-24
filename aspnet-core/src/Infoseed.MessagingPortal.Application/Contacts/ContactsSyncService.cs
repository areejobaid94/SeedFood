using Abp.Domain.Repositories;
using Framework.Data;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Contacts
{
    public class ContactsSyncService
    {
        private readonly IRepository<Contact> _contactRepository;

        // IDBService _dbService;
        private readonly IDocumentClient _IDocumentClient;

        public ContactsSyncService(IRepository<Contact> contactRepos,  IDocumentClient iDocumentClient
)
        {
            this._contactRepository = contactRepos;
            _IDocumentClient = iDocumentClient;

        }


        public async Task Sync(string connectionString)
        {
            // 1: get customers from CosmosDB
            //2: convert customers to contacts
            //3: loop on all customers to insert them into the database
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customers = await itemsCollection.GetAllItemsAsync(null, -1, p => p.IsNew == true);
            if (customers != null)
            {
                foreach (var item in customers.Item1)
                {


                    var cont = new Contact()
                    {
                         
                        UserId = item.userId,
                        DisplayName = item.displayName,
                        AvatarUrl = item.avatarUrl,
                        ChatStatuseId = item.CustomerChatStatusID,
                        CreatorUserId = 1,
                        Description = item.Description,
                        EmailAddress = item.EmailAddress,
                        IsDeleted = false,
                        CreationTime = DateTime.Now,
                        IsLockedByAgent = item.IsLockedByAgent,
                        IsConversationExpired = item.IsConversationExpired,
                        IsBlock = item.IsBlock,
                        IsOpen = item.IsOpen,
                        LockedByAgentName = item.LockedByAgentName,
                        PhoneNumber = item.phoneNumber,
                        SunshineAppID = item.SunshineAppID,
                        Website = item.Website,
                        TenantId = item.TenantId,
                        DeletionTime = null,
                        DeleterUserId = null
                    };
                    


                        InsertContact(cont, connectionString);

                  


                    item.IsNew = false;
                    await itemsCollection.UpdateItemAsync(item._self, item);
                }
            }

        }
        public async Task UpdateConversationExpired()
        {

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customers = await itemsCollection.GetAllItemsAsync(null, -1);


           // var lstCustomerModel = await _dbService.GetCustomersAsync(AbpSession.TenantId, model.PageNumber, model.PageSize, model.SearchTerm);

            foreach (var item in customers.Item1)
             {

                TimeSpan timeSpan = DateTime.Now - item.LastMessageData.Value ;
                int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                if (totalHours <= 24)
                {
                    item.IsConversationExpired = false;


                }
                else
                {       
                
                        item.IsConversationExpired = true;
                        item.IsOpen = false;
                        await itemsCollection.UpdateItemAsync(item._self, item);                  

                }

     


             }
            

        }
        public async Task Update(string connectionString)
        {
            // 1: get customers from CosmosDB
            //2: convert customers to contacts
            //3: loop on all customers to insert them into the database
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customers = await itemsCollection.GetAllItemsAsync(null, -1, p => p.IsNew == true);

            foreach (var item in customers.Item1)
            {


                var cont = new Contact()
                {
                    UserId = item.userId,
                    DisplayName = item.displayName,
                    AvatarUrl = item.avatarUrl,
                    ChatStatuseId = item.CustomerChatStatusID,
                    CreatorUserId = 1,
                    Description = item.Description,
                    EmailAddress = item.EmailAddress,
                    IsDeleted = false,
                    CreationTime = DateTime.Now,
                    IsLockedByAgent = item.IsLockedByAgent,
                    IsConversationExpired = item.IsConversationExpired,
                    IsBlock = item.IsBlock,
                    IsOpen = item.IsOpen,
                    LockedByAgentName = item.LockedByAgentName,
                    PhoneNumber = item.phoneNumber,
                    SunshineAppID = item.SunshineAppID,
                    Website = item.Website,
                    TenantId = item.TenantId,
                    DeletionTime = null,
                    DeleterUserId = null
                };



                Createcontact(cont, connectionString);
            }


        }
        public async Task CreateNewCustomer(Contact model)
        {
            string result = string.Empty;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var CustomerModel = new CustomerModel()
            {
                 IsComplaint=false,
                 IsBlock=false,
                userId = model.UserId,
                displayName = model.DisplayName,
                avatarUrl = model.AvatarUrl,
                type = "user",
                SunshineAppID = model.SunshineAppID,
                SunshineConversationId = "",
                CreateDate = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired=false,
                CustomerChatStatusID = (int)CustomerChatStatus.Active,
                CustomerStatusID = (int)CustomerStatus.Active,
                LastMessageData = DateTime.Now,
                IsNew = false,
                TenantId = model.TenantId,
                 loyalityPoint = model.loyalityPoint,
                  TakeAwayOrder = model.TakeAwayOrder,
                   DeliveryOrder = model.DeliveryOrder,
                   TotalOrder = model.TotalOrder ,

            };
            await itemsCollection.CreateItemAsync(CustomerModel);
        }


        private void Createcontact(Contact contact, string connectionString)
        {
            var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
        {
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@TenantId",
                 Value=contact.TenantId,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Int32
             },
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@AvatarUrl",
                 Value=contact.AvatarUrl,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@DisplayName",
                 Value=contact.DisplayName,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@PhoneNumber",
                 Value=contact.PhoneNumber,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@SunshineAppID",
                 Value=contact.SunshineAppID,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@IsLockedByAgent",
                 Value=contact.IsLockedByAgent,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Boolean
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@LockedByAgentName",
                 Value=contact.LockedByAgentName,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@IsOpen",
                 Value=contact.IsOpen,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Boolean
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@Website",
                 Value=contact.Website,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@EmailAddress",
                 Value=contact.EmailAddress,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@Description",
                 Value=contact.Description,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@ChatStatuseId",
                 Value=contact.ChatStatuseId,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Int32
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@ContactStatuseId",
                 Value=contact.ContactStatuseId,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Int32
             }
              ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@CreationTime",
                 Value=contact.CreationTime,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.DateTime
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@CreatorUserId",
                 Value=contact.CreatorUserId,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Int64,
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@DeleterUserId",
                 Value=contact.DeleterUserId,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Int64
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@DeletionTime",
                 Value=contact.DeletionTime,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.DateTime
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@IsDeleted",
                 Value=contact.IsDeleted,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Boolean
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@LastModificationTime",
                 Value=contact.LastModificationTime,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.DateTime
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@LastModifierUserId",
                 Value=contact.LastModifierUserId,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Int64
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@UserId",
                 Value=contact.UserId,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.String
             }
             ,
             new System.Data.SqlClient.SqlParameter()
             {
                 ParameterName= "@IsConversationExpired",
                 Value=contact.IsConversationExpired,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Boolean
             }
             ,
             new System.Data.SqlClient.SqlParameter( )
             {
                 ParameterName= "@IsBlock",
                 Value=contact.IsBlock,
                 Direction= System.Data.ParameterDirection.Input,
                 DbType= System.Data.DbType.Boolean
             }
        };



          SqlDataHelper.ExecuteNoneQuery(
                "dbo.ContactsAdd",
                sqlParameters.ToArray(),
               connectionString);


        }

        private void InsertContact(Contact contact, string connectionString)
        {

            string connString = connectionString;

            //return orderCount.Count().ToString();
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {


                command.CommandText = "INSERT INTO Contacts (TenantId, AvatarUrl, DisplayName, PhoneNumber, SunshineAppID, IsLockedByAgent, LockedByAgentName, IsOpen,Website,EmailAddress,Description,ChatStatuseId,ContactStatuseId,CreationTime,CreatorUserId,DeleterUserId,DeletionTime,IsDeleted,LastModificationTime,LastModifierUserId,UserId,IsConversationExpired,IsBlock) " +
                    " VALUES (@TenantId, @AvatarUrl, @DisplayName, @PhoneNumber, @SunshineAppID, @IsLockedByAgent, @LockedByAgentName, @IsOpen , @Website, @EmailAddress, @Description, @ChatStatuseId, @ContactStatuseId, @CreationTime, @CreatorUserId, @DeleterUserId, @DeletionTime, @IsDeleted, @LastModificationTime, @LastModifierUserId, @UserId, @IsConversationExpired, @IsBlock)";

                command.Parameters.AddWithValue("@TenantId", contact.TenantId);
                command.Parameters.AddWithValue("@AvatarUrl", contact.AvatarUrl  ?? Convert.DBNull);
                command.Parameters.AddWithValue("@DisplayName", contact.DisplayName);
                command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                command.Parameters.AddWithValue("@SunshineAppID", contact.SunshineAppID);
                command.Parameters.AddWithValue("@IsLockedByAgent", contact.IsLockedByAgent);
                command.Parameters.AddWithValue("@LockedByAgentName", contact.LockedByAgentName ?? Convert.DBNull);
                command.Parameters.AddWithValue("@IsOpen", contact.IsOpen);
                command.Parameters.AddWithValue("@Website", contact.Website ?? Convert.DBNull);

                command.Parameters.AddWithValue("@EmailAddress", contact.EmailAddress ?? Convert.DBNull);
                command.Parameters.AddWithValue("@Description", contact.Description ?? Convert.DBNull);
                command.Parameters.AddWithValue("@ChatStatuseId", Convert.DBNull);
                command.Parameters.AddWithValue("@ContactStatuseId", Convert.DBNull);
                command.Parameters.AddWithValue("@CreationTime", contact.CreationTime);
                command.Parameters.AddWithValue("@CreatorUserId", Convert.DBNull);
                command.Parameters.AddWithValue("@DeleterUserId", Convert.DBNull);
                command.Parameters.AddWithValue("@DeletionTime", Convert.DBNull);
                command.Parameters.AddWithValue("@IsDeleted", contact.IsDeleted);

                command.Parameters.AddWithValue("@LastModificationTime", Convert.DBNull);
                command.Parameters.AddWithValue("@LastModifierUserId", Convert.DBNull);
                command.Parameters.AddWithValue("@UserId", contact.UserId);
                command.Parameters.AddWithValue("@IsConversationExpired", contact.IsConversationExpired);
                command.Parameters.AddWithValue("@IsBlock", contact.IsBlock);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }


        }

    }

  




}
