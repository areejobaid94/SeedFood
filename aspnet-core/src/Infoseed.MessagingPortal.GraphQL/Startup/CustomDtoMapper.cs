using AutoMapper;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Startup
{
    public static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<User, UserDto>()
                .ForMember(dto => dto.Roles, options => options.Ignore())
                .ForMember(dto => dto.OrganizationUnits, options => options.Ignore());
        }
    }
}