using AutoMapper;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;

namespace Infoseed.MessagingPortal.BotAPI
{
    public class InfoSeedBotAPIAutoMapping
    {


        public class AutoMapping : Profile
        {
            public AutoMapping()
            {
             
                CreateMap<Order, OrderDto>(); // means you want to map from User to UserDTO
                CreateMap<OrderDto, Order>(); // means you want to map from User to UserDTO
            }
        }
    }
}
