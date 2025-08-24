//using Infoseed.InfoSeedAzureFunction;
//using InfoSeedAzureFunction.AppFunEntities;
//using InfoSeedAzureFunction.Model;
//using InfoSeedAzureFunction.WhatsAppApi;
//using InfoSeedAzureFunction.WhatsAppApi.Dto;
//using Microsoft.Azure.WebJobs;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace InfoSeedAzureFunction
//{  
//    public static class SendScheduledCampaign
//    {
//        [FunctionName("SendScheduledCampaign")]
//        public static void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = false)] TimerInfo myTimer)
//        {
//            SocketIOManager.Init();
//            Sync().Wait();
//        }

//        public static async Task Sync()
//        {
//            List<WhatsAppScheduledCampaignModel> campaigns = GetScheduledCampaign();
//            if (campaigns.Any())
//            {
//                foreach (var campaign in campaigns)
//                {
//                    if (campaign.SendDateTime >= DateTime.UtcNow)
//                    {
//                        continue;
//                    }

//                    var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
//                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.Tenant && a.TenantId == campaign.TenantId);

//                    MessageTemplateModel objWhatsAppTemplateModel = GetTemplateById(campaign.TemplateId);
//                    MessageTemplateModel templateWA = GetTemplateByWhatsAppId(tenant, objWhatsAppTemplateModel.id).Result;

//                    if (templateWA != null && templateWA.status == "APPROVED")
//                    {
//                        objWhatsAppTemplateModel.components = templateWA.components;
//                        WhatsAppFunModel model = new WhatsAppFunModel
//                        {
//                            IsContact = !campaign.IsExternalContact,
//                            msg = PrepareMessageTemplateText(objWhatsAppTemplateModel, out string type),
//                            type = type,
//                            GuidId = Guid.NewGuid(),
//                            DailyLimit = getDailyLimitByTenantId(campaign.TenantId),
//                            templateId = campaign.TemplateId,
//                            campaignId = campaign.CampaignId,
//                            TenantId = campaign.TenantId,
//                        };

//                        if (model.IsContact)
//                        {
//                            var options = new JsonSerializerOptions { WriteIndented = true };
//                            model.whatsAppContactsDto = System.Text.Json.JsonSerializer.Deserialize<WhatsAppContactsDto>(campaign.ContactsJson, options);
//                            model.whatsAppContactsDto.CampaignId = campaign.CampaignId;
//                            model.whatsAppContactsDto.TemplateId = campaign.TemplateId;
//                        }

//                        WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
//                        {
//                            id = campaign.CampaignId,
//                            sentTime = DateTime.UtcNow,
//                            status = (int)WhatsAppCampaignStatusEnum.Active, // start 
//                            SentCampaignId = model.GuidId
//                        };
//                        UpdateWhatsAppCampaign(whatsAppCampaignModel);
//                        var statistics = GetStatistics(model.TenantId);
//                        await Proceed(model, statistics,tenant, objWhatsAppTemplateModel , campaign.CreatedByUserId);
//                    }
//                }
//            }
//        }
//        private static async Task Proceed(WhatsAppFunModel model, GetAllDashboard Statistics, TenantModel tenant, MessageTemplateModel objWhatsAppTemplateModel, long CreatedByUserId ,int pageNumber =0 )
//        {
//            try
//            {
//                try
//                {
//                    await Task.Delay(1000);

//                }
//                catch
//                {

//                }
//                ContactsEntity contactsEntity = new ContactsEntity();
//                int pageSize = 10000;
//                int contactCount = 0;

//                UsageDetailsModel usageDetailsModel = new UsageDetailsModel();
//                UsersDashModel usersDashModel = new UsersDashModel();
//                List<CountryCountModel> countryCountModel = new List<CountryCountModel>();
//                Dictionary<string, int> countryCounts = new Dictionary<string, int>();
//                WalletDashModel walletDashModel = new WalletDashModel();

//                string templName = "";
//                long CampaignId = 0;
//                int quantity = 0;
//                double totalCost = 0;
//                walletDashModel = walletGetByTenantId(tenant.TenantId.Value);
//                bool IsSent = false;
//                bool don = false;
//                long num = 0;
//                decimal totalwallet = walletDashModel.TotalAmount;
//                string category = "";
//                string CountryCountry = "";

//                if (model.IsContact)
//                {
//                    contactsEntity = GetFilterContacts(model.whatsAppContactsDto, model.TenantId);
//                    contactsEntity.contacts = contactsEntity.contacts.Where(x => x.CustomerOPT != 1).ToList();
//                    contactCount = contactsEntity.contacts.Count;
//                }
//                else
//                {
//                    contactsEntity = GetExternalContacts(model.campaignId, model.templateId, pageNumber, pageSize, model.TenantId);
//                    contactCount = contactsEntity.contacts.Count;
//                }

//                if (contactsEntity.contacts.Any() && model.DailyLimit >= contactCount && contactCount > 0)
//                {
//                    List<ContactCampaignModel> lstcontactCampaignModel = new List<ContactCampaignModel>();
//                    List<SendCampaignFailedModel> LstSendCampaignFailedModel = new List<SendCampaignFailedModel>();

//                    foreach (var contact in contactsEntity.contacts)
//                    {
//                        PostMessageTemplateModel _postMessageTemplateModel = PrepareMessageTemplate(objWhatsAppTemplateModel, contact);
//                        var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

//                        var ISO = getCoutryBIRate(contact.PhoneNumber);
//                        var Country = CountryISOCodeGet(ISO.Iso);

//                        if (objWhatsAppTemplateModel.category == "MARKETING")
//                        {
//                            if (Country != null && walletDashModel.TotalAmount >= Math.Round((decimal)Country.MarketingPrice, 3))
//                            {
//                                totalCost += Country.MarketingPrice;
//                                walletDashModel.TotalAmount = walletDashModel.TotalAmount - Math.Round((decimal)Country.MarketingPrice, 3);
//                                category = "Marketing Conversations";
//                            }
//                            else if (walletDashModel.TotalAmount >= (decimal)0.027)
//                            {
//                                totalCost += 0.027;
//                                walletDashModel.TotalAmount = walletDashModel.TotalAmount - (decimal)0.027;
//                                category = "Marketing Conversations";
//                            }
//                            else { break; }
//                        }
//                        else if (objWhatsAppTemplateModel.category == "UTILITY")
//                        {
//                            if (Country != null && walletDashModel.TotalAmount >= Math.Round((decimal)Country.UtilityPrice, 3))
//                            {
//                                totalCost += Country.UtilityPrice;
//                                walletDashModel.TotalAmount = walletDashModel.TotalAmount - Math.Round((decimal)Country.UtilityPrice, 3);
//                                category = "Utility Conversations";
//                            }
//                            else if (walletDashModel.TotalAmount >= (decimal)0.0192)
//                            {
//                                totalCost += 0.0192;
//                                walletDashModel.TotalAmount = walletDashModel.TotalAmount - (decimal)0.0192;
//                                category = "Utility Conversations";
//                            }
//                            else { break; }
//                        }
//                        if (totalwallet >= Math.Round((decimal)totalCost, 3))
//                        {
//                            var contactCampaignModel = await SendTemplateToWhatsApp(tenant, postBody, model.campaignId, _postMessageTemplateModel.to, contact.Id, model.IsContact, model.msg, model.type, objWhatsAppTemplateModel.mediaLink, model.TenantId, model.UserId, model.templateId, LstSendCampaignFailedModel, model.GuidId, lstcontactCampaignModel, objWhatsAppTemplateModel.id,  IsSent);
//                            if (contactCampaignModel != null)
//                            {
//                                if (contactCampaignModel.IsSent == true)
//                                {
//                                    templName = _postMessageTemplateModel.template.name;
//                                    CampaignId = model.campaignId;
//                                    quantity += 1;
//                                    CountryCountry += Country.Country + ",";
//                                    //// Check if the country exists in the dictionary
//                                    //if (countryCounts.ContainsKey(Country.Country))
//                                    //{
//                                    //    // If it exists, increase its count by 1
//                                    //    countryCounts[Country.Country]++;
//                                    //}
//                                    //else
//                                    //{
//                                    //    // If it doesn't exist, you can add it with a count of 1
//                                    //    countryCounts[Country.Country] = 1;
//                                    //}
//                                    //don = true;
//                                }
//                                else { totalCost = 0; }
//                            }
//                            else { totalCost = 0; }
//                        }
//                        //else { return; }
//                    }
//                    if (totalCost != 0)
//                    {
//                        usageDetailsModel.TenantId = model.TenantId;
//                        usageDetailsModel.categoryType = category;
//                        usageDetailsModel.dateTime = DateTime.UtcNow;
//                        usersDashModel = GetUserInfo(CreatedByUserId);
//                        usageDetailsModel.sentBy = usersDashModel.UserName;
//                        usageDetailsModel.templateName = templName;
//                        usageDetailsModel.campaignName = GetCampaignName(CampaignId);
//                        usageDetailsModel.quantity = quantity;
//                        usageDetailsModel.totalCost = Math.Round((decimal)totalCost, 3);
//                        usageDetailsModel.countries = CountryCountry;
//                        usageDetailsModel.campaignId = model.campaignId;
//                        //foreach (var kvp in countryCounts)
//                        //{
//                        //    usageDetailsModel.countries += kvp.Key + " " + kvp.Value + ",";
//                        //}

