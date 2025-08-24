using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace info_seed_bot.Extensions
{
    public static class TurnContextExtensions
    {
        public static bool IsWhatsappChannel(this ITurnContext turnContext)
        {
            return true; // turnContext.Activity.ChannelId == Enum.GetName(ChannelEnum.Whatspp.GetType(), ChannelEnum.Whatspp);
        }

        public static bool IsTelegramChannel(this ITurnContext turnContext)
        {
            return false; // turnContext.Activity.ChannelId == Enum.GetName(ChannelEnum.Telegram.GetType(), ChannelEnum.Telegram);
        }

        public static bool IsFacebookChannel(this ITurnContext turnContext)
        {
            return false; // turnContext.Activity.ChannelId.Trim().ToLower() == "facebook";
        }
    }
}
