using Abp.Application.Services;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.FacebookDTO.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.FacebookDTO
{
    public interface IFacebookAppService : IApplicationService
    {
        Task<string> CheckInstagram();
        Task<string> GetPageAccessToken(string code);
        Task<FacebookPagesModel> GetFacebookPages(string userToken);
        Task<bool> SubscribePage(string PageId ,string PageAccessToken,bool isSubscribe);


        Task<string> GetInstagramToken(string code, string f3_request_id, string ig_app_id);
        Task DeleteInstagramAsync();
    }
}