//                        if (usageDetailsModel != null && CampaignId != 0)
//                        {
//                            num = DepositAndCreateTransaction(usageDetailsModel);
//                        }
//                    }
//                    else 
//                    {
//                        WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
//                        {
//                            id = model.campaignId,
//                            sentTime = DateTime.UtcNow,
//                            status = (int)WhatsAppCampaignStatusEnum.Draft,
//                            SentCampaignId = model.GuidId
//                        };
//                        UpdateWhatsAppCampaign(whatsAppCampaignModel);
//                    }
//                    if (lstcontactCampaignModel.Any())
//                    {
//                        decimal UsageBIRate = 0, UsageFreeRate = 0;
//                        int contactCampaignCount = lstcontactCampaignModel.Count;
//                        UsageBIRate += contactCampaignCount;
//                        Statistics.RemainingBIConversation -= contactCampaignCount;
//                        model.DailyLimit -= contactCampaignCount;
//                        contactCount -= contactCampaignCount;
//                        AddContactCampaign(lstcontactCampaignModel);
//                        UpdateBIConversation(model.TenantId, UsageBIRate, UsageFreeRate);
//                    }

//                    if (LstSendCampaignFailedModel.Any()) ///failed list
//                    {
//                        UpdateFailedContact(LstSendCampaignFailedModel);
//                    }
//                    if (num != 0)
//                    {
//                        pageNumber++;
//                        await Proceed(model, Statistics, tenant, objWhatsAppTemplateModel, CreatedByUserId , pageNumber );
//                    }
//                }
//                else
//                {
//                    WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
//                    {
//                        id = model.campaignId,
//                        sentTime = DateTime.UtcNow,
//                        status = (int)WhatsAppCampaignStatusEnum.Sent,
//                        SentCampaignId = model.GuidId
//                    };
//                    UpdateWhatsAppCampaign(whatsAppCampaignModel);
//                }
//            }
//            catch
//            {
//                WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
//                {
//                    id = model.campaignId,
//                    sentTime = DateTime.UtcNow,
//                    status = (int)WhatsAppCampaignStatusEnum.Sent,
//                    SentCampaignId = model.GuidId
//                };
//                UpdateWhatsAppCampaign(whatsAppCampaignModel);
//            }
//        }

