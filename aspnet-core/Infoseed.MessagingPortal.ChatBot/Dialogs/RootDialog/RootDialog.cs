using Infoseed.MessagingPortal.ChatBot.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Infoseed.MessagingPortal.ChatBot.Dialogs.RootDialog
{
    public class RootDialog : ComponentDialog
    {
        private static IConfiguration Configuration;
        public RootDialog(IConfiguration configuration)
         : base(nameof(RootDialog))
        {
            try
            {

          
            Configuration = configuration;
            string[] paths = { ".", "Dialogs", "RootDialog", "RootDialog.lg" };
            string fullPath = Path.Combine(paths);
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // Add a generator. This is how all Language Generation constructs specified for this dialog are resolved.
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                // Create a LUIS recognizer.
                // The recognizer is built using the intents, utterances, patterns and entities defined in ./RootDialog.lu file
              //  Recognizer = RecognizerManager.CreateRecognizer(configuration),

                Triggers = new List<OnCondition>()
                {
                     //new OnBeginDialog()
                     //{
                     //    Actions = new List<Dialog>()
                     //    {
                     //        new SetProperty()
                     //       {
                     //            Property="conversation.itemTitle",
                     //            Value="'turn.recognized.score'",
                     //       },
                     //    }
                     //},
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps()
                    },
                    // Intent rules for the LUIS model. Each intent here corresponds to an intent defined in ./RootDialog.lu file
                    new OnIntent("Greeting")
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${HelpRootDialog()}")
                            }
                    },
                    new OnIntent("Order")
                    { 
                        // LUIS returns a confidence score with intent classification. 
                        // Conditions are expressions. 
                        // This expression ensures that this trigger only fires if the confidence score for the 
                        // AddToDoDialog intent classification is at least 0.7
                        //Condition = "#AddItem.Score >= 0.5",
                        Actions = new List<Dialog>()
                        {
                            new BeginDialog(nameof(OrderDialog))
                        }
                    },


                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // Add all child dialogS
            AddDialog(new OrderDialog.OrderDialog(configuration));

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private static List<Dialog> WelcomeUserSteps()
        {
            return new List<Dialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    
                    Actions = new List<Dialog>()
                    {
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${IntroMessage()}"),
                                // Initialize global properties for the user.
                                new SetProperty()
                                {
                                    Property = "user.lists",
                                    Value = "={todo : [], grocery : [], shopping : []}"
                                }
                            }
                        }
                    }
                }
            };
        }

    }
}
