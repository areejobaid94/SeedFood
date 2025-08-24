using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.DeliveryOrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.DeliveryOrderDetails
{
    public class DeliveryOrderDetailsAppService : MessagingPortalAppServiceBase, IDeliveryOrderDetailsAppService
    {


        private readonly IRepository<Order, long> _orderRepository;
       // private  IRepository<DeliveryOrderDetails> _deliveryOrderDetailsDtoRepository;
        public DeliveryOrderDetailsAppService(IRepository<Order, long> orderRepository)
        {
            _orderRepository = orderRepository;
           // _deliveryOrderDetailsDtoRepository = deliveryOrderDetailsDtoRepository;

        }
  
        public DeliveryOrderDetailsDto GetDeliveryOrderDetails(int OrderId)
        {
            var deliveryOrderDetailsDto = GetAllDeliveryOrderDetails(OrderId);

            var fromurl = deliveryOrderDetailsDto.FromGoogleURL.Substring(deliveryOrderDetailsDto.FromGoogleURL.IndexOf("https"));
            var Fromquery = fromurl.Substring(fromurl.IndexOf("=") + 1);
            deliveryOrderDetailsDto.FromLongitudeLatitude = Fromquery;

            var tourl = deliveryOrderDetailsDto.ToGoogleURL.Substring(deliveryOrderDetailsDto.ToGoogleURL.IndexOf("https"));
            var Toquery = tourl.Substring(tourl.IndexOf("=") + 1);
            deliveryOrderDetailsDto.ToLongitudeLatitude = Toquery;

            deliveryOrderDetailsDto.FromGoogleURL=deliveryOrderDetailsDto.FromGoogleURL.Replace(" ", "");
            deliveryOrderDetailsDto.ToGoogleURL=deliveryOrderDetailsDto.ToGoogleURL.Replace(" ", "");


           
            return deliveryOrderDetailsDto;
        }



        
        public void AddDeliveryOrderDetails(DeliveryOrderDetailsDto deliveryLocationCost)
        {
            if (deliveryLocationCost.Id == 0)
            {

                Add(deliveryLocationCost);
            }
            else
            {
                Update(deliveryLocationCost);

            }

        }
       

        public void Delete(DeliveryOrderDetailsDto deliveryLocationCost)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "DELETE FROM  DeliveryOrderDetails Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);

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

        private void Add(DeliveryOrderDetailsDto deliveryLocationCost)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO DeliveryOrderDetails (FromLocationId , ToLocationId, TenantId, DeliveryCost, FromAddress, FromGoogleURL, ToAddress, ToGoogleURL, DeliveryCostString, OrderId) VALUES (@FromLocationId ,@ToLocationId, @TenantId, @DeliveryCost, @FromAddress, @FromGoogleURL, @ToAddress, @ToGoogleURL, @DeliveryCostString, @OrderId) ";

                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
                        command.Parameters.AddWithValue("@FromLocationId", deliveryLocationCost.FromLocationId);
                        command.Parameters.AddWithValue("@ToLocationId", deliveryLocationCost.ToLocationId);
                        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
                        command.Parameters.AddWithValue("@DeliveryCost", deliveryLocationCost.DeliveryCost);

                        command.Parameters.AddWithValue("@FromAddress", deliveryLocationCost.FromAddress);
                        command.Parameters.AddWithValue("@FromGoogleURL", deliveryLocationCost.FromGoogleURL);
                        command.Parameters.AddWithValue("@ToAddress", deliveryLocationCost.ToAddress);
                        command.Parameters.AddWithValue("@ToGoogleURL", deliveryLocationCost.ToGoogleURL);
                        command.Parameters.AddWithValue("@DeliveryCostString", deliveryLocationCost.DeliveryCostString);
                        command.Parameters.AddWithValue("@OrderId", deliveryLocationCost.OrderId);


                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }
        private void Update(DeliveryOrderDetailsDto deliveryLocationCost)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "UPDATE DeliveryOrderDetails SET FromLocationId = @FromLocationId, ToLocationId = @ToLocationId , TenantId = @TenantId, DeliveryCost = @DeliveryCost  , FromAddress = @FromAddress , FromGoogleURL = @FromGoogleURL , ToAddress = @ToAddress , ToGoogleURL = @ToGoogleURL , DeliveryCostString = @DeliveryCostString , OrderId = @OrderId   Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
                        command.Parameters.AddWithValue("@FromLocationId", deliveryLocationCost.FromLocationId);
                        command.Parameters.AddWithValue("@ToLocationId", deliveryLocationCost.ToLocationId);
                        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
                        command.Parameters.AddWithValue("@DeliveryCost", deliveryLocationCost.DeliveryCost);

                        command.Parameters.AddWithValue("@FromAddress", deliveryLocationCost.FromAddress);
                        command.Parameters.AddWithValue("@FromGoogleURL", deliveryLocationCost.FromGoogleURL);
                        command.Parameters.AddWithValue("@ToAddress", deliveryLocationCost.ToAddress);
                        command.Parameters.AddWithValue("@ToGoogleURL", deliveryLocationCost.ToGoogleURL);
                        command.Parameters.AddWithValue("@DeliveryCostString", deliveryLocationCost.DeliveryCostString);
                        command.Parameters.AddWithValue("@OrderId", deliveryLocationCost.OrderId);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }

        private DeliveryOrderDetailsDto GetAllDeliveryOrderDetails(int OrderId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[DeliveryOrderDetails] where  OrderId =" + OrderId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            DeliveryOrderDetailsDto  deliveryOrderDetailsDto = new DeliveryOrderDetailsDto();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                deliveryOrderDetailsDto=new DeliveryOrderDetailsDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    FromLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["FromLocationId"]),
                    ToLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ToLocationId"]),
                    DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                     DeliveryCostString = dataSet.Tables[0].Rows[i]["DeliveryCostString"].ToString(),
                      FromAddress = dataSet.Tables[0].Rows[i]["FromAddress"].ToString(),
                     FromGoogleURL = dataSet.Tables[0].Rows[i]["FromGoogleURL"].ToString(),
                     ToAddress = dataSet.Tables[0].Rows[i]["ToAddress"].ToString(),
                     ToGoogleURL = dataSet.Tables[0].Rows[i]["ToGoogleURL"].ToString(),
                     OrderId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderId"]),
                      // FromLocationDescribation= dataSet.Tables[0].Rows[i]["FromLocationDescribation"].ToString(),
                       // OrderDescribation = dataSet.Tables[0].Rows[i]["OrderDescribation"].ToString(),
                         //ToLocationDescribation = dataSet.Tables[0].Rows[i]["ToLocationDescribation"].ToString(),




                };



            }

            conn.Close();
            da.Dispose();

            return deliveryOrderDetailsDto;

        }

        public Task<PagedResultDto<GetDeliveryOrderDetailsForViewDto>> GetAll(GetAllDeliveryOrderDetailsInput input)
        {
            throw new NotImplementedException();
        }
    }
}