//        private static CoutryTelCodeModel getCoutryBIRate(string phone)
//        {
//            List<CoutryTelCodeModel> TelCodes = new List<CoutryTelCodeModel>
//                   {
//                       new CoutryTelCodeModel("93", "AF",1),
//                       new CoutryTelCodeModel("355", "AL",1),
//                       new CoutryTelCodeModel("213", "DZ",2),
//                       new CoutryTelCodeModel("1-684", "AS",1),
//                       new CoutryTelCodeModel("376", "AD",1),
//                       new CoutryTelCodeModel("244", "AO",1),
//                       new CoutryTelCodeModel("1-264", "AI",1),
//                       new CoutryTelCodeModel("672", "AQ",1),
//                       new CoutryTelCodeModel("1-268", "AG",1),
//                       new CoutryTelCodeModel("54", "AR",1),
//                       new CoutryTelCodeModel("374", "AM",1),
//                       new CoutryTelCodeModel("297", "AW",1),
//                       new CoutryTelCodeModel("61", "AU",1),
//                       new CoutryTelCodeModel("43", "AT",(decimal)1.5),
//                       new CoutryTelCodeModel("994", "AZ",1),
//                       new CoutryTelCodeModel("1-242", "BS",1),
//                       new CoutryTelCodeModel("973", "BH",1),
//                       new CoutryTelCodeModel("880", "BD",1),
//                       new CoutryTelCodeModel("1-246", "BB",1),
//                       new CoutryTelCodeModel("375", "BY",(decimal)1.5),
//                       new CoutryTelCodeModel("32", "BE",2),
//                       new CoutryTelCodeModel("501", "BZ",1),
//                       new CoutryTelCodeModel("229", "BJ",1),
//                       new CoutryTelCodeModel("1-441", "BM",1),
//                       new CoutryTelCodeModel("975", "BT",1),
//                       new CoutryTelCodeModel("591", "BO",1),
//                       new CoutryTelCodeModel("387", "BA",1),
//                       new CoutryTelCodeModel("267", "BW",1),
//                       new CoutryTelCodeModel("55", "BR",1),
//                       new CoutryTelCodeModel("246", "IO",1),
//                       new CoutryTelCodeModel("1-284", "VG",1),
//                       new CoutryTelCodeModel("673", "BN",1),
//                       new CoutryTelCodeModel("359", "BG",(decimal)1.5),
//                       new CoutryTelCodeModel("226", "BF",1),
//                       new CoutryTelCodeModel("257", "BI",1),
//                       new CoutryTelCodeModel("855", "KH",1),
//                       new CoutryTelCodeModel("237", "CM",1),
//                       new CoutryTelCodeModel("1", "CA",(decimal)0.5),
//                       new CoutryTelCodeModel("238", "CV",1),
//                       new CoutryTelCodeModel("1-345", "KY",1),
//                       new CoutryTelCodeModel("236", "CF",1),
//                       new CoutryTelCodeModel("235", "TD",1),
//                       new CoutryTelCodeModel("56", "CL",1),
//                       new CoutryTelCodeModel("86", "CN",1),
//                       new CoutryTelCodeModel("61", "CX",1),
//                       new CoutryTelCodeModel("61", "CC",1),
//                       new CoutryTelCodeModel("57", "CO",(decimal)0.5),
//                       new CoutryTelCodeModel("269", "KM",1),
//                       new CoutryTelCodeModel("682", "CK",1),
//                       new CoutryTelCodeModel("506", "CR",1),
//                       new CoutryTelCodeModel("385", "HR",(decimal)1.5),
//                       new CoutryTelCodeModel("53", "CU",1),
//                       new CoutryTelCodeModel("599", "CW",1),
//                       new CoutryTelCodeModel("357", "CY",1),
//                       new CoutryTelCodeModel("420", "CZ",(decimal)1.5),
//                       new CoutryTelCodeModel("243", "CD",1),
//                       new CoutryTelCodeModel("45", "DK",2),
//                       new CoutryTelCodeModel("253", "DJ",1),
//                       new CoutryTelCodeModel("1-767", "DM",1),
//                       new CoutryTelCodeModel("1-809", "DO",1),
//                       new CoutryTelCodeModel("1-829", "DO",1),
//                       new CoutryTelCodeModel("1-849", "DO",1),
//                       new CoutryTelCodeModel("670", "TL",1),
//                       new CoutryTelCodeModel("593", "EC",1),
//                       new CoutryTelCodeModel("20", "EG",(decimal)1.5),
//                       new CoutryTelCodeModel("503", "SV",1),
//                       new CoutryTelCodeModel("240", "GQ",1),
//                       new CoutryTelCodeModel("291", "ER",1),
//                       new CoutryTelCodeModel("372", "EE",(decimal)1.5),
//                       new CoutryTelCodeModel("251", "ET",1),
//                       new CoutryTelCodeModel("500", "FK",1),
//                       new CoutryTelCodeModel("298", "FO",1),
//                       new CoutryTelCodeModel("679", "FJ",1),
//                       new CoutryTelCodeModel("358", "FI",1),
//                       new CoutryTelCodeModel("33", "FR",2),
//                       new CoutryTelCodeModel("689", "PF",1),
//                       new CoutryTelCodeModel("241", "GA",1),
//                       new CoutryTelCodeModel("220", "GM",1),
//                       new CoutryTelCodeModel("995", "GE",1),
//                       new CoutryTelCodeModel("49", "DE",2),
//                       new CoutryTelCodeModel("233", "GH",1),
//                       new CoutryTelCodeModel("350", "GI",1),
//                       new CoutryTelCodeModel("30", "GR",1),
//                       new CoutryTelCodeModel("299", "GL",(decimal)0.5),
//                       new CoutryTelCodeModel("1-473", "GD",1),
//                       new CoutryTelCodeModel("1-671", "GU",1),
//                       new CoutryTelCodeModel("502", "GT",1),
//                       new CoutryTelCodeModel("44-1481", "GG",1),
//                       new CoutryTelCodeModel("224", "GN",1),
//                       new CoutryTelCodeModel("245", "GW",1),
//                       new CoutryTelCodeModel("592", "GY",1),
//                       new CoutryTelCodeModel("509", "HT",1),
//                       new CoutryTelCodeModel("504", "HN",1),
//                       new CoutryTelCodeModel("852", "HK",1),
//                       new CoutryTelCodeModel("36", "HU",(decimal)1.5),
//                       new CoutryTelCodeModel("354", "IS",1),
//                       new CoutryTelCodeModel("91", "IN",(decimal)0.5),
//                       new CoutryTelCodeModel("62", "ID",(decimal)0.5),
//                       new CoutryTelCodeModel("98", "IR",1),
//                       new CoutryTelCodeModel("964", "IQ",1),
//                       new CoutryTelCodeModel("353", "IE",1),
//                       new CoutryTelCodeModel("44-1624", "IM",1),
//                       new CoutryTelCodeModel("972", "IL",(decimal)0.5),
//                       new CoutryTelCodeModel("39", "IT",1),
//                       new CoutryTelCodeModel("225", "CI",1),
//                       new CoutryTelCodeModel("1-876", "JM",1),
//                       new CoutryTelCodeModel("81", "JP",1),
//                       new CoutryTelCodeModel("44-1534", "JE",1),
//                       new CoutryTelCodeModel("962", "JO",1),
//                       new CoutryTelCodeModel("7", "KZ",1),
//                       new CoutryTelCodeModel("254", "KE",1),
//                       new CoutryTelCodeModel("686", "KI",1),
//                       new CoutryTelCodeModel("383", "XK",1),
//                       new CoutryTelCodeModel("965", "KW",1),
//                       new CoutryTelCodeModel("996", "KG",1),
//                       new CoutryTelCodeModel("856", "LA",1),
//                       new CoutryTelCodeModel("371", "LV",1),
//                       new CoutryTelCodeModel("961", "LB",1),
//                       new CoutryTelCodeModel("266", "LS",1),
//                       new CoutryTelCodeModel("231", "LR",1),
//                       new CoutryTelCodeModel("218", "LY",2),
//                       new CoutryTelCodeModel("423", "LI",1),
//                       new CoutryTelCodeModel("370", "LT",1),
//                       new CoutryTelCodeModel("352", "LU",1),
//                       new CoutryTelCodeModel("853", "MO",1),
//                       new CoutryTelCodeModel("389", "MK",1),
//                       new CoutryTelCodeModel("261", "MG",1),
//                       new CoutryTelCodeModel("265", "MW",1),
//                       new CoutryTelCodeModel("60", "MY",1),
//                       new CoutryTelCodeModel("960", "MV",1),
//                       new CoutryTelCodeModel("223", "ML",1),
//                       new CoutryTelCodeModel("356", "MT",1),
//                       new CoutryTelCodeModel("692", "MH",1),
//                       new CoutryTelCodeModel("222", "MR",1),
//                       new CoutryTelCodeModel("230", "MU",1),
//                       new CoutryTelCodeModel("262", "YT",1),
//                       new CoutryTelCodeModel("52", "MX",(decimal)0.5),
//                       new CoutryTelCodeModel("691", "FM",1),
//                       new CoutryTelCodeModel("373", "MD",(decimal)1.5),
//                       new CoutryTelCodeModel("377", "MC",1),
//                       new CoutryTelCodeModel("976", "MN",1),
//                       new CoutryTelCodeModel("382", "ME",1),
//                       new CoutryTelCodeModel("1-664", "MS",1),
//                       new CoutryTelCodeModel("212", "MA",1),
//                       new CoutryTelCodeModel("258", "MZ",1),
//                       new CoutryTelCodeModel("95", "MM",1),
//                       new CoutryTelCodeModel("264", "NA",1),
//                       new CoutryTelCodeModel("674", "NR",1),
//                       new CoutryTelCodeModel("977", "NP",1),
//                       new CoutryTelCodeModel("31", "NL",1),
//                       new CoutryTelCodeModel("599", "AN",1),
//                       new CoutryTelCodeModel("687", "NC",1),
//                       new CoutryTelCodeModel("64", "NZ",1),
//                       new CoutryTelCodeModel("505", "NI",1),
//                       new CoutryTelCodeModel("227", "NE",1),
//                       new CoutryTelCodeModel("234", "NG",1),
//                       new CoutryTelCodeModel("683", "NU",1),
//                       new CoutryTelCodeModel("850", "KP",1),
//                       new CoutryTelCodeModel("1-670", "MP",1),
//                       new CoutryTelCodeModel("47", "NO",1),
//                       new CoutryTelCodeModel("968", "OM",1),
//                       new CoutryTelCodeModel("92", "PK",1),
//                       new CoutryTelCodeModel("680", "PW",1),
//                       new CoutryTelCodeModel("970", "PS",1),
//                       new CoutryTelCodeModel("507", "PA",1),
//                       new CoutryTelCodeModel("675", "PG",1),
//                       new CoutryTelCodeModel("595", "PY",1),
//                       new CoutryTelCodeModel("51", "PE",1),
//                       new CoutryTelCodeModel("63", "PH",1),
//                       new CoutryTelCodeModel("64", "PN",1),
//                       new CoutryTelCodeModel("48", "PL",(decimal)1.5),
//                       new CoutryTelCodeModel("351", "PT",1),
//                       new CoutryTelCodeModel("1-787", "PR",1),
//                       new CoutryTelCodeModel("1-939", "PR",1),
//                       new CoutryTelCodeModel("974", "QA",1),
//                       new CoutryTelCodeModel("242", "CG",1),
//                       new CoutryTelCodeModel("262", "RE",1),
//                       new CoutryTelCodeModel("40", "RO",(decimal)1.5),
//                       new CoutryTelCodeModel("7", "RU",1),
//                       new CoutryTelCodeModel("250", "RW",1),
//                       new CoutryTelCodeModel("590", "BL",1),
//                       new CoutryTelCodeModel("290", "SH",1),
//                       new CoutryTelCodeModel("1-869", "KN",1),
//                       new CoutryTelCodeModel("1-758", "LC",1),
//                       new CoutryTelCodeModel("590", "MF",1),
//                       new CoutryTelCodeModel("508", "PM",(decimal)0.5),
//                       new CoutryTelCodeModel("1-784", "VC",1),
//                       new CoutryTelCodeModel("685", "WS",1),
//                       new CoutryTelCodeModel("378", "SM",1),
//                       new CoutryTelCodeModel("239", "ST",1),
//                       new CoutryTelCodeModel("966", "SA",(decimal)0.5),
//                       new CoutryTelCodeModel("221", "SN",1),
//                       new CoutryTelCodeModel("381", "RS",1),
//                       new CoutryTelCodeModel("248", "SC",1),
//                       new CoutryTelCodeModel("232", "SL",1),
//                       new CoutryTelCodeModel("65", "SG",1),
//                       new CoutryTelCodeModel("1-721", "SX",1),
//                       new CoutryTelCodeModel("421", "SK",(decimal)1.5),
//                       new CoutryTelCodeModel("386", "SI",1),
//                       new CoutryTelCodeModel("677", "SB",1),
//                       new CoutryTelCodeModel("252", "SO",1),
//                       new CoutryTelCodeModel("27", "ZA",(decimal)0.5),
//                       new CoutryTelCodeModel("82", "KR",1),
//                       new CoutryTelCodeModel("211", "SS",2),
//                       new CoutryTelCodeModel("34", "ES",1),
//                       new CoutryTelCodeModel("94", "LK",1),
//                       new CoutryTelCodeModel("249", "SD",2),
//                       new CoutryTelCodeModel("597", "SR",1),
//                       new CoutryTelCodeModel("47", "SJ",1),
//                       new CoutryTelCodeModel("268", "SZ",1),
//                       new CoutryTelCodeModel("46", "SE",1),
//                       new CoutryTelCodeModel("41", "CH",1),
//                       new CoutryTelCodeModel("963", "SY",1),
//                       new CoutryTelCodeModel("886", "TW",1),
//                       new CoutryTelCodeModel("992", "TJ",1),
//                       new CoutryTelCodeModel("255", "TZ",1),
//                       new CoutryTelCodeModel("66", "TH",1),
//                       new CoutryTelCodeModel("228", "TG",1),
//                       new CoutryTelCodeModel("690", "TK",1),
//                       new CoutryTelCodeModel("676", "TO",1),
//                       new CoutryTelCodeModel("1-868", "TT",1),
//                       new CoutryTelCodeModel("216", "TN",2),
//                       new CoutryTelCodeModel("90", "TR",(decimal)0.5),
//                       new CoutryTelCodeModel("993", "TM",1),
//                       new CoutryTelCodeModel("1-649", "TC",1),
//                       new CoutryTelCodeModel("688", "TV",1),
//                       new CoutryTelCodeModel("1-340", "VI",1),
//                       new CoutryTelCodeModel("256", "UG",1),
//                       new CoutryTelCodeModel("380", "UA",(decimal)1.5),
//                       new CoutryTelCodeModel("971", "AE",(decimal)0.5),
//                       new CoutryTelCodeModel("44", "GB",1),
//                       new CoutryTelCodeModel("1", "US",(decimal)0.5),
//                       new CoutryTelCodeModel("598", "UY",1),
//                       new CoutryTelCodeModel("998", "UZ",1),
//                       new CoutryTelCodeModel("678", "VU",1),
//                       new CoutryTelCodeModel("379", "VA",1),
//                       new CoutryTelCodeModel("58", "VE",1),
//                       new CoutryTelCodeModel("84", "VN",1),
//                       new CoutryTelCodeModel("681", "WF",1),
//                       new CoutryTelCodeModel("212", "EH",1),
//                       new CoutryTelCodeModel("967", "YE",1),
//                       new CoutryTelCodeModel("260", "ZM",1),
//                       new CoutryTelCodeModel("263", "ZW",1),
//            };
//            CoutryTelCodeModel result = TelCodes.Where(x => phone.StartsWith(x.Pfx)).FirstOrDefault();
//            return result;

