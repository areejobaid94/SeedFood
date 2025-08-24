using Abp.Extensions;
using Abp.Runtime.Caching;
using CampaignProject.Models;
using Framework.Data;
using GraphQL.Language.AST;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Azure.Documents;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignProject.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsManagerController : MessagingPortalControllerBase
    {

        private readonly ILogger<CampaignsManagerController> _logger;
        private readonly IDocumentClient _IDocumentClient;
      //  private readonly ICacheManager _cacheManager;    
        public CampaignsManagerController(ILogger<CampaignsManagerController> logger, IDocumentClient IDocumentClient)
        { 
        
            _logger = logger;
            _IDocumentClient=IDocumentClient;
          //  _cacheManager=cacheManager;
        }
        [Route("PreparingCampaign")]
        [HttpPost]// from Core Applications 
        public async Task<IActionResult> PreparingCampaignAsync( int campaignId)
        {

            //Preparing contacts ( Variablles ) Get from table SendCampaignNow DB
            //Preparing  MessageTemplateModel  Get from table SendCampaignNow DB
            //Preparing  ListContactCampaign  Get from table SendCampaignNow DB

            var GetCampaign = GetCampaignFun(campaignId);
            string json = JsonConvert.SerializeObject(GetCampaign, Formatting.Indented);

            var TenantId = 0;
            MessageTemplateModel model = new Models.MessageTemplateModel();
            List<ListContactCampaign> listContact = new List<ListContactCampaign>();
            TemplateVariablles2 templateVariablles = new TemplateVariablles2();
            HeaderVariablesTemplate2 headerVariablesTemplates = new HeaderVariablesTemplate2();
            FirstButtonURLVariabllesTemplate2 firstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate2();
            SecondButtonURLVariabllesTemplate2 secondButtonURLVariabllesTemplate = new SecondButtonURLVariabllesTemplate2();
            CarouselVariabllesTemplate carouselVariabllesTemplate = new CarouselVariabllesTemplate();

            bool IsExternal = false;

            if (GetCampaign.Count()>0)
            {
                TenantId=GetCampaign.FirstOrDefault().TenantId;
                model = GetCampaign.FirstOrDefault().model;
                IsExternal= GetCampaign.FirstOrDefault().IsExternal;
                foreach (var x in GetCampaign)
                {
                    listContact.AddRange(x.contacts); // Assuming x.contacts is a list of ListContactCampaign
                }


            }
            //var xyd =BuildWhatsAppTemplatePayload(
            //     model,
            //     listContact,
            //     templateVariablles);

            foreach (var contacts in listContact)
            {


                var teaminboxmsg = "";


                try
                {
                    if (GetCampaign.FirstOrDefault().templateVariablles!=null)
                    {
                        if (GetCampaign.FirstOrDefault().templateVariablles.VarOne != null)
                        {
                            teaminboxmsg = teaminboxmsg.Replace("{{1}}", GetCampaign.FirstOrDefault().templateVariablles.VarOne);

                            templateVariablles = GetCampaign.FirstOrDefault().templateVariablles;

                        }
                        if (GetCampaign.FirstOrDefault().templateVariablles.VarTwo != null)
                        {
                            teaminboxmsg = teaminboxmsg.Replace("{{2}}", GetCampaign.FirstOrDefault().templateVariablles.VarTwo);

                        }
                        if (GetCampaign.FirstOrDefault().templateVariablles.VarThree != null)
                        {
                            teaminboxmsg = teaminboxmsg.Replace("{{3}}", GetCampaign.FirstOrDefault().templateVariablles.VarThree);

                        }
                        if (GetCampaign.FirstOrDefault().templateVariablles.VarFour != null)
                        {
                            teaminboxmsg = teaminboxmsg.Replace("{{4}}", GetCampaign.FirstOrDefault().templateVariablles.VarFour);

                        }
                        if (GetCampaign.FirstOrDefault().templateVariablles.VarFive != null)
                        {
                            teaminboxmsg = teaminboxmsg.Replace("{{5}}", GetCampaign.FirstOrDefault().templateVariablles.VarFive);

                        }
                    }

                }


               
                catch
                {

                }


                try
                {

                    if (contacts.templateVariables!=null)
                    {


                        if (contacts.templateVariables.VarOne!=null)
                        {
                            teaminboxmsg= teaminboxmsg.Replace("{{1}}", contacts.templateVariables.VarOne);


                            templateVariablles=contacts.templateVariables;

                        }
                        if (contacts.templateVariables.VarTwo!=null)
                        {
                            teaminboxmsg= teaminboxmsg.Replace("{{2}}", contacts.templateVariables.VarTwo);

                        }
                        if (contacts.templateVariables.VarThree!=null)
                        {
                            teaminboxmsg= teaminboxmsg.Replace("{{3}}", contacts.templateVariables.VarThree);

                        }
                        if (contacts.templateVariables.VarFour!=null)
                        {
                            teaminboxmsg= teaminboxmsg.Replace("{{4}}", contacts.templateVariables.VarFour);

                        }
                        if (contacts.templateVariables.VarFive!=null)
                        {
                            teaminboxmsg= teaminboxmsg.Replace("{{5}}", contacts.templateVariables.VarFive);

                        }

                    }



                }
                catch
                {


                }






                    if (templateVariablles.VarOne==" ")
                    {

                        templateVariablles.VarOne="";
                    }
                    if (templateVariablles.VarTwo==" ")
                    {

                        templateVariablles.VarTwo="";
                    }
                    if (templateVariablles.VarThree==" ")
                    {

                        templateVariablles.VarThree="";
                    }
                    if (templateVariablles.VarFour==" ")
                    {

                        templateVariablles.VarFour="";
                    }
                    if (templateVariablles.VarFive==" ")
                    {

                        templateVariablles.VarFive="";
                    }


                   
                



                PostMessageTemplateModel _postMessageTemplateModel = PrepareMessageTemplate(model, contacts, IsExternal, templateVariablles, headerVariablesTemplates, firstButtonURLVariabllesTemplate, secondButtonURLVariabllesTemplate);
                var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });



                contacts.PostBody=postBody;

            }



            //Preparing  AccessToken and D360Key  Get from Tenant CosmDB
            // Cache Tenant


            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantId);



            CampaignModel campaignModel = new CampaignModel();

            try
            {

                var AccessToken = tenant.AccessToken;
                var D360Key = tenant.D360Key;
                var ListContacts = listContact;
                var NumberMessagesPerSecond = 25;




                campaignModel.AccessToken=AccessToken;
                campaignModel.D360Key=D360Key;
                campaignModel.contacts=ListContacts;
                campaignModel.NumberMessagesPerSecond=NumberMessagesPerSecond;
                campaignModel.TenantId=TenantId;
                //send to CampaignSendController (List contacts) ,(AccessToken),(D360Key),(NumberMessagesPerSecond),

            }
            catch
            {



            }



            return Ok(campaignModel);
        }
        [HttpPost("UpdateCampaignStatus")]// from CampaignSendController 
        public IActionResult UpdateCampaignStatus(int campaignId,int status)
        {

            //get the reszult from CampaignSendController with massage id and status 

            //if the status  faled  should get the list of filad number to send it agin 

            // if the status compleat all number ,match the massage id 
            WhatsAppCampaignModel whatsAppCampaignModel2 = new WhatsAppCampaignModel
            {
                id = campaignId,
                sentTime = DateTime.UtcNow,
                status = status // as sent
            };
            updateWhatsAppCampaign(whatsAppCampaignModel2);

            return Ok();
        }
        [HttpGet("GetCampaignStatusWebHook")]// from webhook 
        public IActionResult GetCampaignStatusWebHook( CampaignModel campaign)
        {
            // get from webhook all the status   sending *3 (send ,deleverd ,read)

            //match the massage id



            return Ok();
        }


        #region privet
        private static List<SendCampaignNow> GetCampaignFun(long CampaignId)
        {
            try
            {
                var SP_Name = "GetSendCampaignNowById";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CampaignId", CampaignId)
                };
                List<SendCampaignNow> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, SettingsModel.ConnectionStrings).ToList();
                return model;
            }
            catch
            {
                return new List<SendCampaignNow>();
            }
        }


        private static PostMessageTemplateModel PrepareMessageTemplate(
                MessageTemplateModel objWhatsAppTemplateModel,
                ListContactCampaign contact,
                bool isExternal,
                TemplateVariablles2 templateVariables,
                HeaderVariablesTemplate2 headerVariablesTemplates,
                FirstButtonURLVariabllesTemplate2 firstButtonUrlVariables,
                SecondButtonURLVariabllesTemplate2 secondButtonUrlVariables)
        {
            try
            {
                var postMessageTemplateModel = new PostMessageTemplateModel
                {
                    messaging_product = "whatsapp",
                    to = contact.PhoneNumber,
                    type = "template",
                    template = new WhatsAppTemplateModel
                    {
                        name = objWhatsAppTemplateModel.name,
                        language = new WhatsAppLanguageModel { code = objWhatsAppTemplateModel.language },
                        components = new List<Component>()
                    }
                };

                if (objWhatsAppTemplateModel.components != null)
                {
                    var carouselComponent = objWhatsAppTemplateModel.components
                        .FirstOrDefault(c => c.type.Equals("carousel", StringComparison.OrdinalIgnoreCase));
                    if (carouselComponent != null || objWhatsAppTemplateModel.sub_category == "carousel")
                    {
                        var bodyComponent1 = objWhatsAppTemplateModel.components.FirstOrDefault(c => c.type == "BODY");

                        if (bodyComponent1 != null)
                        {
                            var topLevelBody = new Component
                            {
                                type = "BODY",
                                parameters = GenerateTextParameters(
                                     contact.templateVariables?.VarOne,
                                   contact.templateVariables?.VarTwo,
                                    contact.templateVariables?.VarThree,
                                    contact.templateVariables?.VarFour,
                                     contact.templateVariables?.VarFive,
                                    contact.templateVariables?.VarSix,
                                    contact.templateVariables?.VarSeven,
                                     contact.templateVariables?.VarEight,
                                     contact.templateVariables?.VarNine,
                                     contact.templateVariables?.VarTen,
                                     contact.templateVariables?.VarEleven,
                                     contact.templateVariables?.VarTwelve,
                                     contact.templateVariables?.VarThirteen,
                                     contact.templateVariables?.VarFourteen,
                                     contact.templateVariables?.VarFifteen
                                )
                            };

                            if (topLevelBody.parameters.Count > 0)
                            {
                                postMessageTemplateModel.template.components.Add(topLevelBody);
                            }
                        }
                        var carousel = new Component
                        {
                            type = "CAROUSEL",
                            cards = new List<CardComponent>()
                        };
                        if (contact.carouselVariabllesTemplate != null)
                        {
                            int indexCard = 0;
                            foreach (var contactCard in contact.carouselVariabllesTemplate?.cards)
                            {
                                var templateCard = carouselComponent.cards[indexCard];
                                var cardComponent = new CardComponent
                                {
                                    card_index = contactCard.CardIndex,
                                    components = new List<Component>()
                                };
              
                     

                                var headerComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("HEADER", StringComparison.OrdinalIgnoreCase));
                                if (headerComponent != null)
                                {
                                    var componentHeader = new Component
                                    {
                                        type = "HEADER",
                                        parameters = new List<Parameter>()
                                    };

                                    var parameterHeader = new Parameter
                                    {
                                        type = headerComponent.format.ToLower()
                                    };

                                    if (headerComponent.format == "TEXT")
                                    {
                                        var headerText = contactCard.Variables.VarOne;
                                        if (!string.IsNullOrWhiteSpace(headerText))
                                        {
                                            parameterHeader.type = "text";
                                            parameterHeader.text = headerText;
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                    }
                                    else if (headerComponent.format == "IMAGE")
                                    {
                                        var mediaID = headerComponent.example?.mediaID;
                                        if (!string.IsNullOrEmpty(mediaID))
                                        {
                                            parameterHeader.type = "image";
                                            parameterHeader.image = new ImageTemplate { id = mediaID };
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                        else if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
                                        {
                                            parameterHeader.type = "image";
                                            parameterHeader.image = new ImageTemplate { link = objWhatsAppTemplateModel.mediaLink };
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                    }
                                    else if (headerComponent.format == "VIDEO")
                                    {
                                        var mediaID = headerComponent.example?.mediaID;
                                        if (!string.IsNullOrEmpty(mediaID))
                                        {
                                            parameterHeader.type = "video";
                                            parameterHeader.video = new VideoTemplate { id = mediaID };
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                    }
                                }

                                var bodyComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("BODY", StringComparison.OrdinalIgnoreCase));
                                if (bodyComponent != null && contactCard.Variables !=null)
                                {
                                    var componentBody = new Component
                                    {
                                        type = "BODY",
                                        parameters = GenerateTextParameters(
                                            contactCard.Variables.VarOne,
                                            contactCard.Variables.VarTwo,
                                            contactCard.Variables.VarThree,
                                            contactCard.Variables.VarFour,
                                            contactCard.Variables.VarFive,
                                            contactCard.Variables.VarSix,
                                            contactCard.Variables.VarSeven,
                                            contactCard.Variables.VarEight,
                                            contactCard.Variables.VarNine,
                                            contactCard.Variables.VarTen,
                                            contactCard.Variables.VarEleven,
                                            contactCard.Variables.VarTwelve,
                                            contactCard.Variables.VarThirteen,
                                            contactCard.Variables.VarFourteen,
                                            contactCard.Variables.VarFifteen
                                        )
                                    };

                                    if (componentBody.parameters.Count > 0)
                                    {
                                        cardComponent.components.Add(componentBody);
                                    }
                                }

                                var buttonsComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("BUTTONS", StringComparison.OrdinalIgnoreCase));
                                if (buttonsComponent != null && buttonsComponent.buttons != null)
                                {
                                    int buttonIndex = -1;
                                    var checkButtonURL1 = true;
                                    var checkButtonURL2 = true;
                                    foreach (var button in buttonsComponent.buttons)
                                    {
                                        var buttonComponent = new Component();
                                        buttonIndex++;

                                        switch (button.type)
                                        {
                                            case "QUICK_REPLY":
                                                buttonComponent.type = "BUTTON";
                                                buttonComponent.sub_type = "quick_reply";
                                                buttonComponent.index = buttonIndex;
                                                buttonComponent.parameters = new List<Parameter>
                                    {
                                        new Parameter {
                                            type = "payload",
                                            payload = button.text?.ToLower().Replace(" ", "-") ?? "default-payload"
                                        }
                                    };
                                                cardComponent.components.Add(buttonComponent);
                                                break;

                                            case "URL":
                                                if (checkButtonURL1 && button.type == "URL" && button.example != null && !string.IsNullOrWhiteSpace(contactCard.firstButtonURLVariabllesTemplate?.VarOne))
                                                {
                                                    var urlText = contactCard.firstButtonURLVariabllesTemplate?.VarOne ?? button.text;
                                                    if (!string.IsNullOrEmpty(urlText))
                                                    {
                                                        buttonComponent.type = "BUTTON";
                                                        buttonComponent.sub_type = "url";
                                                        buttonComponent.index = buttonIndex;
                                                        buttonComponent.parameters = new List<Parameter>
                                                           {
                                                               new Parameter { type = "text", text = urlText }
                                                           };
                                                        cardComponent.components.Add(buttonComponent);
                                                    }
                                                    checkButtonURL1=false;
                                                    break;
                                                }
                                                if (checkButtonURL2 && button.type == "URL" && button.example != null && !string.IsNullOrWhiteSpace(contactCard.secondButtonURLVariabllesTemplate?.VarOne))
                                                {
                                                    var urlText = contactCard.secondButtonURLVariabllesTemplate?.VarOne ?? button.text;
                                                    if (!string.IsNullOrEmpty(urlText))
                                                    {
                                                        buttonComponent.type = "BUTTON";
                                                        buttonComponent.sub_type = "url";
                                                        buttonComponent.index = buttonIndex;
                                                        buttonComponent.parameters = new List<Parameter>
                                                           {
                                                               new Parameter { type = "text", text = urlText }
                                                           };
                                                        cardComponent.components.Add(buttonComponent);
                                                    }
                                                    checkButtonURL2 = false;
                                                    break;
                                                }
                                                break;

                                            case "PHONE_NUMBER":
                                                if (!string.IsNullOrEmpty(button.phone_number))
                                                {
                                                    buttonComponent.type = "BUTTON";
                                                    buttonComponent.sub_type = "call-phone-number";
                                                    buttonComponent.index = buttonIndex;
                                                    buttonComponent.parameters = new List<Parameter>
                                        {
                                            new Parameter {
                                                type = "text",
                                                text = button.text
                                            }
                                        };
                                                    cardComponent.components.Add(buttonComponent);
                                                }
                                                break;
                                            case "COPY_CODE":
                                                if (!string.IsNullOrEmpty(button.phone_number))
                                                {
                                                    buttonComponent.type = "BUTTON";
                                                    buttonComponent.sub_type = "copy_code";
                                                    buttonComponent.index = buttonIndex;
                                                    buttonComponent.parameters = new List<Parameter>
                                        {
                                            new Parameter {
                                                type = "text",
                                                text = button.text
                                            }
                                        };
                                                    cardComponent.components.Add(buttonComponent);
                                                }
                                                break;

                                        }
                                    }
                                }

                                carousel.cards.Add(cardComponent);
                            }
                            indexCard++;


                        }
                        else
                        {   
                            var cardIndex=0;

                            foreach (var contactCard in carouselComponent?.cards)
                            {
                                var header = contactCard.components[0];
                                var body = contactCard.components[1];
                                var buttons = contactCard.components[2];
                               var templateCard = carouselComponent.cards[cardIndex];
                                var cardComponent = new CardComponent
                                {
                                    card_index = cardIndex,
                                    components = new List<Component>()
                                };

                                var headerComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("HEADER", StringComparison.OrdinalIgnoreCase));
                                if (headerComponent != null)
                                {

                                    var componentHeader = new Component
                                    {
                                        type = "HEADER",
                                        parameters = new List<Parameter>()
                                    };

                                    var parameterHeader = new Parameter
                                    {
                                        type = headerComponent.format.ToLower()
                                    };
                                    if (headerComponent.format == "IMAGE")
                                    {
                                        var mediaID = headerComponent.example?.mediaID;
                                        if (!string.IsNullOrEmpty(mediaID))
                                        {
                                            parameterHeader.type = "image";
                                            parameterHeader.image = new ImageTemplate { id = mediaID };
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                        else if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
                                        {
                                            parameterHeader.type = "image";
                                            parameterHeader.image = new ImageTemplate { link = objWhatsAppTemplateModel.mediaLink };
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                    }
                                    else if (headerComponent.format == "VIDEO")
                                    {
                                        var mediaID = headerComponent.example?.mediaID;
                                        if (!string.IsNullOrEmpty(mediaID))
                                        {
                                            parameterHeader.type = "video";
                                            parameterHeader.video = new VideoTemplate { id = mediaID };
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                        else if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
                                        {
                                            parameterHeader.type = "image";
                                            parameterHeader.image = new ImageTemplate { link = objWhatsAppTemplateModel.mediaLink };
                                            componentHeader.parameters.Add(parameterHeader);
                                            cardComponent.components.Add(componentHeader);
                                        }
                                    }
                                }

                                var bodyComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("BODY", StringComparison.OrdinalIgnoreCase));

                                var buttonsComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("BUTTONS", StringComparison.OrdinalIgnoreCase));
                                if (buttonsComponent != null && buttonsComponent.buttons != null)
                                {
                                    int buttonIndex = 0;

                                    foreach (var button in buttonsComponent.buttons)
                                    {
                                        var buttonComponent = new Component();

                                        switch (button.type)
                                        {
                                            case "QUICK_REPLY":
                                                buttonComponent.type = "BUTTON";
                                                buttonComponent.sub_type = "quick_reply";
                                                buttonComponent.index = buttonIndex;
                                                buttonComponent.parameters = new List<Parameter>
                                    {
                                        new Parameter {
                                            type = "payload",
                                            payload = button.text?.ToLower().Replace(" ", "-") ?? "default-payload"
                                        }
                                    };
                                                cardComponent.components.Add(buttonComponent);
                                                buttonIndex++;
                                                break;

                                        //    case "URL":
                                        //        var urlText = contactCard.firstButtonURLVariabllesTemplate?.VarOne ?? button.text;
                                        //        if (!string.IsNullOrEmpty(urlText))
                                        //        {
                                        //            buttonComponent.type = "BUTTON";
                                        //            buttonComponent.sub_type = "url";
                                        //            buttonComponent.index = buttonIndex;
                                        //            buttonComponent.parameters = new List<Parameter>
                                        //{
                                        //    new Parameter { type = "text", text = urlText }
                                        //};
                                        //            cardComponent.components.Add(buttonComponent);
                                        //            buttonIndex++;
                                        //        }
                                        //        break;

                                        //    case "PHONE_NUMBER":
                                        //        if (!string.IsNullOrEmpty(button.phone_number))
                                        //        {
                                        //            buttonComponent.type = "BUTTON";
                                        //            buttonComponent.sub_type = "call-phone-number";
                                        //            buttonComponent.index = buttonIndex;
                                        //            buttonComponent.parameters = new List<Parameter>
                                        //{
                                        //    new Parameter {
                                        //        type = "text",
                                        //        text = button.text
                                        //    }
                                        //};
                                        //            cardComponent.components.Add(buttonComponent);
                                        //            buttonIndex++;
                                        //        }
                                        //        break;

                                        }
                                    }
                                }

                                cardIndex++;
                                carousel.cards.Add(cardComponent);
                            }


                        }
                        postMessageTemplateModel.template.components.Add(carousel);
                    }
                    else
                    {

                        foreach (var item in objWhatsAppTemplateModel.components)
                        {
                            if (objWhatsAppTemplateModel.category == "AUTHENTICATION")
                            {
                                if (item.type.Equals("BODY", StringComparison.OrdinalIgnoreCase))
                                {
                                    var componentBody = new Component
                                    {
                                        type = "BODY",
                                        parameters = GenerateTextParameters(
                                            contact.templateVariables?.VarOne
                                        )
                                    };

                                    if (componentBody.parameters.Count > 0)
                                    {
                                        postMessageTemplateModel.template.components.Add(componentBody);
                                        if (contact.FirstButtonURLVariabllesTemplate == null)
                                        {
                                            contact.FirstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate2(); 
                                        }

                                        contact.FirstButtonURLVariabllesTemplate.VarOne = contact.templateVariables?.VarOne;
                                    }

                                }
                                if (item.type.Equals("BUTTONS", StringComparison.OrdinalIgnoreCase))
                                {
                                    int buttonIndex = 0;

                                    if (!string.IsNullOrWhiteSpace(contact.FirstButtonURLVariabllesTemplate?.VarOne))
                                    {
                                        var componentButton1 = new Component
                                        {
                                            type = "BUTTON",
                                            sub_type = "url",
                                            index = buttonIndex,
                                            parameters = new List<Parameter>
                                    {
                                        new Parameter
                                        {
                                            type = "text",
                                            text = contact.templateVariables?.VarOne
                                        }
                                    }
                                        };
                                        postMessageTemplateModel.template.components.Add(componentButton1);
                                        buttonIndex++;
                                    }

                                    if (!string.IsNullOrWhiteSpace(contact.SecondButtonURLVariabllesTemplate?.VarOne))
                                    {
                                        var componentButton2 = new Component
                                        {
                                            type = "BUTTON",
                                            sub_type = "url",
                                            index = buttonIndex,
                                            parameters = new List<Parameter>
                                         {
                                        new Parameter
                                        {
                                            type = "text",
                                            text = contact.SecondButtonURLVariabllesTemplate?.VarOne
                                        }
                                    }


                                        };
                                        postMessageTemplateModel.template.components.Add(componentButton2);
                                    }


                                    if (!string.IsNullOrWhiteSpace(contact.buttonCopyCodeVariabllesTemplate?.VarOne))
                                    {
                                        var componentButton1 = new Component
                                        {
                                            type = "BUTTON",
                                            sub_type = "copy_code",
                                            index = buttonIndex,
                                            parameters = new List<Parameter>
                                    {
                                        new Parameter
                                        {
                                            type = "COUPON_CODE",
                                            coupon_code = contact.buttonCopyCodeVariabllesTemplate?.VarOne
                                        }
                                    }
                                        };
                                        postMessageTemplateModel.template.components.Add(componentButton1);
                                        buttonIndex++;
                                    }

                                }
                            }
                          
                            else
                            {

                                if (item.type.Equals("HEADER", StringComparison.OrdinalIgnoreCase))
                                {
                                    var componentHeader = new Component
                                    {
                                        type = "HEADER",
                                        parameters = new List<Parameter>()
                                    };

                                    var parameterHeader = new Parameter
                                    {
                                        type = item.format.ToLower()
                                    };

                                    if (item.format == "TEXT")
                                    {
                                        var headerText = headerVariablesTemplates?.VarOne ?? contact.headerVariabllesTemplate?.VarOne;
                                        if (!string.IsNullOrWhiteSpace(headerText))
                                        {
                                            parameterHeader.type = "text";
                                            parameterHeader.text = headerText;
                                            componentHeader.parameters.Add(parameterHeader);
                                        }
                                    }
                                    else if (item.format == "IMAGE")
                                    {
                                        parameterHeader.image = new ImageTemplate { link = objWhatsAppTemplateModel.mediaLink };
                                        componentHeader.parameters.Add(parameterHeader);
                                    }
                                    else if (item.format == "VIDEO")
                                    {
                                        parameterHeader.video = new VideoTemplate { link = objWhatsAppTemplateModel.mediaLink };
                                        componentHeader.parameters.Add(parameterHeader);
                                    }
                                    else if (item.format == "DOCUMENT")
                                    {
                                        var document = new DocumentTemplate();
                                        string mediaLink = objWhatsAppTemplateModel.mediaLink?.Trim(); 

                                        if (string.IsNullOrEmpty(mediaLink))
                                        {
                                            throw new ArgumentException("Media link cannot be empty.");
                                        }

                                        if (mediaLink.Contains(","))
                                        {
                                            string[] parts = mediaLink.Split(',', 2);
                                            document.filename = parts[0].Trim();
                                            document.link = parts[1].Trim();
                                        }
                                        else
                                        {
                                            document.link = mediaLink;

                                            try
                                            {
                                                Uri uri = new Uri(mediaLink);
                                                string fileNameFromUrl = Path.GetFileName(uri.LocalPath);

                                                document.filename = !string.IsNullOrEmpty(fileNameFromUrl)
                                                    ? fileNameFromUrl
                                                    : $"document_{DateTime.Now:yyyyMMddHHmmss}";
                                            }
                                            catch (UriFormatException)
                                            {
                                                document.filename = $"document_{DateTime.Now:yyyyMMddHHmmss}";
                                            }
                                        }

                                        parameterHeader.document = document;
                                        componentHeader.parameters.Add(parameterHeader);
                                    }
                                    //else if (item.format == "DOCUMENT")
                                    //{
                                    //    var document = new DocumentTemplate();
                                    //    var mediaLink = objWhatsAppTemplateModel.mediaLink;

                                    //    if (mediaLink.Contains(","))
                                    //    {
                                    //        var parts = mediaLink.Split(',');
                                    //        document.filename = parts[0];
                                    //        document.link = parts.Length > 1 ? parts[1] : parts[0];
                                    //    }
                                    //    else
                                    //    {
                                    //        var uri = new Uri(mediaLink);
                                    //        var fileName = Path.GetFileName(uri.LocalPath);
                                    //        document.filename = fileName.Length < 20 ? fileName : "fileName";
                                    //        document.link = mediaLink;
                                    //    }

                                    //    parameterHeader.document = document;
                                    //    componentHeader.parameters.Add(parameterHeader);
                                    //}

                                    postMessageTemplateModel.template.components.Add(componentHeader);
                                }
                                else if (item.type.Equals("BODY", StringComparison.OrdinalIgnoreCase))
                                {
                                    var componentBody = new Component
                                    {
                                        type = "BODY",
                                        parameters = GenerateTextParameters(
                                             contact.templateVariables?.VarOne,
                                             contact.templateVariables?.VarTwo,
                                             contact.templateVariables?.VarThree,
                                             contact.templateVariables?.VarFour,
                                             contact.templateVariables?.VarFive,
                                             contact.templateVariables?.VarSix,
                                             contact.templateVariables?.VarSeven,
                                             contact.templateVariables?.VarEight,
                                             contact.templateVariables?.VarNine,
                                            contact.templateVariables?.VarTen,
                                            contact.templateVariables?.VarEleven,
                                             contact.templateVariables?.VarTwelve,
                                             contact.templateVariables?.VarThirteen,
                                            contact.templateVariables?.VarFourteen,
                                             contact.templateVariables?.VarFifteen
                                        )
                                    };

                                    if (componentBody.parameters.Count > 0)
                                    {
                                        postMessageTemplateModel.template.components.Add(componentBody);
                                    }
                                }
                                else if (item.type.Equals("BUTTONS", StringComparison.OrdinalIgnoreCase))
                                {
                                    int buttonIndex = -1;
                                    var buttons = objWhatsAppTemplateModel.components.FirstOrDefault(c => c.type == "BUTTONS");
                                    

                                    if (item.buttons != null)
                                    {
                                        var checkButtonURL1 = true;
                                        var checkButtonURL2 = true;
                                        var checkButtonCopyCode = true;
                                        foreach (var button in item.buttons)
                                        {
                                            buttonIndex++;
                                            if (button.type == "QUICK_REPLY" && !string.IsNullOrWhiteSpace(button.text))
                                            {
                                                postMessageTemplateModel.template.components.Add(new Component
                                                {
                                                    type = "BUTTON",
                                                    sub_type = "quick_reply",
                                                    index = buttonIndex,
                                                    parameters = new List<Parameter>
                                                        {
                                                            new Parameter
                                                            {
                                                                type = "payload",
                                                                payload = button.text.ToLower().Replace(" ", "-")
                                                            }
                                                        }
                                                });
                                                continue;
                                                
                                            }

                                            if (checkButtonCopyCode&& button.type == "COPY_CODE" && !string.IsNullOrWhiteSpace(contact.buttonCopyCodeVariabllesTemplate?.VarOne))
                                            {

                                                postMessageTemplateModel.template.components.Add(new Component
                                                {
                                                    type = "BUTTON",
                                                    sub_type = "copy_code",
                                                    index = buttonIndex,
                                                    parameters = new List<Parameter>
                                                {
                                                    new Parameter
                                                    {
                                                        type = "COUPON_CODE",
                                                        coupon_code = contact.buttonCopyCodeVariabllesTemplate?.VarOne
                                                    }
                                                }
                                                });
                                                checkButtonCopyCode=false;
                                                continue;
                                                
                                            }
                                            if (checkButtonURL1&& button.type == "URL" && button.example != null&&  !string.IsNullOrWhiteSpace(contact.FirstButtonURLVariabllesTemplate?.VarOne))
                                            {
                                                postMessageTemplateModel.template.components.Add(new Component
                                                {
                                                    type = "BUTTON",
                                                    sub_type = "url",
                                                    index = buttonIndex,
                                                    parameters = new List<Parameter>
                                            {
                                                new Parameter
                                                {
                                                    type = "text",
                                                    text = contact.FirstButtonURLVariabllesTemplate?.VarOne
                                                }
                                            }
                                                });
                                                checkButtonURL1 = false;
                                                continue;
                                            }

                                            if (checkButtonURL2 && button.type == "URL" && button.example != null  &&  !string.IsNullOrWhiteSpace(contact.SecondButtonURLVariabllesTemplate?.VarOne))
                                            {
                                                postMessageTemplateModel.template.components.Add(new Component
                                                {
                                                    type = "BUTTON",
                                                    sub_type = "url",
                                                    index = buttonIndex,
                                                    parameters = new List<Parameter>
                                             {
                                                 new Parameter
                                                 {
                                                     type = "text",
                                                     text = contact.SecondButtonURLVariabllesTemplate?.VarOne
                                                 }
                                             }
                                                });
                                                checkButtonURL2 = false; continue;
                                                
                                            }
                                        }
                                    }

                       
                                }
                            }

                        }
                    }

                }

                return postMessageTemplateModel;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to prepare message template", ex);
            }
        }

        //private static PostMessageTemplateModel PrepareMessageTemplate(
        //    MessageTemplateModel objWhatsAppTemplateModel,
        //    ListContactCampaign contact,
        //    bool isExternal,
        //    TemplateVariablles2 templateVariables,
        //    HeaderVariablesTemplate2 headerVariablesTemplates,
        //    FirstButtonURLVariabllesTemplate2 firstButtonUrlVariables,
        //    SecondButtonURLVariabllesTemplate2 secondButtonUrlVariables)
        //{
        //    try
        //    {
        //        var postMessageTemplateModel = new PostMessageTemplateModel
        //        {
        //            messaging_product = "whatsapp",
        //            to = contact.PhoneNumber,
        //            type = "template",
        //            template = new WhatsAppTemplateModel
        //            {
        //                name = objWhatsAppTemplateModel.name,
        //                language = new WhatsAppLanguageModel { code = objWhatsAppTemplateModel.language },
        //                components = new List<Component>()
        //            }
        //        };

        //        if (objWhatsAppTemplateModel.components != null)
        //        {
        //            var carouselComponent = objWhatsAppTemplateModel.components.FirstOrDefault(c => c.type == "carousel");
        //            var bodyComponent1 = objWhatsAppTemplateModel.components.FirstOrDefault(c => c.type == "BODY");
        //            if (bodyComponent1!= null)
        //            {

        //            }

        //            if (carouselComponent != null && contact.carouselVariabllesTemplate != null)
        //            {
        //                var carousel = new Component
        //                {
        //                    type = "CAROUSEL",
        //                    cards = new List<CardComponent>()
        //                };

        //                foreach (var contactCard in contact.carouselVariabllesTemplate.cards)
        //                {
        //                    var templateCard = carouselComponent.cards[contactCard.CardIndex];
        //                    var cardComponent = new CardComponent
        //                    {
        //                        card_index = contactCard.CardIndex,
        //                        components = new List<Component>()
        //                    };

        //                    var headerComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("HEADER", StringComparison.OrdinalIgnoreCase));
        //                    if (headerComponent != null)
        //                    {
        //                        var componentHeader = new Component
        //                        {
        //                            type = "HEADER",
        //                            parameters = new List<Parameter>()
        //                        };

        //                        var parameterHeader = new Parameter
        //                        {
        //                            type = headerComponent.format.ToLower()
        //                        };

        //                        if (headerComponent.format == "TEXT")
        //                        {
        //                            var headerText = contactCard.Variables.VarOne;
        //                            if (!string.IsNullOrWhiteSpace(headerText))
        //                            {
        //                                parameterHeader.type = "text";
        //                                parameterHeader.text = headerText;
        //                                componentHeader.parameters.Add(parameterHeader);
        //                                cardComponent.components.Add(componentHeader);
        //                            }
        //                        }
        //                        else if (headerComponent.format == "IMAGE")
        //                        {
        //                            var mediaID = headerComponent.example?.mediaID;
        //                            if (!string.IsNullOrEmpty(mediaID))
        //                            {
        //                                parameterHeader.type = "image";
        //                                parameterHeader.image = new ImageTemplate { id = mediaID };
        //                                componentHeader.parameters.Add(parameterHeader);
        //                                cardComponent.components.Add(componentHeader);
        //                            }
        //                            else if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
        //                            {
        //                                parameterHeader.type = "image";
        //                                parameterHeader.image = new ImageTemplate { link = objWhatsAppTemplateModel.mediaLink };
        //                                componentHeader.parameters.Add(parameterHeader);
        //                                cardComponent.components.Add(componentHeader);
        //                            }
        //                        }
        //                        else if (headerComponent.format == "VIDEO")
        //                        {
        //                            var mediaID = headerComponent.example?.mediaID;
        //                            if (!string.IsNullOrEmpty(mediaID))
        //                            {
        //                                parameterHeader.type = "video";
        //                                parameterHeader.video = new VideoTemplate { id = mediaID };
        //                                componentHeader.parameters.Add(parameterHeader);
        //                                cardComponent.components.Add(componentHeader);
        //                            }
        //                        }
        //                    }

        //                    var bodyComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("BODY", StringComparison.OrdinalIgnoreCase));
        //                    if (bodyComponent != null)
        //                    {
        //                        var componentBody = new Component
        //                        {
        //                            type = "BODY",
        //                            parameters = GenerateTextParameters(
        //                                contactCard.Variables.VarOne,
        //                                contactCard.Variables.VarTwo,
        //                                contactCard.Variables.VarThree,
        //                                contactCard.Variables.VarFour,
        //                                contactCard.Variables.VarFive,
        //                                contactCard.Variables.VarSix,
        //                                contactCard.Variables.VarSeven,
        //                                contactCard.Variables.VarEight,
        //                                contactCard.Variables.VarNine,
        //                                contactCard.Variables.VarTen,
        //                                contactCard.Variables.VarEleven,
        //                                contactCard.Variables.VarTwelve,
        //                                contactCard.Variables.VarThirteen,
        //                                contactCard.Variables.VarFourteen,
        //                                contactCard.Variables.VarFifteen
        //                            )
        //                        };

        //                        if (componentBody.parameters.Count > 0)
        //                        {
        //                            cardComponent.components.Add(componentBody);
        //                        }
        //                    }

        //                    var buttonsComponent = templateCard.components.FirstOrDefault(c => c.type.Equals("BUTTONS", StringComparison.OrdinalIgnoreCase));
        //                    if (buttonsComponent != null && buttonsComponent.buttons != null)
        //                    {
        //                        int buttonIndex = 0;

        //                        foreach (var button in buttonsComponent.buttons)
        //                        {
        //                            var buttonComponent = new Component();

        //                            switch (button.type)
        //                            {
        //                                case "QUICK_REPLY":
        //                                    buttonComponent.type = "BUTTON";
        //                                    buttonComponent.sub_type = "quick_reply";
        //                                    buttonComponent.index = buttonIndex;
        //                                    buttonComponent.parameters = new List<Parameter>
        //                            {
        //                                new Parameter {
        //                                    type = "payload",
        //                                    payload = button.text?.ToLower().Replace(" ", "-") ?? "default-payload"
        //                                }
        //                            };
        //                                    cardComponent.components.Add(buttonComponent);
        //                                    buttonIndex++;
        //                                    break;

        //                                case "URL":
        //                                    var urlText = contactCard.firstButtonURLVariabllesTemplate?.VarOne ?? button.text;
        //                                    if (!string.IsNullOrEmpty(urlText))
        //                                    {
        //                                        buttonComponent.type = "BUTTON";
        //                                        buttonComponent.sub_type = "url";
        //                                        buttonComponent.index = buttonIndex;
        //                                        buttonComponent.parameters = new List<Parameter>
        //                                {
        //                                    new Parameter { type = "text", text = urlText }
        //                                };
        //                                        cardComponent.components.Add(buttonComponent);
        //                                        buttonIndex++;
        //                                    }
        //                                    break;

        //                                case "PHONE_NUMBER":
        //                                    if (!string.IsNullOrEmpty(button.phone_number))
        //                                    {
        //                                        buttonComponent.type = "BUTTON";
        //                                        buttonComponent.sub_type = "call";
        //                                        buttonComponent.index = buttonIndex;
        //                                        buttonComponent.parameters = new List<Parameter>
        //                                {
        //                                    new Parameter {
        //                                        type = "text",
        //                                        text = button.phone_number
        //                                    }
        //                                };
        //                                        cardComponent.components.Add(buttonComponent);
        //                                        buttonIndex++;
        //                                    }
        //                                    break;
        //                            }
        //                        }
        //                    }

        //                    carousel.cards.Add(cardComponent);
        //                }

        //                postMessageTemplateModel.template.components.Add(carousel);
        //            }
        //            else
        //            {
        //                // Handle non-carousel templates
        //                foreach (var item in objWhatsAppTemplateModel.components)
        //                {
        //                    if (objWhatsAppTemplateModel.category == "AUTHENTICATION")
        //                    {
        //                        if (item.type.Equals("BUTTONS", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            int buttonIndex = 0;

        //                            if (!string.IsNullOrWhiteSpace(contact.FirstButtonURLVariabllesTemplate?.VarOne))
        //                            {
        //                                var componentButton1 = new Component
        //                                {
        //                                    type = "BUTTONS",
        //                                    sub_type = "url",
        //                                    index = buttonIndex,
        //                                    parameters = new List<Parameter>
        //                            {
        //                                new Parameter
        //                                {
        //                                    type = "text",
        //                                    text = contact.FirstButtonURLVariabllesTemplate?.VarOne
        //                                }
        //                            }
        //                                };
        //                                postMessageTemplateModel.template.components.Add(componentButton1);
        //                                buttonIndex++;
        //                            }

        //                            if (!string.IsNullOrWhiteSpace(contact.SecondButtonURLVariabllesTemplate?.VarOne))
        //                            {
        //                                var componentButton2 = new Component
        //                                {
        //                                    type = "BUTTONS",
        //                                    sub_type = "url",
        //                                    index = buttonIndex,
        //                                    parameters = new List<Parameter>
        //                            {
        //                                new Parameter
        //                                {
        //                                    type = "text",
        //                                    text = contact.SecondButtonURLVariabllesTemplate?.VarOne
        //                                }
        //                            }
        //                                };
        //                                postMessageTemplateModel.template.components.Add(componentButton2);
        //                            }
        //                        }
        //                    }
        //                    else // Standard Templates
        //                    {
        //                        if (item.type.Equals("HEADER", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            var componentHeader = new Component
        //                            {
        //                                type = "HEADER",
        //                                parameters = new List<Parameter>()
        //                            };

        //                            var parameterHeader = new Parameter
        //                            {
        //                                type = item.format.ToLower()
        //                            };

        //                            if (item.format == "TEXT")
        //                            {
        //                                var headerText = headerVariablesTemplates?.VarOne ?? contact.headerVariabllesTemplate?.VarOne;
        //                                if (!string.IsNullOrWhiteSpace(headerText))
        //                                {
        //                                    parameterHeader.type = "text";
        //                                    parameterHeader.text = headerText;
        //                                    componentHeader.parameters.Add(parameterHeader);
        //                                }
        //                            }
        //                            else if (item.format == "IMAGE")
        //                            {
        //                                parameterHeader.image = new ImageTemplate { link = objWhatsAppTemplateModel.mediaLink };
        //                                componentHeader.parameters.Add(parameterHeader);
        //                            }
        //                            else if (item.format == "VIDEO")
        //                            {
        //                                parameterHeader.video = new VideoTemplate { link = objWhatsAppTemplateModel.mediaLink };
        //                                componentHeader.parameters.Add(parameterHeader);
        //                            }
        //                            else if (item.format == "DOCUMENT")
        //                            {
        //                                var document = new DocumentTemplate();
        //                                var mediaLink = objWhatsAppTemplateModel.mediaLink;

        //                                if (mediaLink.Contains(","))
        //                                {
        //                                    var parts = mediaLink.Split(',');
        //                                    document.filename = parts[0];
        //                                    document.link = parts.Length > 1 ? parts[1] : parts[0];
        //                                }
        //                                else
        //                                {
        //                                    var uri = new Uri(mediaLink);
        //                                    var fileName = Path.GetFileName(uri.LocalPath);
        //                                    document.filename = fileName.Length < 20 ? fileName : "fileName";
        //                                    document.link = mediaLink;
        //                                }

        //                                parameterHeader.document = document;
        //                                componentHeader.parameters.Add(parameterHeader);
        //                            }

        //                            postMessageTemplateModel.template.components.Add(componentHeader);
        //                        }
        //                        else if (item.type.Equals("BODY", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            var componentBody = new Component
        //                            {
        //                                type = "BODY",
        //                                parameters = GenerateTextParameters(
        //                                    templateVariables?.VarOne ?? contact.templateVariables?.VarOne,
        //                                    templateVariables?.VarTwo ?? contact.templateVariables?.VarTwo,
        //                                    templateVariables?.VarThree ?? contact.templateVariables?.VarThree,
        //                                    templateVariables?.VarFour ?? contact.templateVariables?.VarFour,
        //                                    templateVariables?.VarFive ?? contact.templateVariables?.VarFive,
        //                                    templateVariables?.VarSix ?? contact.templateVariables?.VarSix,
        //                                    templateVariables?.VarSeven ?? contact.templateVariables?.VarSeven,
        //                                    templateVariables?.VarEight ?? contact.templateVariables?.VarEight,
        //                                    templateVariables?.VarNine ?? contact.templateVariables?.VarNine,
        //                                    templateVariables?.VarTen ?? contact.templateVariables?.VarTen,
        //                                    templateVariables?.VarEleven ?? contact.templateVariables?.VarEleven,
        //                                    templateVariables?.VarTwelve ?? contact.templateVariables?.VarTwelve,
        //                                    templateVariables?.VarThirteen ?? contact.templateVariables?.VarThirteen,
        //                                    templateVariables?.VarFourteen ?? contact.templateVariables?.VarFourteen,
        //                                    templateVariables?.VarFifteen ?? contact.templateVariables?.VarFifteen
        //                                )
        //                            };

        //                            if (componentBody.parameters.Count > 0)
        //                            {
        //                                postMessageTemplateModel.template.components.Add(componentBody);
        //                            }
        //                        }
        //                        else if (item.type.Equals("BUTTONS", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            int buttonIndex = 0;

        //                            if (!string.IsNullOrWhiteSpace(contact.FirstButtonURLVariabllesTemplate?.VarOne))
        //                            {
        //                                var componentButton1 = new Component
        //                                {
        //                                    type = "BUTTONS",
        //                                    sub_type = "url",
        //                                    index = buttonIndex,
        //                                    parameters = new List<Parameter>
        //                            {
        //                                new Parameter
        //                                {
        //                                    type = "text",
        //                                    text = contact.FirstButtonURLVariabllesTemplate?.VarOne
        //                                }
        //                            }
        //                                };
        //                                postMessageTemplateModel.template.components.Add(componentButton1);
        //                                buttonIndex++;
        //                            }

        //                            if (!string.IsNullOrWhiteSpace(contact.SecondButtonURLVariabllesTemplate?.VarOne))
        //                            {
        //                                var componentButton2 = new Component
        //                                {
        //                                    type = "BUTTONS",
        //                                    sub_type = "url",
        //                                    index = buttonIndex,
        //                                    parameters = new List<Parameter>
        //                            {
        //                                new Parameter
        //                                {
        //                                    type = "text",
        //                                    text = contact.SecondButtonURLVariabllesTemplate?.VarOne
        //                                }
        //                            }
        //                                };
        //                                postMessageTemplateModel.template.components.Add(componentButton2);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        return postMessageTemplateModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException("Failed to prepare message template", ex);
        //    }
        //}

     
        private static List<Parameter> GenerateTextParameters(params string[] values)
        {
            var parameters = new List<Parameter>();
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    parameters.Add(new Parameter { type = "TEXT", text = value });
                }
            }
            return parameters;
        }
      
        private static SendCampaignNow MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                SendCampaignNow model = new SendCampaignNow
                {
                    rowId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    campaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
                    templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
                    IsExternal = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                    CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId"),
                    JopName = SqlDataHelper.GetValue<string>(dataReader, "JopName"),
                    campaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                    templateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent")
                };

                try
                {

                    model.model=System.Text.Json.JsonSerializer.Deserialize<MessageTemplateModel>(SqlDataHelper.GetValue<string>(dataReader, "TemplateJson"));
                }
                catch
                {


                }
                try
                {

                    model.templateVariablles=System.Text.Json.JsonSerializer.Deserialize<TemplateVariablles2>(SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables"));
                }
                catch
                {
                    model.templateVariablles=new TemplateVariablles2(); 

                }

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactCampaign>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new SendCampaignNow();
            }
        }




        private static void updateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel)
        {
            try
            {
                var SP_Name = "[dbo].[UpdateCampaignStatus]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",whatsAppCampaignModel.id)
                    ,new System.Data.SqlClient.SqlParameter("@Status",whatsAppCampaignModel.status)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), SettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

}


