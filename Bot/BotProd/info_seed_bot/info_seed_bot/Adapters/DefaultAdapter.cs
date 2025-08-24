using info_seed_bot.Middlewares;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace info_seed_bot.Adapters
{
    public class DefaultAdapter : BotFrameworkHttpAdapter
    {
        public DefaultAdapter(CheckLanguageMiddleware  checkLanguageMiddleware,DataRepresentationMiddleware dataRepresentationMiddleware)
             
        {
            //OnTurnError = async (turnContext, exception) =>
            //{
            //    await turnContext.SendActivityAsync(new Activity(type: ActivityTypes.Message, text: "Sorry, something went wrong"));
            //    await turnContext.SendActivityAsync(templateEngine.GenerateActivityForLocale("ErrorMessage"));
            //    telemetryClient.TrackException(exception);
            //};
           // Use(new CheckLanguageMiddleware());
            //Use(new DataRepresentationMiddleware());

        }
    }
}