//        }

//        #region ForWallet
//        private static UsersDashModel GetUserInfo(long UserId)
//        {
//            try
//            {
//                UsersDashModel usersDashModel = new UsersDashModel();

//                var SP_Name = Constants.User.SP_UsersGetInfo;

//                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
//                    new System.Data.SqlClient.SqlParameter("@UserId",UserId)
//                };

//                usersDashModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapUserInfo, Constants.ConnectionString).FirstOrDefault();

//                return usersDashModel;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        private static string GetCampaignName(long CampaignId)
//        {
//            try
//            {
//                var SP_Name = Constants.Campaign.SP_GetCampaignName;
//                string str;

//                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
//                    new System.Data.SqlClient.SqlParameter("@CampaignId",CampaignId)
//                };

//                str = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapCampignInfo, Constants.ConnectionString).FirstOrDefault();

//                return str;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        private static CountryCodeDashModel CountryISOCodeGet(string ISOCodes)
//        {
//            try
//            {
//                CountryCodeDashModel countryCodeDashModel = new CountryCodeDashModel();
//                var SP_Name = Constants.Country.SP_CountryISOCodeGet;

//                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
//                    new System.Data.SqlClient.SqlParameter("@ISOCodes",ISOCodes)
//                };

//                countryCodeDashModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapCountryInfo, Constants.ConnectionString).FirstOrDefault();

//                return countryCodeDashModel;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        private static WalletDashModel walletGetByTenantId(int TenantId)
//        {
//            try
//            {
//                WalletDashModel walletModel = new WalletDashModel();

//                var SP_Name = Constants.Wallet.SP_WalletGet;

//                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
//                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
//                };

//                walletModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapWallet, Constants.ConnectionString).FirstOrDefault();

//                return walletModel;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        private static long DepositAndCreateTransaction(UsageDetailsModel usageDetailsModel)
//        {
//            try
//            {
//                var SP_Name = Constants.Wallet.SP_ConversationMinus;

//                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
//                     new System.Data.SqlClient.SqlParameter("@TenantId",usageDetailsModel.TenantId)
//                    ,new System.Data.SqlClient.SqlParameter("@categoryType",usageDetailsModel.categoryType)
//                    ,new System.Data.SqlClient.SqlParameter("@dateTime",usageDetailsModel.dateTime)
//                    ,new System.Data.SqlClient.SqlParameter("@SentBy",usageDetailsModel.sentBy)
//                    ,new System.Data.SqlClient.SqlParameter("@templateName",usageDetailsModel.templateName)
//                    ,new System.Data.SqlClient.SqlParameter("@campaignName",usageDetailsModel.campaignName)
//                    ,new System.Data.SqlClient.SqlParameter("@quantity",usageDetailsModel.quantity)
//                    ,new System.Data.SqlClient.SqlParameter("@totalCost",usageDetailsModel.totalCost)
//                    ,new System.Data.SqlClient.SqlParameter("@countries",usageDetailsModel.countries)
//                    ,new System.Data.SqlClient.SqlParameter("@campaignId",usageDetailsModel.campaignId)

//                };

//                var OutputParameter = new System.Data.SqlClient.SqlParameter
//                {
//                    SqlDbType = SqlDbType.BigInt,
//                    ParameterName = "@Output",
//                    Direction = ParameterDirection.Output
//                };
//                sqlParameters.Add(OutputParameter);
//                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

//                if (OutputParameter.Value.ToString() != "" || OutputParameter.Value.ToString() != null)
//                {
//                    return (long)OutputParameter.Value;
//                }
//                else
//                {
//                    return 0;
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion 


