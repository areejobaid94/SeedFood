using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Framework.Data;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.ItemAdditionsCategorys;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.ItemSpecificationsDetails;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Specifications.Dtos;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ItemAdditions
{
    //[AbpAuthorize(AppPermissions.Pages_MenuCategories)]
    public class ItemAdditionAppService : MessagingPortalAppServiceBase, IItemAdditionAppService
    {
        private readonly IRepository<Item, long> _itemRepository;
        private readonly IRepository<ItemAdditions, long> _itemAdditionsRepository;

        private readonly IRepository<ItemAdditionsCategory, long> _itemAdditionsCategoryRepository;
        private readonly IRepository<Area, long> _areaRepository;
        private ILoyaltyAppService _loyaltyAppService;

        public ItemAdditionAppService(IRepository<Item, long> itemRepository, IRepository<ItemAdditions, long> itemAdditionsRepository, IRepository<ItemAdditionsCategory, long> itemAdditionsCategoryRepository, IRepository<Area, long> areaRepository, ILoyaltyAppService loyaltyAppService)
        {
            _itemRepository = itemRepository;
            _itemAdditionsRepository = itemAdditionsRepository;
            _itemAdditionsCategoryRepository = itemAdditionsCategoryRepository;
            _areaRepository = areaRepository;
            _loyaltyAppService = loyaltyAppService;
        }

        public bool DeleteItemAddition(long id)
        {
            return deleteItemAddition(id);
        }
        public async Task<PagedResultDto<GetItemAdditionForViewDto>> GetAll(GetAllItemAdditionInput input)
        {
            try
            {
                var filteredItemsAddition = _itemAdditionsRepository.GetAll()
                     .Include(e => e.ItemFk)
                      .Include(e => e.ItemAdditionsCategoryFk)
                      .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                     .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.NameEnglish == input.NameFilter);


                var itemAddition = from o in filteredItemsAddition
                                   select new GetItemAdditionForViewDto()
                                   {
                                       itemAdditionDto = new ItemAdditionDto
                                       {
                                           Id = o.Id,
                                           Name = o.Name,
                                           NameEnglish = o.NameEnglish,
                                           itemId = o.ItemId,
                                           price = o.Price,
                                           SKU = o.SKU,
                                           MenuType = o.MenuType,
                                           ItemAdditionsCategoryId = o.ItemAdditionsCategoryId


                                       }
                                   };


                var totalCount = filteredItemsAddition.Count();
                var list = filteredItemsAddition.ToList();

                return new PagedResultDto<GetItemAdditionForViewDto>(totalCount, itemAddition.ToList());
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public async Task<long> CreateOrEdit(CreateOrEditItemAdditionDto input)
        {
            try
            {
                if (input.Id == null || input.Id == 0)
                {
                    return await Create(input);
                }
                else
                {
                    return await Update(input);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        private long AddAdditionsModel(AdditionsModelDto Model)
        {
            try
            {
                var SP_Name = "[dbo].[AdditionsAdd]";
                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@itemAdditionsCategory",JsonConvert.SerializeObject(Model.createOrEditItemAdditionCategoryDto))
                , new SqlParameter("@itemAdditionsChoicesJson",JsonConvert.SerializeObject(Model.itemAdditions))
                  , new SqlParameter("@tenantId",(int?)AbpSession.TenantId)
                  , new SqlParameter("@LoyaltyDefinitionId",Model.LoyaltyDefinitionId)
                  , new SqlParameter("@CreatedBy",AbpSession.UserId)
                   //, new SqlParameter("@createdBy",AbpSession.UserId)
                   // , new SqlParameter("@loyaltyDefinitionId",(int?)Model.LoyaltyDefinitionId)

            };
                SqlParameter OutsqlParameter = new SqlParameter();
                OutsqlParameter.ParameterName = "@itemAdditionsCategoryId";
                OutsqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutsqlParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (long)OutsqlParameter.Value;



            }
            catch (Exception ex)
            {
                throw ex; ;
            }


        }
        private void updateAdditionsModel(AdditionsModelDto Model)
        {
            try
            {
                var SP_Name = "[dbo].[AdditionsUpdate]";
                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@itemAdditionsCategory",JsonConvert.SerializeObject(Model.createOrEditItemAdditionCategoryDto))
                , new SqlParameter("@itemAdditionsChoicesJson",JsonConvert.SerializeObject(Model.itemAdditions))
                 , new SqlParameter("@itemAdditionsCategoryId",Model.createOrEditItemAdditionCategoryDto.Id)
                  , new SqlParameter("@tenantId",(int?)AbpSession.TenantId)
                  , new SqlParameter("@LoyaltyDefinitionId",Model.LoyaltyDefinitionId)
                  , new SqlParameter("@CreatedBy",AbpSession.UserId)

                  //, new SqlParameter("@createdBy",AbpSession.UserId)
                  //  , new SqlParameter("@loyaltyDefinitionId",Model.LoyaltyDefinitionId)

            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);


            }
            catch (Exception ex)
            {
                throw ex; ;
            }


        }



        public long DeleteItemAdditionCategory(long input)
        {

            return deleteItemAdditionCategory(input);

        }
        public bool DeleteSpecification(int input)
        {
            return deleteSpecification(input);
        }


        private List<ItemSpecificationsDto> GetItemSpecifications(int? TenantID)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemSpecifications] where TenantId=" + TenantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<ItemSpecificationsDto> itemSpecifications = new List<ItemSpecificationsDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    itemSpecifications.Add(new ItemSpecificationsDto
                    {
                        Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                        ItemId = int.Parse(dataSet.Tables[0].Rows[i]["ItemId"].ToString()),
                        IsRequired = bool.Parse(dataSet.Tables[0].Rows[i]["IsRequired"].ToString()),
                        SpecificationId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationId"].ToString()),
                        TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString())

                    });
                }

                conn.Close();
                da.Dispose();

                return itemSpecifications;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

        private List<SpecificationChoicesDto> GetSpecificationChoices(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[SpecificationChoices] where TenantId=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<SpecificationChoicesDto> specificationChoices = new List<SpecificationChoicesDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                specificationChoices.Add(new SpecificationChoicesDto
                {
                    Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                    SpecificationChoiceDescription = dataSet.Tables[0].Rows[i]["SpecificationChoiceDescription"].ToString(),
                    SpecificationChoiceDescriptionEnglish = dataSet.Tables[0].Rows[i]["SpecificationChoiceDescriptionEnglish"].ToString(),
                    Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString() ?? "",
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                    SpecificationId = int.Parse(dataSet.Tables[0].Rows[i]["SpecificationId"].ToString())
                });
            }

            conn.Close();
            da.Dispose();

            return specificationChoices;

        }

        private List<SpecificationsDto> GetSpecifications(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Specifications] where TenantId=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<SpecificationsDto> specifications = new List<SpecificationsDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                specifications.Add(new SpecificationsDto
                {
                    Id = int.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                    Priority = int.Parse(dataSet.Tables[0].Rows[i]["Priority"].ToString()),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                    IsMultipleSelection = bool.Parse(dataSet.Tables[0].Rows[i]["IsMultipleSelection"].ToString()),
                    SpecificationDescription = dataSet.Tables[0].Rows[i]["SpecificationDescription"].ToString(),
                    MaxSelectNumber = int.Parse(dataSet.Tables[0].Rows[i]["MaxSelectNumber"].ToString()),
                    SpecificationDescriptionEnglish = dataSet.Tables[0].Rows[i]["SpecificationDescriptionEnglish"].ToString()

                });
            }

            conn.Close();
            da.Dispose();

            return specifications;

        }



        public void DeleteItemSpecifications(long? id)
        {
            try 
            {

                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "DELETE FROM ItemSpecifications Where Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            catch(Exception ex)
            {
               throw ex;
            }
           

        }
        private bool deleteSpecification(int specificationId)
        {
            try 
            {

                var SP_Name = Constants.Menu.SP_SpecificationDelete;
                var sqlParameters = new List<SqlParameter>
                {
                   new SqlParameter("@SpecificationId",specificationId),
                };

                var OutputParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;
            } 
            catch(Exception ex)
            {
                throw ex;
            }
           

        }
        [HttpGet]
        public bool CheckIfSpecificationConnectedToItem(int specificationId)
        {
            try
            {
                var SP_Name = Constants.Menu.SP_SpecificationConnectedToItemCheck;
                var sqlParameters = new List<SqlParameter>
                {
                   new SqlParameter("@SpecificationId",specificationId),
                };

                var OutputParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                bool result = (bool)OutputParameter.Value;
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void DeleteSpecificationChoices(long? id)
        {


            try
            {
                var ditList = GetItemSpecificationsDetailList((int)AbpSession.TenantId);

                var listaddList = ditList.Where(x => x.SpecificationChoicesId == id);

                foreach (var add in listaddList)
                {

                    DeleteItemSpecificationsDetails(add.Id);

                }

            }
            catch(Exception ex)
            {
                throw ex;

            }
            try 
            {
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "DELETE FROM SpecificationChoices   Where Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }


        }
        private void DeleteItemSpecificationsDetails(long? id)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "DELETE FROM ItemSpecificationsDetail   Where Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void updateItemSpecifications(ItemSpecificationsDto input)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE ItemSpecifications SET  ItemId = @ItemId , SpecificationId = @SpecificationId , IsRequired = @IsRequired  Where Id = @Id";
                    command.Parameters.AddWithValue("@Id", input.Id);
                    command.Parameters.AddWithValue("@SpecificationId", input.SpecificationId);
                    command.Parameters.AddWithValue("@ItemId", input.ItemId);
                    command.Parameters.AddWithValue("@IsRequired", input.IsRequired);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private void updateSpecificationsModel(SpecificationsModelDto Model)
        {
            try
            {
                var SP_Name = "[dbo].[SpecificationsUpdate]";
                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@specificationsJson",JsonConvert.SerializeObject(Model.specificationsDto))
                , new SqlParameter("@specificationChoicesJson",JsonConvert.SerializeObject(Model.specificationChoicesDtos))
                 , new SqlParameter("@specificationId",Model.specificationsDto.Id)
                  , new SqlParameter("@tenantId",(int?)AbpSession.TenantId)

                  , new SqlParameter("@LoyaltyDefinitionId",Model.LoyaltyDefinitionId)
                  , new SqlParameter("@CreatedBy",AbpSession.UserId)

            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

              
            }
            catch (Exception ex)
            {
                throw ex; ;
            }


        }
        private void updateSpecifications(SpecificationsDto input)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Specifications SET  SpecificationDescription = @SpecificationDescription,"
                        + "  IsMultipleSelection = @IsMultipleSelection ,"
                        + "  LanguageBotId = @LanguageBotId ,"
                        + "  SpecificationDescriptionEnglish = @SpecificationDescriptionEnglish ,"
                        + "  Priority = @Priority ,"
                        + "  MaxSelectNumber = @MaxSelectNumber "
                        + "  Where Id = @Id";
                    command.Parameters.AddWithValue("@Id", input.Id);
                    command.Parameters.AddWithValue("@SpecificationDescription", input.SpecificationDescription);
                    command.Parameters.AddWithValue("@SpecificationDescriptionEnglish", input.SpecificationDescriptionEnglish);
                    command.Parameters.AddWithValue("@Priority", input.Priority);
                    command.Parameters.AddWithValue("@LanguageBotId", input.LanguageBotId);
                    command.Parameters.AddWithValue("@IsMultipleSelection", input.IsMultipleSelection);
                    command.Parameters.AddWithValue("@MaxSelectNumber", input.MaxSelectNumber);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private void updateSpecificationChoices(string input)
        {
            //try
            //{
            //    string connString = AppSettingsModel.ConnectionStrings;
            //    using (SqlConnection connection = new SqlConnection(connString))
            //    using (SqlCommand command = connection.CreateCommand())
            //    {
            //        command.CommandText = "UPDATE SpecificationChoices SET  SpecificationId = @SpecificationId,"
            //           + "  SpecificationChoiceDescription = @SpecificationChoiceDescription ,"
            //           + "  SpecificationChoiceDescriptionEnglish = @SpecificationChoiceDescriptionEnglish ,"
            //           + "  LanguageBotId = @LanguageBotId ,"
            //           + "  SKU = @SKU, "

            //            + "  LoyaltyPoints = @LoyaltyPoints, "
            //             + "  OriginalLoyaltyPoints = @OriginalLoyaltyPoints, "
            //              + "  IsOverrideLoyaltyPoints = @IsOverrideLoyaltyPoints, "

            //           + "  Price = @Price "
            //           + "  Where Id = @Id";
            //        command.Parameters.AddWithValue("@Id", input.Id);
            //        command.Parameters.AddWithValue("@SpecificationId", input.SpecificationId);
            //        command.Parameters.AddWithValue("@SpecificationChoiceDescription", input.SpecificationChoiceDescription);
            //        command.Parameters.AddWithValue("@SpecificationChoiceDescriptionEnglish", input.SpecificationChoiceDescriptionEnglish);
            //        command.Parameters.AddWithValue("@LanguageBotId", input.LanguageBotId);
            //        command.Parameters.AddWithValue("@SKU", input.SKU);
            //        command.Parameters.AddWithValue("@Price", input.Price);

            //        command.Parameters.AddWithValue("@LoyaltyPoints", input.LoyaltyPoints);
            //        command.Parameters.AddWithValue("@OriginalLoyaltyPoints", input.OriginalLoyaltyPoints);
            //        command.Parameters.AddWithValue("@IsOverrideLoyaltyPoints", input.IsOverrideLoyaltyPoints);


            //        connection.Open();
            //        command.ExecuteNonQuery();
            //        connection.Close();
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
            
        }


        private int CreateItemSpecifications(ItemSpecificationsDto deliveryLocationCost)
        {
            try
            {
                int modified = 0;
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))

                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "INSERT INTO ItemSpecifications (ItemId , SpecificationId, TenantId, IsRequired) VALUES (@ItemId ,@SpecificationId, @TenantId, @IsRequired) ;SELECT SCOPE_IDENTITY();  ";

                    // command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
                    command.Parameters.AddWithValue("@ItemId", deliveryLocationCost.ItemId);
                    command.Parameters.AddWithValue("@SpecificationId", deliveryLocationCost.SpecificationId);
                    command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
                    command.Parameters.AddWithValue("@IsRequired", deliveryLocationCost.IsRequired);


                    connection.Open();


                    modified = Convert.ToInt32(command.ExecuteScalar());
                    if (connection.State == System.Data.ConnectionState.Open) connection.Close();


                    return modified;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private long CreateSpecificationsModel(SpecificationsModelDto Model)
        {


            try
            {
                var SP_Name = "[dbo].[SpecificationsAdd]";
                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@specificationsJson",JsonConvert.SerializeObject(Model.specificationsDto))
                , new SqlParameter("@specificationChoicesJson",JsonConvert.SerializeObject(Model.specificationChoicesDtos))
                , new SqlParameter("@tenantId",(int?)AbpSession.TenantId)
                , new SqlParameter("@LoyaltyDefinitionId",Model.LoyaltyDefinitionId)
                , new SqlParameter("@CreatedBy",AbpSession.UserId)
            };



                SqlParameter OutsqlParameter = new SqlParameter();
                OutsqlParameter.ParameterName = "@specificationId";
                OutsqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutsqlParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (long)OutsqlParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex; ;
            }

        }
        private int CreateSpecifications(SpecificationsDto deliveryLocationCost)
        {

            
                try
                {
                    int modified = 0;
                    string connString = AppSettingsModel.ConnectionStrings;
                    using (SqlConnection connection = new SqlConnection(connString))
                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO Specifications (SpecificationDescription , IsMultipleSelection, LanguageBotId, SpecificationDescriptionEnglish , TenantId ,Priority , CreationTime , IsDeleted ,MaxSelectNumber) VALUES (@SpecificationDescription ,@IsMultipleSelection, @LanguageBotId, @SpecificationDescriptionEnglish , @TenantId ,@Priority ,@CreationTime ,@IsDeleted ,@MaxSelectNumber) ;SELECT SCOPE_IDENTITY(); ";

                        // command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
                        command.Parameters.AddWithValue("@SpecificationDescription", deliveryLocationCost.SpecificationDescription);
                        command.Parameters.AddWithValue("@IsMultipleSelection", deliveryLocationCost.IsMultipleSelection);
                        command.Parameters.AddWithValue("@MaxSelectNumber", deliveryLocationCost.MaxSelectNumber);
                        command.Parameters.AddWithValue("@LanguageBotId", 1);
                        command.Parameters.AddWithValue("@SpecificationDescriptionEnglish", deliveryLocationCost.SpecificationDescriptionEnglish);

                        command.Parameters.AddWithValue("@CreationTime", DateTime.Now);
                        command.Parameters.AddWithValue("@IsDeleted", false);

                        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
                        command.Parameters.AddWithValue("@Priority", deliveryLocationCost.Priority);


                        connection.Open();


                        modified = Convert.ToInt32(command.ExecuteScalar());
                        if (connection.State == System.Data.ConnectionState.Open) connection.Close();


                        return modified;
                    }
                }
                catch (Exception ex)
                {
                    throw ex; ;
                }

        }

        private long CreateSpecificationChoicesDto(string specificationChoices)
        {
            try
            {

                var SP_Name = "[dbo].[SpecificationChoicesAdd]";
                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@specificationChoicesJson",specificationChoices)

            };



                SqlParameter OutsqlParameter = new SqlParameter();
                OutsqlParameter.ParameterName = "@specificationChoicesId";
                OutsqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                OutsqlParameter.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(OutsqlParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (long)OutsqlParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //try
            //{
            //    int modified = 0;
            //    string connString = AppSettingsModel.ConnectionStrings;
            //    using (SqlConnection connection = new SqlConnection(connString))
            //    using (SqlCommand command = connection.CreateCommand())
            //    {

            //        command.CommandText = "INSERT INTO SpecificationChoices (SpecificationId , SpecificationChoiceDescription, TenantId, LanguageBotId ,Price ,SKU , SpecificationChoiceDescriptionEnglish ,LoyaltyPoints ,OriginalLoyaltyPoints ,IsOverrideLoyaltyPoints ) VALUES (@SpecificationId ,@SpecificationChoiceDescription, @TenantId, @LanguageBotId ,@Price ,@SKU,@SpecificationChoiceDescriptionEnglish ,@LoyaltyPoints,@OriginalLoyaltyPoints,@IsOverrideLoyaltyPoints );SELECT SCOPE_IDENTITY();  ";

            //        // command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
            //        command.Parameters.AddWithValue("@SpecificationId", deliveryLocationCost.SpecificationId);
            //        command.Parameters.AddWithValue("@SpecificationChoiceDescription", deliveryLocationCost.SpecificationChoiceDescription);
            //        command.Parameters.AddWithValue("@SpecificationChoiceDescriptionEnglish", deliveryLocationCost.SpecificationChoiceDescriptionEnglish);
            //        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
            //        command.Parameters.AddWithValue("@LanguageBotId", deliveryLocationCost.LanguageBotId);
            //        command.Parameters.AddWithValue("@Price", deliveryLocationCost.Price);
            //        command.Parameters.AddWithValue("@SKU", deliveryLocationCost.SKU);

            //    command.Parameters.AddWithValue("@LoyaltyPoints", deliveryLocationCost.LoyaltyPoints);
            //    command.Parameters.AddWithValue("@OriginalLoyaltyPoints", deliveryLocationCost.OriginalLoyaltyPoints);
            //    command.Parameters.AddWithValue("@IsOverrideLoyaltyPoints", deliveryLocationCost.IsOverrideLoyaltyPoints);

            //    connection.Open();


            //        modified = Convert.ToInt32(command.ExecuteScalar());
            //        if (connection.State == System.Data.ConnectionState.Open) connection.Close();


            //        return modified;
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}

        }
        public async Task<long> CreateOrEditItemSpecifications(ItemSpecificationsDto input)
        {
            try
            {
                if (input.Id == 0)
                {
                    input.TenantId = (int?)AbpSession.TenantId;
                    return CreateItemSpecifications(input);
                }
                else
                {
                    input.TenantId = (int?)AbpSession.TenantId;
                    updateItemSpecifications(input);
                    return input.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public async Task<long> CreateOrEditSpecificationsModel(SpecificationsModelDto input)
        {
            try
            {

                input.LoyaltyDefinitionId= _loyaltyAppService.GetAll().Id;
                input.CreatedBy=AbpSession.UserId.Value;


                foreach (var item in input.specificationChoicesDtos)
                {
                    var GetSpecificationsLoyaltyLog = _loyaltyAppService.GetSpecificationsLoyaltyLog(item.Id);

                    item.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(item.Price, item.LoyaltyPoints, GetSpecificationsLoyaltyLog.OriginalLoyaltyPoints, GetSpecificationsLoyaltyLog.LoyaltyDefinitionId);

                    item.LoyaltyDefinitionId=_loyaltyAppService.GetAll().Id;
                    item.CreatedBy=AbpSession.UserId.Value;

                }

                if (input.specificationsDto.Id == 0)
                {
                    input.specificationsDto.TenantId = (int?)AbpSession.TenantId;
                    return CreateSpecificationsModel(input);
                }
                else
                {
                    input.specificationsDto.TenantId = (int?)AbpSession.TenantId;
                      updateSpecificationsModel(input);

                    return input.specificationsDto.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public async Task<long> CreateOrEditSpecifications(SpecificationsDto input)
        {
            try
            {
                if (input.Id == 0)
                {
                    input.TenantId = (int?)AbpSession.TenantId;
                    return CreateSpecifications(input);
                }
                else
                {
                    input.TenantId = (int?)AbpSession.TenantId;
                    updateSpecifications(input);

                    return input.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<long> CreateOrEditSpecificationChoices(List<SpecificationChoicesDto> input)
        {
            try
            {


                //foreach (var x in input)
                //{
                //    if (x.OriginalLoyaltyPoints==null || x.OriginalLoyaltyPoints==0)
                //        x.OriginalLoyaltyPoints=0;
                //    if (x.IsOverrideLoyaltyPoints==null)
                //        x.IsOverrideLoyaltyPoints=false;


                //    x.TenantId = (int?)AbpSession.TenantId;
                //   // x.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(x.Price, x.LoyaltyPoints, x.OriginalLoyaltyPoints, _loyaltyAppService.GetAll().Id);

                //}
                var entityId = CreateSpecificationChoicesDto(JsonConvert.SerializeObject(input));


                return entityId;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<long> CreateItemAdditionCategory(AdditionsModelDto input)
        {
            try
            {
               

                input.LoyaltyDefinitionId= _loyaltyAppService.GetAll().Id;
                input.CreatedBy=AbpSession.UserId.Value;


                foreach (var item in input.itemAdditions)
                {
                    var GetAdditionsLoyaltyLog = _loyaltyAppService.GetAdditionsLoyaltyLog(item.Id);

                    item.LoyaltyPoints =_loyaltyAppService.ConvertPriceToPoints(item.price, item.LoyaltyPoints, GetAdditionsLoyaltyLog.OriginalLoyaltyPoints, GetAdditionsLoyaltyLog.LoyaltyDefinitionId);

                    item.LoyaltyDefinitionId=_loyaltyAppService.GetAll().Id;
                    item.CreatedBy=AbpSession.UserId.Value;

                }

                if (input.createOrEditItemAdditionCategoryDto.Id == 0)
                {

                    var entityIds = AddAdditionsModel(input);



                    return entityIds;
                }
                else
                {
                        updateAdditionsModel(input);

                    return input.createOrEditItemAdditionCategoryDto.Id;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }





        }


        protected virtual async Task<long> Create(CreateOrEditItemAdditionDto input)
        {


            try
            {
                var ItemAdditions = new ItemAdditions();

                ItemAdditions.Name = input.Name;
                ItemAdditions.NameEnglish = input.NameEnglish;
                ItemAdditions.Price = input.Price;
                ItemAdditions.TenantId = (int?)AbpSession.TenantId;
                ItemAdditions.ItemAdditionsCategoryId = input.ItemAdditionsCategoryId;
                ItemAdditions.ItemId = null;
                ItemAdditions.MenuType = input.MenuType;
                ItemAdditions.ImageUri = input.ImageUri;

                ItemAdditions.LoyaltyPoints = input.LoyaltyPoints;
                ItemAdditions.OriginalLoyaltyPoints = input.OriginalLoyaltyPoints;
                ItemAdditions.IsOverrideLoyaltyPoints = input.IsOverrideLoyaltyPoints;

                var entityId = await _itemAdditionsRepository.InsertAndGetIdAsync(ItemAdditions);



                //  createDetAsync(input, itemAdditionDetailsModel);

                return entityId;
            }
            catch (Exception ex)
            {

                throw ex;
            }




        }
        public void createDet(int AddID)
        {
            try
            {
                var ditlist = GetItemAdditionDetailsList((int)AbpSession.TenantId);
                var list = ditlist.Where(x => x.MenuType == 0 && x.ItemAdditionId == AddID).ToList();


                ItemAdditionDetailsModel itemAdditionDetailsModel = new ItemAdditionDetailsModel
                {
                    IsInService = true,
                    ItemAdditionId = AddID,
                    MenuType = 0,
                    TenantId = (int)AbpSession.TenantId


                };


                if (list.Count == 0)
                {

                    var x = CreateItemAdditionDetails(itemAdditionDetailsModel);//for menu 0
                }

                var Areas = _areaRepository.GetAll().ToList();
                foreach (var item in Areas)
                {

                    var xx = ditlist.Where(x => x.ItemAdditionId == AddID && x.MenuType == item.Id).FirstOrDefault();
                    if (xx == null)
                    {
                        itemAdditionDetailsModel.MenuType = int.Parse(item.Id.ToString());


                        CreateItemAdditionDetails(itemAdditionDetailsModel);
                    }


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

        public void createpecificationsDet(int spsID)
        {
            try
            {
                var ditlist = GetItemSpecificationsDetailList((int)AbpSession.TenantId);
                var list = ditlist.Where(x => x.MenuType == 0 && x.SpecificationChoicesId == spsID).ToList();


                ItemSpecificationsDetail itemAdditionDetailsModel = new ItemSpecificationsDetail
                {
                    IsInService = true,
                    SpecificationChoicesId = spsID,
                    MenuType = 0,
                    TenantId = (int)AbpSession.TenantId


                };


                if (list.Count == 0)
                {

                    var x = CreateItemSpecificationsDetail(itemAdditionDetailsModel);//for menu 0
                }

                var Areas = _areaRepository.GetAll().ToList();
                foreach (var item in Areas)
                {

                    var xx = ditlist.Where(x => x.SpecificationChoicesId == spsID && x.MenuType == item.Id).FirstOrDefault();
                    if (xx == null)
                    {
                        itemAdditionDetailsModel.MenuType = int.Parse(item.Id.ToString());


                        CreateItemSpecificationsDetail(itemAdditionDetailsModel);
                    }


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected virtual async Task<long> Update(CreateOrEditItemAdditionDto input)
        {
            try
            {
                var item = await _itemAdditionsRepository.FirstOrDefaultAsync((long)input.Id);

                item.ItemId = input.ItemId;

                item.Name = input.Name;
                item.NameEnglish = input.NameEnglish;

                item.Price = input.Price;
                item.TenantId = (int?)AbpSession.TenantId;
                item.MenuType = input.MenuType;
                item.ImageUri = input.ImageUri;



                item.LoyaltyPoints = input.LoyaltyPoints;
                item.OriginalLoyaltyPoints = input.OriginalLoyaltyPoints;
                item.IsOverrideLoyaltyPoints = input.IsOverrideLoyaltyPoints;

                await _itemAdditionsRepository.UpdateAsync(item);

                return item.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        //public async Task Delete(EntityDto<long> input)
        //{
        //    await _itemAdditionsRepository.DeleteAsync(input.Id);
        //}

        public async Task<GetItemAdditionForEditOutput> GetItemCategoryForEdit(EntityDto<long> input)
        {
            try
            {
                var item = await _itemAdditionsRepository.FirstOrDefaultAsync(input.Id);

                var output = new GetItemAdditionForEditOutput { createOrEditItemAdditionDto = ObjectMapper.Map<CreateOrEditItemAdditionDto>(item) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<GetItemAdditionForViewDto> GetItemCategoryForView(long id)
        {
            try
            {
                var item = await _itemAdditionsRepository.GetAsync(id);

                var output = new GetItemAdditionForViewDto { itemAdditionDto = ObjectMapper.Map<ItemAdditionDto>(item) };

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        } 
        
        public async Task <List<ItemAdditionDto>> GetCondiments(int tenantID, int menuType)
        {
         
            return await getCondiments(tenantID,  menuType);
        }
         public async Task <List<ItemAdditionDto>> GetDeserts(int tenantID, int menuType)
        {
         
            return await getDeserts(tenantID,  menuType);
        }
         public async Task <List<ItemAdditionDto>> GetCrispy(int tenantID, int menuType)
        {
         
            return await getCrispy(tenantID,  menuType);
        }

        #region Private Methods
        private long deleteItemAdditionCategory(long input)
        {
            try
            {
                var SP_Name = Constants.ItemAdditions.SP_ItemAdditionsCategoryDelete;
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@AdditionCategoryId",input)
                };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (long)OutputParameter.Value;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool deleteItemAddition(long id)
        {
            try
            {
                var SP_Name = Constants.ItemAdditions.SP_ItemAdditionDelete;
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@ItemAdditionsId",id)
                };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ItemAdditionDetailsModel> GetItemAdditionDetailsList(int TenantID)
        {
            try
            {
                //TenantID = "31";
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemAdditionDetails] where TenantID=" + TenantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<ItemAdditionDetailsModel> itemAdditionDetails = new List<ItemAdditionDetailsModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    itemAdditionDetails.Add(new ItemAdditionDetailsModel
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                        ItemAdditionId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemAdditionId"]),
                        //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                    });



                }

                conn.Close();
                da.Dispose();

                return itemAdditionDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private int CreateItemAdditionDetails(ItemAdditionDetailsModel itemAdditionDetails)
        {

            try
            {
                int modified = 0;
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "INSERT INTO ItemAdditionDetails (ItemAdditionId , TenantId, MenuType  ,IsInService ) VALUES (@ItemAdditionId , @TenantId, @MenuType  ,@IsInService );SELECT SCOPE_IDENTITY();  ";

                    command.Parameters.AddWithValue("@ItemAdditionId", itemAdditionDetails.ItemAdditionId);
                    //   command.Parameters.AddWithValue("@ItemId", itemAdditionDetails.ItemId);
                    command.Parameters.AddWithValue("@MenuType", itemAdditionDetails.MenuType);
                    command.Parameters.AddWithValue("@TenantId", itemAdditionDetails.TenantId);
                    command.Parameters.AddWithValue("@IsInService", itemAdditionDetails.IsInService);
                    //    command.Parameters.AddWithValue("@CopiedFromId", itemAdditionDetails.CopiedFromId);

                    connection.Open();


                    modified = Convert.ToInt32(command.ExecuteScalar());
                    if (connection.State == System.Data.ConnectionState.Open) connection.Close();


                    return modified;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private List<ItemSpecificationsDetail> GetItemSpecificationsDetailList(int TenantID)
        {
            try
            {
                //TenantID = "31";
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemSpecificationsDetail] where TenantID=" + TenantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<ItemSpecificationsDetail> itemAdditionDetails = new List<ItemSpecificationsDetail>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    itemAdditionDetails.Add(new ItemSpecificationsDetail
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                        SpecificationChoicesId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SpecificationChoicesId"]),
                        //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                    });



                }

                conn.Close();
                da.Dispose();

                return itemAdditionDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private int CreateItemSpecificationsDetail(ItemSpecificationsDetail itemAdditionDetails)
        {
            try
            {
                int modified = 0;
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))

                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "INSERT INTO ItemSpecificationsDetail (SpecificationChoicesId , TenantId, MenuType  ,IsInService,CopiedFromId, ItemId ) VALUES (@SpecificationChoicesId , @TenantId, @MenuType  ,@IsInService, @CopiedFromId, @ItemId );SELECT SCOPE_IDENTITY();  ";

                    command.Parameters.AddWithValue("@SpecificationChoicesId", itemAdditionDetails.SpecificationChoicesId);
                    command.Parameters.AddWithValue("@CopiedFromId", 0);
                    command.Parameters.AddWithValue("@ItemId", 0);
                    command.Parameters.AddWithValue("@MenuType", itemAdditionDetails.MenuType);
                    command.Parameters.AddWithValue("@TenantId", itemAdditionDetails.TenantId);
                    command.Parameters.AddWithValue("@IsInService", itemAdditionDetails.IsInService);
                    //    command.Parameters.AddWithValue("@CopiedFromId", itemAdditionDetails.CopiedFromId);

                    connection.Open();


                    modified = Convert.ToInt32(command.ExecuteScalar());
                    if (connection.State == System.Data.ConnectionState.Open) connection.Close();


                    return modified;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task<List<ItemAdditionDto>> getCondiments(int tenantID, int menuType)
        {

            try
            {
                List<ItemAdditionDto> lstItemAdditionDto = new List<ItemAdditionDto>();
                var SP_Name = "[dbo].[CondimentGet]";
                var sqlParameters = new List<SqlParameter> {
             new SqlParameter("@TenantId",tenantID)
             ,new SqlParameter("@MenuType",menuType)


            };




                lstItemAdditionDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapItemAdditionDto, AppSettingsModel.ConnectionStrings).ToList();

                return lstItemAdditionDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<List<ItemAdditionDto>> getCrispy(int tenantID, int menuType)
        {

            try
            {
                List<ItemAdditionDto> lstItemAdditionDto = new List<ItemAdditionDto>();
                var SP_Name = "[dbo].[CrispyGet]";
                var sqlParameters = new List<SqlParameter> {
             new SqlParameter("@TenantId",tenantID)
             ,new SqlParameter("@MenuType",menuType)


            };




                lstItemAdditionDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapItemAdditionDto, AppSettingsModel.ConnectionStrings).ToList();

                return lstItemAdditionDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<List<ItemAdditionDto>> getDeserts(int tenantID, int menuType)
        {

            try
            {
                List<ItemAdditionDto> lstItemAdditionDto = new List<ItemAdditionDto>();
                var SP_Name = "[dbo].[DesertsGet]";
                var sqlParameters = new List<SqlParameter> {
             new SqlParameter("@TenantId",tenantID)
             ,new SqlParameter("@MenuType",menuType)


            };




                lstItemAdditionDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                MapItemAdditionDto, AppSettingsModel.ConnectionStrings).ToList();

                return lstItemAdditionDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ItemAdditionDto MapItemAdditionDto(IDataReader dataReader)
        {
            try
            {
                ItemAdditionDto entity = new ItemAdditionDto();
                entity.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                entity.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                entity.price = SqlDataHelper.GetValue<decimal>(dataReader, "Price");
                entity.itemId = SqlDataHelper.GetValue<long>(dataReader, "ItemId");
                entity.SKU = SqlDataHelper.GetValue<string>(dataReader, "SKU");
                entity.MenuType = SqlDataHelper.GetValue<int>(dataReader, "MenuType");
                entity.LanguageBotId = SqlDataHelper.GetValue<int>(dataReader, "LanguageBotId");
                entity.NameEnglish = SqlDataHelper.GetValue<string>(dataReader, "NameEnglish");
                entity.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
                entity.ItemAdditionsCategoryId = SqlDataHelper.GetValue<long>(dataReader, "ItemAdditionsCategoryId");
                entity.IsInService = SqlDataHelper.GetValue<bool>(dataReader, "IsInService");
                entity.ImageUri = SqlDataHelper.GetValue<bool>(dataReader, "ImageUri");

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion


    }
}