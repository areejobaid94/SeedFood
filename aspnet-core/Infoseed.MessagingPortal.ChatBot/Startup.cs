// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.10.3

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;

using Infoseed.MessagingPortal.ChatBot.Bots;
using Infoseed.MessagingPortal.ChatBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Builder.BotFramework;
using Infoseed.MessagingPortal.ChatBot.Dialogs.RootDialog;

namespace Infoseed.MessagingPortal.ChatBot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //// Create the Bot Framework Adapter with error handling enabled.
            //services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            //// Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            //services.AddSingleton<IStorage, MemoryStorage>();

            //// Create the User state. (Used in this bot's Dialog implementation.)
            //services.AddSingleton<UserState>();

            //// Create the Conversation state. (Used by the Dialog system itself.)
            //services.AddSingleton<ConversationState>();

            //// Register LUIS recognizer
            //services.AddSingleton<FlightBookingRecognizer>();

            //// Register the BookingDialog.
            //services.AddSingleton<BookingDialog>();

            //// The MainDialog that will be run by the bot.
            //services.AddSingleton<MainDialog>();

            //// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();



            services.AddMvc();

            // Register dialog. This sets up memory paths for adaptive.
            ComponentRegistration.Add(new DialogsComponentRegistration());

            // Register adaptive component
            ComponentRegistration.Add(new AdaptiveComponentRegistration());

            // Register to use language generation.
            ComponentRegistration.Add(new LanguageGenerationComponentRegistration());

            // Register luis component
            ComponentRegistration.Add(new LuisComponentRegistration());

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter with error handling enabled. 
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

           

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.) 
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
           // services.AddSingleton<UserState>();

            var storage = new MemoryStorage();
            var userState = new UserState(storage);
            services.AddSingleton(userState);

            // Create the Conversation state passing in the storage layer.
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);
            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<RootDialog>();

            // Create the bot. the ASP Controller is expecting an IBot.
            services.AddSingleton<IBot, DialogBot<RootDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });


        }
    }
}