//        #region Template & Campaign
//        private static List<WhatsAppScheduledCampaignModel> GetScheduledCampaign()
//        {
//            try
//            {
//                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppScheduledDashCampaignGet;
//                var sqlParameters = new List<SqlParameter>();
//                List<WhatsAppScheduledCampaignModel> message = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, Constants.ConnectionString).ToList();
//                return message;
//            }
//            catch
//            {
//                return new List<WhatsAppScheduledCampaignModel>();
//            }

//        }
//        private static MessageTemplateModel GetTemplateById(long templateId)
//        {
//            try
//            {
//                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesGetById;
//                MessageTemplateModel objWhatsAppTemplateModel = new MessageTemplateModel();
//                var sqlParameters = new List<SqlParameter> { new SqlParameter("@TemplateId", templateId) };
//                objWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapTemplate, Constants.ConnectionString).FirstOrDefault();
//                return objWhatsAppTemplateModel;
//            }
//            catch
//            {
//                return new MessageTemplateModel();
//            }
//        }
//        private static async Task<MessageTemplateModel> GetTemplateByWhatsAppId(TenantModel tenant, string templateId)
//        {
//            var httpClient = new HttpClient();
//            var postUrl = Constant.WhatsAppApiUrl + templateId;
//            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

//            var response = await httpClient.GetAsync(postUrl);
//            var content = response.Content;
//            if (response.StatusCode == System.Net.HttpStatusCode.OK)
//            {
//                var WhatsAppTemplate = await content.ReadAsStringAsync();
//                return JsonConvert.DeserializeObject<MessageTemplateModel>(WhatsAppTemplate);
//            }
//            else
//            {
//                return null;
//            }
//        }
//        private static void UpdateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel)
//        {
//            var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignUpdate;
//            var sqlParameters = new List<SqlParameter> {
//                new SqlParameter("@CampaignId",whatsAppCampaignModel.id)
//                ,new SqlParameter("@SentTime",DateTime.UtcNow)
//                ,new SqlParameter("@Status",whatsAppCampaignModel.status)
//                ,new SqlParameter("@SentCampaignId",whatsAppCampaignModel.SentCampaignId)
//            };
//            SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
//        }
//        private static PostMessageTemplateModel PrepareMessageTemplate(MessageTemplateModel objWhatsAppTemplateModel, WhatsAppContactsDto contact)
//        {
//            PostMessageTemplateModel postMessageTemplateModel = new PostMessageTemplateModel();
//            WhatsAppTemplateModel whatsAppTemplateModel = new WhatsAppTemplateModel();
//            WhatsAppLanguageModel whatsAppLanguageModel = new WhatsAppLanguageModel();

//            List<Component> components = new List<Component>();
//            Component componentHeader = new Component();
//            Parameter parameterHeader = new Parameter();

//            Component componentBody = new Component();
//            Parameter parameterBody1 = new Parameter();
//            Parameter parameterBody2 = new Parameter();
//            Parameter parameterBody3 = new Parameter();
//            Parameter parameterBody4 = new Parameter();
//            Parameter parameterBody5 = new Parameter();

//            ImageTemplate image = new ImageTemplate();
//            VideoTemplate video = new VideoTemplate();
//            DocumentTemplate document = new DocumentTemplate();

//            if (objWhatsAppTemplateModel.components != null)
//            {
//                components = new List<Component>();
//                foreach (var item in objWhatsAppTemplateModel.components)
//                {
//                    if (item.type == "HEADER")
//                    {
//                        componentHeader.type = item.type;
//                        parameterHeader.type = item.format.ToLower();
//                        if (item.format == "IMAGE")
//                        {
//                            image.link = objWhatsAppTemplateModel.mediaLink;
//                            parameterHeader.image = image;
//                            componentHeader.parameters = new List<Parameter>
//                            {
//                                parameterHeader
//                            };
//                        }
//                        if (item.format == "VIDEO")
//                        {
//                            video.link = objWhatsAppTemplateModel.mediaLink;
//                            parameterHeader.video = video;
//                            componentHeader.parameters = new List<Parameter>
//                            {
//                                parameterHeader
//                            };

//                        }
//                        if (item.format == "DOCUMENT")
//                        {
//                            document.link = objWhatsAppTemplateModel.mediaLink;
//                            parameterHeader.document = document;

//                            componentHeader.parameters = new List<Parameter>
//                            {
//                                parameterHeader
//                            };

//                        }
//                        components.Add(componentHeader);

//                    }
//                    if (item.type == "BODY")
//                    {
//                        if (contact.templateVariables != null)
//                        {
//                            if (objWhatsAppTemplateModel.VariableCount >= 1)
//                            {
//                                componentBody.parameters = new List<Parameter>();

//                                parameterBody1.type = "TEXT";
//                                parameterBody1.text = contact.templateVariables.VarOne;
//                                componentBody.parameters.Add(parameterBody1);

//                                if (objWhatsAppTemplateModel.VariableCount >= 2)
//                                {
//                                    parameterBody2.type = "TEXT";
//                                    parameterBody2.text = contact.templateVariables.VarTwo;
//                                    componentBody.parameters.Add(parameterBody2);

//                                    if (objWhatsAppTemplateModel.VariableCount >= 3)
//                                    {
//                                        parameterBody3.type = "TEXT";
//                                        parameterBody3.text = contact.templateVariables.VarThree;
//                                        componentBody.parameters.Add(parameterBody3);

//                                        if (objWhatsAppTemplateModel.VariableCount >= 4)
//                                        {
//                                            parameterBody4.type = "TEXT";
//                                            parameterBody4.text = contact.templateVariables.VarFour;
//                                            componentBody.parameters.Add(parameterBody4);

//                                            if (objWhatsAppTemplateModel.VariableCount >= 5)
//                                            {
//                                                parameterBody5.type = "TEXT";
//                                                parameterBody5.text = contact.templateVariables.VarFive;
//                                                componentBody.parameters.Add(parameterBody5);
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        componentBody.type = item.type;
//                        components.Add(componentBody);
//                    }
//                }
//            }
//            whatsAppLanguageModel.code = objWhatsAppTemplateModel.language;
//            whatsAppTemplateModel.language = whatsAppLanguageModel;
//            whatsAppTemplateModel.name = objWhatsAppTemplateModel.name;
//            whatsAppTemplateModel.components = components;
//            postMessageTemplateModel.template = whatsAppTemplateModel;
//            postMessageTemplateModel.to = contact.PhoneNumber;
//            //postMessageTemplateModel.to = "962786464718";
//            postMessageTemplateModel.messaging_product = "whatsapp";
//            postMessageTemplateModel.type = "template";

//            return postMessageTemplateModel;
//        }
//        private static string PrepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, out string type)
//        {
//            string result = string.Empty;
//            type = "text";
//            if (objWhatsAppTemplateModel.components != null)
//            {
//                foreach (var item in objWhatsAppTemplateModel.components)
//                {
//                    if (item.type.Equals("HEADER"))
//                    {
//                        type = item.format.ToLower();
//                    }
//                    if (item.type.Equals("BUTTONS"))
//                    {
//                        for (int i = 0; i < item.buttons.Count; i++)
//                        {
//                            result = result + "\n\r" + (i + 1) + "-" + item.buttons[i].text;
//                        }
//                    }
//                    result += item.text;
//                }
//            }
//            return result;
//        }
//        private static async Task<ContactCampaignModel> SendTemplateToWhatsApp(TenantModel tenant, string postBody, long campaignId, string phoneNumber, int? ContactId, bool IsContacts, string msg, string type, string mediaUrl, int TenantId, string UserId, long templateId, List<SendCampaignFailedModel> lstSendCampaignFailedModel, Guid guid, List<ContactCampaignModel> lstcontactCampaignModel, string WTID, bool IsSent)
//        {
//            try
//            {
//                ContactCampaignModel contactCampaignModel = new ContactCampaignModel();

