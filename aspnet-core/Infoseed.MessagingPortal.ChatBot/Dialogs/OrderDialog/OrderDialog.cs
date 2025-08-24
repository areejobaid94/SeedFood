using Infoseed.MessagingPortal.ChatBot.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ChatBot.Dialogs.OrderDialog
{

    public class OrderDialog : ComponentDialog
    {
        private static IConfiguration Configuration;
        public OrderDialog(IConfiguration configuration)
            : base(nameof(OrderDialog))
        {
            Configuration = configuration;
            string[] paths = { ".", "Dialogs", "OrderDialog", "OrderDialog.lg" };
            string fullPath = Path.Combine(paths);
            var MenuDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // Add a generator. This is how all Language Generation constructs specified for this dialog are resolved.
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                // Create a LUIS recognizer.
              //  Recognizer = RecognizerManager.CreateRecognizer(configuration),
                Triggers = new List<OnCondition>()
                {
                 new OnBeginDialog()
                    {

                        Actions = new List<Dialog>()
                        {
                              new SetProperty()
                            {
                                 Property="conversation.itemTitle",
                                 Value="=turn.recognized.score",
                            },
                            // Take todo title if we already have it from root dialog's LUIS model.
                            // There is one LUIS application for this bot. So any entity captured by the rootDialog
                            // will be automatically available to child dialog.
                            // @EntityName is a short-hand for turn.recognized.entities.<EntityName>. Other useful short-hands are 
                            //     #IntentName is a short-hand for turn.intents.<IntentName>
                            //     $PropertyName is a short-hand for dialog.<PropertyName>
                            //new SetProperties()
                            //{
                            //    Assignments = new List<PropertyAssignment>()
                            //    {
                            //        new PropertyAssignment()
                            //        {
                            //            Property = "dialog.itemTitle",
                            //            Value = "=@itemTitle"
                            //        },
                            //        new PropertyAssignment()
                            //        {
                            //            Property = "dialog.listType",
                            //            Value = "=@listType"
                            //        }
                            //    }
                            //},
                            new SendActivity("${Test()}"),
                            new SendActivity("How May I help you?"),
                            // TextInput by default will skip the prompt if the property has value.
                           new TextInput()
                            {
                                Property = "dialog.service",
                                Prompt = new ActivityTemplate("1-Order or 2-Order Inquiry"),
                                // This entity is coming from the local AddToDoDialog's own LUIS recognizer.
                                // This dialog's .lu file is under ./AddToDoDialog.lu
                                Value = "=@service",
                                // Allow interruption if we do not have an item title and have a super high confidence classification of an intent.
                                //AllowInterruptions = "!@itemTitle && turn.recognized.score >= 0.7"
                            },
                           new SendActivity("${Test()}")
                            // Get list type
                            //new TextInput()
                            //{
                            //    Property = "dialog.listType",
                            //    Prompt = new ActivityTemplate("${GetListType()}"),
                            //    Value = "=@listType",
                            //    AllowInterruptions = "!@listType && turn.recognized.score >= 0.7",
                            //    Validations = new List<BoolExpression>()
                            //    {
                            //        // Verify using expressions that the value is one of todo or shopping or grocery
                            //        "contains(createArray('todo', 'shopping', 'grocery'), toLower(this.value))",
                            //    },
                            //    OutputFormat = "=toLower(this.value)",
                            //    InvalidPrompt = new ActivityTemplate("${GetListType.Invalid()}"),
                            //    MaxTurnCount = 2,
                            //    DefaultValue = "todo",
                            //    DefaultValueResponse = new ActivityTemplate("${GetListType.DefaultValueResponse()}")
                            //},

                            //new SendActivity("${AddItemReadBack()}")
                            // All child dialogs will automatically end if there are no additional steps to execute. 
                            // If you wish for a child dialog to not end automatically, you can set 
                            // AutoEndDialog property on the Adaptive Dialog to 'false'
                        }
                    }
                }
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(MenuDialog);
            InitialDialogId = nameof(AdaptiveDialog);

        }
    }
}
