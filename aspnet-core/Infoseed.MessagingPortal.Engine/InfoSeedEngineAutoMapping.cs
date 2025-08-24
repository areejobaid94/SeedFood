using AutoMapper;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;

namespace Infoseed.MessagingPortal.Engine
{
    public class InfoSeedEngineAutoMapping
    {


        public class AutoMapping : Profile
        {
            public AutoMapping()
            {
             
             //   CreateMap<Order, OrderDto>(); // means you want to map from User to UserDTO
              //  CreateMap<OrderDto, Order>(); // means you want to map from User to UserDTO
            }
        }
    }
}