//                var httpClient = new HttpClient();
//                var postUrl = Constant.WhatsAppApiUrl + tenant.D360Key + "/messages";
//                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

//                var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json"));
//                var result = await response.Content.ReadAsStringAsync();

//                if (response.StatusCode == System.Net.HttpStatusCode.OK)
//                {
//                    WhatsAppMessageTemplateResult templateResult = new WhatsAppMessageTemplateResult();
//                    templateResult = JsonConvert.DeserializeObject<WhatsAppMessageTemplateResult>(result);

//                    contactCampaignModel.TenantId = tenant.TenantId.Value;
//                    contactCampaignModel.CampaignId = campaignId;
//                    contactCampaignModel.PhoneNumber = phoneNumber;
//                    contactCampaignModel.ResultJson = JsonConvert.SerializeObject(result);
//                    contactCampaignModel.MessageId = templateResult.messages.FirstOrDefault().id;
//                    contactCampaignModel.ContactId = ContactId.Value;
//                    contactCampaignModel.TemplateId = templateId;
//                    contactCampaignModel.SentCampaignId = guid;
//                    contactCampaignModel.MessageRate = 1;
//                    contactCampaignModel.IsSent = true;

//                    //if (IsContacts)
//                    //{
//                    //    await UpdateCustomerChatAsync(phoneNumber, msg, type, mediaUrl, TenantId, UserId, templateResult.messages.FirstOrDefault().id, WTID);
//                    //}
//                    await UpdateCustomerTemp(phoneNumber, msg, type, mediaUrl, TenantId, UserId, templateResult.messages.FirstOrDefault().id, WTID, tenant.D360Key , campaignId.ToString());
//                    lstcontactCampaignModel.Add(contactCampaignModel);
//                }
//                else
//                {
//                    SendCampaignFailedModel sendCampaignFailedModel = new SendCampaignFailedModel
//                    {
//                        TenantId = tenant.TenantId.Value,
//                        CampaignId = campaignId,
//                        PhoneNumber = phoneNumber,
//                        ContactId = ContactId.Value,
//                        TemplateId = templateId,
//                        SentCampaignId = guid
//                    };
//                    lstSendCampaignFailedModel ??= new List<SendCampaignFailedModel>();
//                    lstSendCampaignFailedModel.Add(sendCampaignFailedModel);
//                }
//                return contactCampaignModel;
//            }
//            catch
//            {
//                return new ContactCampaignModel();
//            }
//        }

//        #endregion

//        #region Contact
//        private static ContactsEntity GetFilterContacts(WhatsAppContactsDto contacts, int TenantId)
//        {
//            try
//            {
//                contacts.tenantId ??= TenantId;

//                ContactsEntity contactsEntity = new ContactsEntity();

//                List<WhatsAppContactsDto> lstContacts = new List<WhatsAppContactsDto>();
//                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppContactFilterGet;
//                var sqlParameters = new List<SqlParameter>
//                {
//                    new SqlParameter("@phone",contacts.PhoneNumber)
//                   ,new SqlParameter("@contactName",contacts.ContactName)
//                   ,new SqlParameter("@countryCode",contacts.CountryCode)
//                   ,new SqlParameter("@city",contacts.City)
//                   ,new SqlParameter("@branch",contacts.Branch)
//                   ,new SqlParameter("@joiningFrom",contacts.JoiningFrom)
//                   ,new SqlParameter("@joiningTo",contacts.JoiningTo)
//                   ,new SqlParameter("@orderTimeFrom",contacts.OrderTimeFrom )
//                   ,new SqlParameter("@orderTimeTo", contacts.OrderTimeTo)
//                   ,new SqlParameter("@totalSessions",contacts.TotalSessions)
//                   ,new SqlParameter("@totalOrderMin",contacts.TotalOrderMin)
//                   ,new SqlParameter("@totalOrderMax",contacts.TotalOrderMax)
//                   ,new SqlParameter("@interestedOfOne",contacts.InterestedOfOne)
//                   ,new SqlParameter("@interestedOfTwo",contacts.InterestedOfTwo)
//                   ,new SqlParameter("@interestedOfThree",contacts.InterestedOfThree)
//                   ,new SqlParameter("@isOpt",contacts.IsOpt)
//                   ,new SqlParameter("@TemplateId",contacts.TemplateId)
//                   ,new SqlParameter("@CampaignId",contacts.CampaignId)
//                   ,new SqlParameter("@PageNumber",contacts.pageNumber)
//                   ,new SqlParameter("@PageSize",contacts.pageSize)
//                   ,new SqlParameter("@TenantId",contacts.tenantId)
//                };
//                var OutputParameter = new SqlParameter
//                {
//                    SqlDbType = SqlDbType.BigInt,
//                    ParameterName = "@TotalCount",
//                    Direction = ParameterDirection.Output
//                };
//                sqlParameters.Add(OutputParameter);

//                var OutputParameter2 = new SqlParameter
//                {
//                    SqlDbType = SqlDbType.BigInt,
//                    ParameterName = "@TotalOptOut",
//                    Direction = ParameterDirection.Output
//                };
//                sqlParameters.Add(OutputParameter2);

//                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapFilterContacts, Constants.ConnectionString).ToList();

//                contactsEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
//                contactsEntity.TotalOptOut = Convert.ToInt32(OutputParameter2.Value);
//                contactsEntity.contacts = lstContacts;
//                return contactsEntity;
//            }
//            catch
//            {
//                return new ContactsEntity();
//            }
//        }
//        private static ContactsEntity GetExternalContacts(long campaignId, long templateId, int pageNumber, int pageSize, int? tenantId = null)
//        {
//            try
//            {
//                ContactsEntity contactsEntity = new ContactsEntity();

//                List<WhatsAppContactsDto> lstContacts = new List<WhatsAppContactsDto>();
//                var SP_Name = Constants.Contacts.SP_ContactsExternalByCampaignGet;

//                var sqlParameters = new List<SqlParameter>
//                {
//                    new SqlParameter("@PageSize",pageSize)
//                   ,new SqlParameter("@PageNumber",pageNumber)
//                   ,new SqlParameter("@TenantId",tenantId)
//                   ,new SqlParameter("@CampaignId",campaignId)
//                   ,new SqlParameter("@TemplateId",templateId)
//                };
//                var OutputParameter = new SqlParameter
//                {
//                    SqlDbType = SqlDbType.BigInt,
//                    ParameterName = "@TotalCount",
//                    Direction = ParameterDirection.Output
//                };

//                sqlParameters.Add(OutputParameter);
//                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapExternalContacts, Constants.ConnectionString).ToList();


//                contactsEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
//                contactsEntity.contacts = lstContacts;
//                return contactsEntity;
//            }
//            catch 
//            {
//                return new ContactsEntity();
//            }
//        }
//        private static void AddContactCampaign(List<ContactCampaignModel> lstcontactCampaignModel)
//        {
//            var SP_Name = Constants.Contacts.SP_ContactsCampaignlBulkAddNew;

//            var sqlParameters = new List<SqlParameter>
//            {
//                new SqlParameter("@ContactsCampaignJson",JsonConvert.SerializeObject(lstcontactCampaignModel))
//            };
//            SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            
//        }
//        private static void UpdateFailedContact(List<SendCampaignFailedModel> lstSendCampaignFailedModel)
//        {
//            var SP_Name = Constants.WhatsAppCampaign.SP_ContactsFailedCampaignBulkAdd;

//            var sqlParameters = new List<SqlParameter> {
//                    new SqlParameter("@ContactsFailedCampaignJson",JsonConvert.SerializeObject(lstSendCampaignFailedModel) )
//            };
//            SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
//        }
//        private static async Task UpdateCustomerTemp(string phoneNumber, string msg, string type, string mediaUrl, int TenantId, string UserId, string massageId = "", string templateId ="",string D360Key="", string campaignId = "")
//        {
//            string userId = TenantId + "_" + phoneNumber;
//            string displayName = "";



