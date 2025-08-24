using Infoseed.MessagingPortal.Web.Models.InfoSeedBotModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    public class InfoSeedBotController : MessagingPortalControllerBase
    {
        public InfoSeedBotController(  )
        {
 

        }
        [HttpPost("BotMessageHandler")]
        public string BotMessageHandler([FromBody] BotMessageActivityModel model, string Key)
        {
            var TempName = model.TempName;
            var Rez = "";
            switch (TempName)
            {
                case "Temp1":
                    Rez= Temp1(model.StepNumber);
                    break;
                case "Temp2":
                    break;
                case "Temp3":
                    break;
                case "Temp4":
                    break;
            }

            return Rez;
        }

        private string Temp1(string step)
        {
            var Rez = "";

            switch (step)
            {
                case "1":
                    Rez= step1();
                    break;
                case "2":
                    Rez=step2();
                    break;
                case "3":
                    Rez= step3();
                    break;
                case "4":
                    Rez= step4();
                    break;
            }

            return Rez;
        }
        private string step1()
        {
      
            return "الرجاء ارسال الموقع ";
        }

        private string step2()
        {

            return "https://infoseedordersystem-stg.azurewebsites.net/?TenantID=27&ContactId=40480&PhoneNumber=962781399319&Menu=0&LanguageBot=1&lang=ar";
        }
        private string step3()
        {

            return "هل تريد تاكيد";
        }

        private string step4()
        {

            return "شكرا لك سيتم التواصل معك قريبا";
        }
    }
}
