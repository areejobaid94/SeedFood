using Abp.Application.Services;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.General.Dto;
using Infoseed.MessagingPortal.InfoSeedParser;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppMediaResult;

namespace Infoseed.MessagingPortal.General
{
    public interface IGeneralAppService : IApplicationService
    {
        LocationsPinned GetNearbyLocationsPinned(int tenantId, float latitude, float longitude);
        GetLocationInfoTowModel GetNearbyLocations(int tenantId, float latitude, float longitude,int ParentId);
        
        Task<string> FindFileByName(string fileName, string type);
        ContactDto GetContactbyId(int id);
        void UpdateContactInfo(ContactDto contactDto);
        ContactDto CreateContact(ContactDto contact);
        CountryInfoModel GetCountryInfo(string countryISO);
        List<CountryInfoModel> GetAllCountryInfo();
        TenantsModel GetTenantById(int tenantId);
        List<BotTemplatesModel> GetBotTemplates();
        Task<WhatsAppHeaderUrl> GetInfoSeedUrlFile([FromForm] UploadFileModel file);
    }
}