//            var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
//            var Customer = itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.CustomerItem && a.userId == userId).Result;
//            CustomerChat customerChat = new CustomerChat()
//            {
//                messageId = massageId,
//                TenantId = TenantId,
//                userId = userId,
//                text = msg,
//                type = type,
//                CreateDate = DateTime.Now,
//                status = (int)Messagestatus.New,
//                sender = MessageSenderType.TeamInbox,
//                mediaUrl = mediaUrl,
//                UnreadMessagesCount = 0,
//                agentName = "admin",
//                agentId = UserId,
//            };


//            var itemsCollection2 = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection);
//            await itemsCollection2.CreateItemAsync(customerChat);
//            if (Customer == null)
//            {
                

//                var cont = new ContactDto()
//                {
//                    loyalityPoint = 0,
//                    TotalOrder = 0,
//                    TakeAwayOrder = 0,
//                    DeliveryOrder = 0,
//                    UserId = userId,
//                    DisplayName = displayName,
//                    AvatarUrl = "",
//                    CreatorUserId = 1,
//                    Description = "",
//                    EmailAddress = "",
//                    IsDeleted = false,
//                    CreationTime = DateTime.Now,
//                    IsLockedByAgent = false,
//                    IsConversationExpired = false,
//                    IsBlock = false,
//                    IsOpen = false,
//                    LockedByAgentName = "",
//                    PhoneNumber = phoneNumber,
//                    SunshineAppID = "",
//                    Website = "",
//                    TenantId = TenantId,
//                    DeletionTime = null,
//                    DeleterUserId = null,
//                    ConversationsCount = 1,


//                };

//                //var idcont = InsertContact(cont);
//                var idcont = CreateContactDB(cont);


//                var CustomerModel = new CustomerModel()
//                {
//                    TennantPhoneNumberId=D360Key,
//                    ConversationsCount = 0,
//                    ContactID = idcont.ToString(),
//                    IsComplaint = false,
//                    userId = userId,
//                    displayName = displayName,
//                    avatarUrl = "",
//                    type = type,
//                    D360Key = D360Key,
//                    CreateDate = DateTime.Now,
//                    IsLockedByAgent = false,
//                    LockedByAgentName = "infoseedBot",
//                    IsOpen = false,
//                    agentId = 100000,
//                    IsBlock = false,
//                    IsConversationExpired = false,
//                    CustomerChatStatusID = (int)CustomerChatStatus.Active,
//                    CustomerStatusID = (int)CustomerStatus.Active,
//                    LastMessageData = DateTime.Now,
//                    IsNew = true,
//                    TenantId = TenantId,
//                    phoneNumber = phoneNumber,
//                    UnreadMessagesCount = 1,
//                    IsNewContact = true,
//                    IsBotChat = true,
//                    IsBotCloseChat = false,
//                    loyalityPoint = 0,
//                    TotalOrder = 0,
//                    TakeAwayOrder = 0,
//                    DeliveryOrder = 0,
//                    customerChat = new CustomerChat()
//                    {
//                        CreateDate = DateTime.Now,
//                    },
//                    creation_timestamp= 0,
//                    expiration_timestamp= 0,
//                    // ConversationId = model.statuses.FirstOrDefault().conversation.id
//                    CustomerStepModel=new CustomerStepModel() { ChatStepId=-1, ChatStepPervoiusId=0, IsLiveChat=false }
//                };
//                CustomerModel.customerChat= customerChat;

//                CustomerModel.templateId=templateId;
//                CustomerModel.campaignId = campaignId;
//                CustomerModel.IsTemplateFlow = true;
//                CustomerModel.TemplateFlowDate=DateTime.UtcNow;
//                CustomerModel.getBotFlowForViewDto=new AppFunEntities.GetBotFlowForViewDto();

//                CustomerModel.CustomerStepModel=new CustomerStepModel() { ChatStepId=-1, ChatStepPervoiusId=0 };

//                var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;


//            }
//            else
//            {
//                Customer.customerChat= customerChat;

//                Customer.templateId=templateId;
//                Customer.campaignId = campaignId;
//                Customer.IsTemplateFlow = true;
//                Customer.TemplateFlowDate=DateTime.UtcNow;
//                Customer.getBotFlowForViewDto=new AppFunEntities.GetBotFlowForViewDto();

//                Customer.CustomerStepModel=new CustomerStepModel() { ChatStepId=-1, ChatStepPervoiusId=0 };

//                var Result = itemsCollection.UpdateItemAsync(Customer._self, Customer).Result;

//            }

//            SocketIOManager.SendChat(Customer, TenantId);


//        }
//        private static int CreateContactDB(ContactDto contactDto)
//        {
//            try
//            {
//                var SP_Name = Constants.Contacts.SP_ContactsAdd;
//                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
//                {
//                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto))
//                };

//                var OutputParameter = new System.Data.SqlClient.SqlParameter();
//                OutputParameter.SqlDbType = SqlDbType.Int;
//                OutputParameter.ParameterName = "@ContactId";
//                OutputParameter.Direction = ParameterDirection.Output;
//                sqlParameters.Add(OutputParameter);
//                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

//                return (int)OutputParameter.Value;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        private static async Task UpdateCustomerChatAsync(string phoneNumber, string msg, string type, string mediaUrl, int TenantId, string UserId, string massageId = "", string templateId = "")
//        {
//            string userId = TenantId + "_" + phoneNumber;
//            CustomerChat customerChat = new CustomerChat()
//            {
//                messageId = massageId,
//                TenantId = TenantId,
//                userId = userId,
//                text = msg,
//                type = type,
//                CreateDate = DateTime.Now,
//                status = (int)Messagestatus.New,
//                sender = MessageSenderType.TeamInbox,
//                mediaUrl = mediaUrl,
//                UnreadMessagesCount = 0,
//                agentName = "admin",
//                agentId = UserId,
//            };
//            var itemsCollection2 = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection);
//            await itemsCollection2.CreateItemAsync(customerChat);

          
//        }
//        #endregion

//        #region Statistic
//        private static GetAllDashboard GetStatistics(int TenantId)
//        {
//            try
//            {
//                GetAllDashboard statistics = new GetAllDashboard();
//                var SP_Name = Constants.Dashboard.SP_ConversationMeasurementsGet;
//                var sqlParameters = new List<SqlParameter> {
//                    new SqlParameter("@Year",DateTime.UtcNow.Year)
//                    ,new SqlParameter("@Month",DateTime.UtcNow.Month)
//                    ,new SqlParameter("@TenantId",TenantId)
//                };
//                statistics = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapDashboardStatistic, Constants.ConnectionString).FirstOrDefault();
//                statistics.RemainingUIConversation = statistics.TotalUIConversation - statistics.TotalUsageUIConversation;
//                statistics.RemainingBIConversation = statistics.TotalBIConversation - statistics.TotalUsageBIConversation;
//                statistics.RemainingFreeConversation = statistics.TotalFreeConversationWA - statistics.TotalUsageFreeConversation;

//                return statistics;
//            }
//            catch
//            {
//                return new GetAllDashboard();
//            }
//        }
//        private static int GetTenantDailyLimit(int tenantId)
//        {
//            try
//            {
//                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppContactCountFilterGet;
//                var sqlParameters = new List<SqlParameter>
//                {
//                    new SqlParameter("@TenantId",tenantId)
//                };
//                var OutputParameter = new SqlParameter
//                {
//                    SqlDbType = SqlDbType.Int,
//                    ParameterName = "@TotalCount",
//                    Direction = ParameterDirection.Output
//                };
//                sqlParameters.Add(OutputParameter);
//                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

