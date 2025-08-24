using Abp.Application.Services;
using Infoseed.MessagingPortal.Facebook_Template.Dtos;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Facebook_Template
{
    public interface IFacebookTemplateAppService : IApplicationService
    {

        FacebookTemplateDto GetAll(long id);
        Task<MessageTemplateModel> GetTemplateFacebookById(string templateId);
        Task<FacebookTemplateDto> AddFacebookMessageTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null);
        Task<FacebookTemplateDto> UpdateTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null);
        Task<FacebookTemplateDto> DeleteFacebookMessageTemplateAsync(string templateName);

    }
}
