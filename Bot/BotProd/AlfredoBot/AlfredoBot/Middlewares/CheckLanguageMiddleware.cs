using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace info_seed_bot.Middlewares
{
    public class CheckLanguageMiddleware : IMiddleware
    {
        private Dictionary<string, string> conversations = new Dictionary<string, string>();

        public CheckLanguageMiddleware()
        {
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                if (string.IsNullOrEmpty(turnContext.Activity.Text))
                {
                    turnContext.Activity.Text = ".";
                }

                var text = turnContext.Activity.Text;

                Regex emailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                Regex mobileRegex = new Regex(@"^(\+97[\s]{0,1}[\-]{0,1}[\s]{0,1}1|0)(50|55)[\s]{0,1}[\-]{0,1}[\s]{0,1}[1-9]{1}[0-9]{6}$");
                Regex mobile2Regex = new Regex(@"^(\+971|971|00971)[0-9]{9}$");
                Regex accountNumberRegex = new Regex(@"^(04|1.)[0-9]{8}$");
                Regex accountNumber2Regex = new Regex(@"^(05|04)[0-9]{8}$");
                Regex containsEnglishChars = new Regex("[a-zA-Z]");

                if (containsEnglishChars.IsMatch(text) && !emailRegex.IsMatch(text))
                {
                    var numbers = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("٠", "0"),
                        new KeyValuePair<string, string>("١", "1"),
                        new KeyValuePair<string, string>("٢", "2"),
                        new KeyValuePair<string, string>("٣", "3"),
                        new KeyValuePair<string, string>("٤", "4"),
                        new KeyValuePair<string, string>("٥", "5"),
                        new KeyValuePair<string, string>("٦", "6"),
                        new KeyValuePair<string, string>("٧", "7"),
                        new KeyValuePair<string, string>("٨", "8"),
                        new KeyValuePair<string, string>("٩", "9"),
                        new KeyValuePair<string, string>("١٠", "10"),
                        new KeyValuePair<string, string>("١١", "11"),
                        new KeyValuePair<string, string>("١٢", "12"),
                        new KeyValuePair<string, string>("١٣", "13"),
                        new KeyValuePair<string, string>("١٤", "14"),
                        new KeyValuePair<string, string>("١٥", "15"),
                        new KeyValuePair<string, string>("١٦", "16"),
                        new KeyValuePair<string, string>("١٧", "17"),
                        new KeyValuePair<string, string>("١٨", "18"),
                        new KeyValuePair<string, string>("١٩", "19"),
                        new KeyValuePair<string, string>("٢٠", "20"),
                        new KeyValuePair<string, string>("٢١", "21"),
                        new KeyValuePair<string, string>("٢٢", "22"),
                        new KeyValuePair<string, string>("٢٣", "23"),
                        new KeyValuePair<string, string>("٢٤", "24"),
                        new KeyValuePair<string, string>("٢٥", "25"),
                        new KeyValuePair<string, string>("٢٦", "26"),
                        new KeyValuePair<string, string>("٢٧", "27"),
                        new KeyValuePair<string, string>("٢٨", "28"),
                        new KeyValuePair<string, string>("٢٩", "29"),
                        new KeyValuePair<string, string>("٣٠", "30"),
                        new KeyValuePair<string, string>("٣١", "31"),
                        new KeyValuePair<string, string>("٣٢", "32"),
                        new KeyValuePair<string, string>("٣٣", "33"),
                        new KeyValuePair<string, string>("٣٤", "34"),
                        new KeyValuePair<string, string>("٣٥", "35"),
                        new KeyValuePair<string, string>("٣٦", "36"),
                        new KeyValuePair<string, string>("٣٧", "37"),
                        new KeyValuePair<string, string>("٣٨", "38"),
                        new KeyValuePair<string, string>("٣٩", "39"),
                        new KeyValuePair<string, string>("٤٠", "40"),
                        new KeyValuePair<string, string>("٤١", "41"),
                        new KeyValuePair<string, string>("٤٢", "42"),
                        new KeyValuePair<string, string>("٤٣", "43"),
                        new KeyValuePair<string, string>("٤٤", "44"),
                        new KeyValuePair<string, string>("٤٥", "45"),
                        new KeyValuePair<string, string>("٤٦", "46"),
                        new KeyValuePair<string, string>("٤٧", "47"),
                        new KeyValuePair<string, string>("٤٨", "48"),
                        new KeyValuePair<string, string>("٤٩", "49"),
                        new KeyValuePair<string, string>("٥٠", "50")
                    };

                    foreach (var item in numbers)
                    {
                        if (text.Contains(item.Key))
                        {
                            text = text.Replace(item.Key, item.Value);
                        }
                    }

                    // turnContext.Activity.Locale = "en-US";
                    turnContext.Activity.Text = text;
                    turnContext.Activity.Speak = text;
                }
                else if (IsArabic(text))
                {
                    var numbers = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("٠", "0"),
                        new KeyValuePair<string, string>("١", "1"),
                        new KeyValuePair<string, string>("٢", "2"),
                        new KeyValuePair<string, string>("٣", "3"),
                        new KeyValuePair<string, string>("٤", "4"),
                        new KeyValuePair<string, string>("٥", "5"),
                        new KeyValuePair<string, string>("٦", "6"),
                        new KeyValuePair<string, string>("٧", "7"),
                        new KeyValuePair<string, string>("٨", "8"),
                        new KeyValuePair<string, string>("٩", "9"),
                        new KeyValuePair<string, string>("١٠", "10"),
                        new KeyValuePair<string, string>("١١", "11"),
                        new KeyValuePair<string, string>("١٢", "12"),
                        new KeyValuePair<string, string>("١٣", "13"),
                        new KeyValuePair<string, string>("١٤", "14"),
                        new KeyValuePair<string, string>("١٥", "15"),
                        new KeyValuePair<string, string>("١٦", "16"),
                        new KeyValuePair<string, string>("١٧", "17"),
                        new KeyValuePair<string, string>("١٨", "18"),
                        new KeyValuePair<string, string>("١٩", "19"),
                        new KeyValuePair<string, string>("٢٠", "20"),
                        new KeyValuePair<string, string>("٢١", "21"),
                        new KeyValuePair<string, string>("٢٢", "22"),
                        new KeyValuePair<string, string>("٢٣", "23"),
                        new KeyValuePair<string, string>("٢٤", "24"),
                        new KeyValuePair<string, string>("٢٥", "25"),
                        new KeyValuePair<string, string>("٢٦", "26"),
                        new KeyValuePair<string, string>("٢٧", "27"),
                        new KeyValuePair<string, string>("٢٨", "28"),
                        new KeyValuePair<string, string>("٢٩", "29"),
                        new KeyValuePair<string, string>("٣٠", "30"),
                        new KeyValuePair<string, string>("٣١", "31"),
                        new KeyValuePair<string, string>("٣٢", "32"),
                        new KeyValuePair<string, string>("٣٣", "33"),
                        new KeyValuePair<string, string>("٣٤", "34"),
                        new KeyValuePair<string, string>("٣٥", "35"),
                        new KeyValuePair<string, string>("٣٦", "36"),
                        new KeyValuePair<string, string>("٣٧", "37"),
                        new KeyValuePair<string, string>("٣٨", "38"),
                        new KeyValuePair<string, string>("٣٩", "39"),
                        new KeyValuePair<string, string>("٤٠", "40"),
                        new KeyValuePair<string, string>("٤١", "41"),
                        new KeyValuePair<string, string>("٤٢", "42"),
                        new KeyValuePair<string, string>("٤٣", "43"),
                        new KeyValuePair<string, string>("٤٤", "44"),
                        new KeyValuePair<string, string>("٤٥", "45"),
                        new KeyValuePair<string, string>("٤٦", "46"),
                        new KeyValuePair<string, string>("٤٧", "47"),
                        new KeyValuePair<string, string>("٤٨", "48"),
                        new KeyValuePair<string, string>("٤٩", "49"),
                        new KeyValuePair<string, string>("٥٠", "50")
                    };

                    foreach (var item in numbers)
                    {
                        if (text.Contains(item.Key))
                        {
                            text = text.Replace(item.Key, item.Value);
                        }
                    }

                    //turnContext.Activity.Locale = "ar-JO";
                    turnContext.Activity.Text = text;
                    turnContext.Activity.Speak = text;
                }
                else if (conversations.TryGetValue(turnContext.Activity.Conversation.Id, out var lastLocale))
                {
                    turnContext.Activity.Locale = lastLocale;
                }

                conversations[turnContext.Activity.Conversation.Id] = turnContext.Activity.Locale;
            }

            await next(cancellationToken).ConfigureAwait(false);
        }

        public bool IsArabic(string text)
        {
            //Regex regex = new Regex(@"[\u0600-\u06FF\u0750-\u077F\u0660-\u0669]");
            Regex regex = new Regex(@"\p{IsArabic}");

            //Regex regex = new Regex("[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");
            Match match = regex.Match(text);
            return match.Success;
        }
    }
}