//                return Convert.ToInt32(OutputParameter.Value);
//            }
//            catch
//            {
//                return 0;
//            }
//        }
//        private static int getDailyLimitByTenantId(int tenantId)
//        {
//            try
//            {
//                var SP_Name = Constants.WhatsAppCampaign.SP_DailyLimitByTenantId;
//                var sqlParameters = new List<SqlParameter>
//                {
//                    new SqlParameter("@TenantId",tenantId)


//                };
//                var OutputParameter = new SqlParameter
//                {
//                    SqlDbType = SqlDbType.Int,
//                    ParameterName = "@TotalCount",
//                    Direction = ParameterDirection.Output
//                };
//                sqlParameters.Add(OutputParameter);


//                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);


//                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        private static void UpdateBIConversation(int TenantId, decimal? usageBI, decimal? usageFree)
//        {
//            var SP_Name = Constants.Dashboard.SP_ConversationMeasurementBIUpdate;
//            var sqlParameters = new List<SqlParameter> {

//                new SqlParameter("@UsageBi",usageBI),
//                new SqlParameter("@UsageFree",usageFree),
//                new SqlParameter("@TenantId", TenantId)
//            };
//            SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
//        }
//        #endregion

//        #region Mapper
//        public static WhatsAppScheduledCampaignModel MapScheduledCampaign(IDataReader dataReader)
//        {
//            try
//            {
//                WhatsAppScheduledCampaignModel model = new WhatsAppScheduledCampaignModel
//                {
//                    Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
//                    SendDateTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SendDateTime"),
//                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
//                    CampaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
//                    TemplateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
//                    ContactsJson = SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"),
//                    IsExternalContact = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
//                    CreatedByUserId = SqlDataHelper.GetValue<long>(dataReader, "CreatedByUserId")

//                };
//                return model;
//            }
//            catch
//            {
//                return new WhatsAppScheduledCampaignModel();
//            }
//        }
//        private static MessageTemplateModel MapTemplate(IDataReader dataReader)
//        {
//            try
//            {
//                MessageTemplateModel _MessageTemplateModel = new MessageTemplateModel
//                {
//                    name = SqlDataHelper.GetValue<string>(dataReader, "Name"),
//                    language = SqlDataHelper.GetValue<string>(dataReader, "Language"),
//                    category = SqlDataHelper.GetValue<string>(dataReader, "Category"),
//                    id = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppTemplateId"),
//                    LocalTemplateId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
//                    mediaType = SqlDataHelper.GetValue<string>(dataReader, "MediaType"),
//                    mediaLink = SqlDataHelper.GetValue<string>(dataReader, "MediaLink"),
//                    isDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted"),
//                    VariableCount = SqlDataHelper.GetValue<int>(dataReader, "VariableCount"),
//                };

//                var components = SqlDataHelper.GetValue<string>(dataReader, "Components");
//                var options = new JsonSerializerOptions { WriteIndented = true };
//                _MessageTemplateModel.components = System.Text.Json.JsonSerializer.Deserialize<List<WhatsAppComponentModel>>(components, options);

//                return _MessageTemplateModel;
//            }
//            catch
//            {
//                return new MessageTemplateModel();
//            }

//        }
//        public static GetAllDashboard MapDashboardStatistic(IDataReader dataReader)
//        {
//            try
//            {
//                GetAllDashboard GetAllDashboard = new GetAllDashboard
//                {
//                    TotalFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalFreeConversationWA"),
//                    TotalUsageFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversationWA"),
//                    TotalUsageFreeUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeUIWA"),
//                    TotalUsageFreeBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeBIWA"),

//                    TotalUsagePaidConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidConversationWA"),
//                    TotalUsagePaidUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidUIWA"),
//                    TotalUsagePaidBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidBIWA"),

//                    TotalUsageFreeConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversation"),
//                    TotalUIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUIConversation"),
//                    TotalUsageUIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageUIConversation"),
//                    TotalBIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalBIConversation"),
//                    TotalUsageBIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageBIConversation")
//                };
//                return GetAllDashboard;
//            }
//            catch
//            {
//                return new GetAllDashboard();
//            }

//        }
//        private static WhatsAppContactsDto MapFilterContacts(IDataReader dataReader)
//        {
//            try
//            {
//                WhatsAppContactsDto contacts = new WhatsAppContactsDto
//                {
//                    Id = SqlDataHelper.GetValue<int>(dataReader, "id"),
//                    ContactName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName"),
//                    PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
//                    CustomerOPT = SqlDataHelper.GetValue<string>(dataReader, "CustomerOPT")
//                };
//                return contacts;
//            }
//            catch
//            {
//                return new WhatsAppContactsDto();
//            }

//        }
//        private static WhatsAppContactsDto MapExternalContacts(IDataReader dataReader)
//        {
//            try
//            {
//                WhatsAppContactsDto contacts = new WhatsAppContactsDto
//                {
//                    Id = SqlDataHelper.GetValue<int>(dataReader, "Id"),
//                    CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT"),
//                    ContactName = SqlDataHelper.GetValue<string>(dataReader, "ContactName"),
//                    PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber")
//                };
//                var templateVariables = SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables");
//                var options = new JsonSerializerOptions { WriteIndented = true };
//                contacts.templateVariables = System.Text.Json.JsonSerializer.Deserialize<TemplateVariables>(templateVariables, options);
//                return contacts;
//            }
//            catch
//            {
//                return new WhatsAppContactsDto();
//            }

//        }
//        public static UsersDashModel MapUserInfo(IDataReader dataReader)
//        {
//            try
//            {
//                UsersDashModel model = new UsersDashModel();
//                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
//                model.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
//                model.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
//                model.EmailAddress = SqlDataHelper.GetValue<string>(dataReader, "EmailAddress");

//                return model;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//        public static string MapCampignInfo(IDataReader dataReader)
//        {
//            try
//            {
//                string Title;
//                Title = SqlDataHelper.GetValue<string>(dataReader, "Title");

//                return Title;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//        public static CountryCodeDashModel MapCountryInfo(IDataReader dataReader)
//        {
//            try
//            {
//                CountryCodeDashModel countryCodeDashModel = new CountryCodeDashModel();
//                countryCodeDashModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
//                countryCodeDashModel.Country = SqlDataHelper.GetValue<string>(dataReader, "Country");
//                countryCodeDashModel.Region = SqlDataHelper.GetValue<string>(dataReader, "Region");
//                countryCodeDashModel.CountryCallingCode = SqlDataHelper.GetValue<string>(dataReader, "CountryCallingCode");
//                countryCodeDashModel.Currency = SqlDataHelper.GetValue<string>(dataReader, "Currency");
//                countryCodeDashModel.MarketingPrice = SqlDataHelper.GetValue<float>(dataReader, "MarketingPrice");
//                countryCodeDashModel.UtilityPrice = SqlDataHelper.GetValue<float>(dataReader, "UtilityPrice");
//                countryCodeDashModel.AuthenticationPrice = SqlDataHelper.GetValue<float>(dataReader, "AuthenticationPrice");
//                countryCodeDashModel.ServicePrice = SqlDataHelper.GetValue<float>(dataReader, "ServicePrice");
//                countryCodeDashModel.ISOCountryCodes = SqlDataHelper.GetValue<string>(dataReader, "ISOCountryCodes");

//                return countryCodeDashModel;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//        public static WalletDashModel MapWallet(IDataReader dataReader)
//        {
//            try
//            {
//                WalletDashModel model = new WalletDashModel();
//                model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
//                model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
//                model.TotalAmount = SqlDataHelper.GetValue<decimal>(dataReader, "TotalAmount");
//                model.OnHold = SqlDataHelper.GetValue<decimal>(dataReader, "OnHold");
//                model.DepositDate = SqlDataHelper.GetValue<DateTime>(dataReader, "DepositDate");

//                return model;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//        #endregion
//    }
//}