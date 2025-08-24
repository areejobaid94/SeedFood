using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Infoseed.MessagingPortal.SocketIOClient
{

    public static class SocketIOManager
    {
        public static SocketIO client;
        public static void Init()
        {
            // var uri = new Uri("http://localhost:3009/");

            //var uri = new Uri("https://infoseedsocketioserverstg.azurewebsites.net/");
           // string token = "313cb6e6-caad-4457-a0e4-669415d93250"; // stg

            //var uri = new Uri("https://infoseedsocketioserver-prod.azurewebsites.net/");
            //string token = "2bb33e3ae845db0b32dd1c5efdd9f35c";


          //  var uri = new Uri(AppSettingsCoreModel.ConnectionStrings);
           // string token = "313cb6e6-caad-4457-a0e4-669415d93250"; // stg
            // string token = "2bb33e3ae845db0b32dd1c5efdd9f35c";   // prod
            // string token = "313cb6e6-caad-4457-a0e4-669415d93250"; // stg
            //string token = "313cb6e6-caad-4457-a0e4-669415d93250";

            client = new SocketIO(AppSettingsCoreModel.SocketIOURL, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                     {"token",AppSettingsCoreModel.SocketIOToken}
                },
            });

            client.OnConnected += Socket_OnConnected;
            client.OnReconnecting += Socket_OnReconnecting;
            client.OnDisconnected += Socket_OnDisconnected;
            client.OnError += Socket_OnError;
            client.ConnectAsync().Wait();
        }

        private static async void Socket_OnConnected(object sender, EventArgs e)
        {
            //  await client.EmitAsync("connected", client.Id, SocketUserTypesEnum.WebAPI);
        }

        private static void Socket_OnReconnecting(object sender, int e)
        {
            // client.EmitAsync("connected", client.Id, SocketUserTypesEnum.WebAPI).Wait();
        }

        private static void Socket_OnDisconnected(object sender, string e)
        {
            // Console.WriteLine("disconnect: " + e);
        }






        public static void SendOrder(GetOrderForViewDto model, int tenantId)
        {
            client.EmitAsync("order-get", TocamelCase(model), tenantId).Wait();
            //client.EmitAsync("selling-request", SelingRequestModel, 10);
            //await socket.EmitAsync("order-get", SelingRequestModel1, 10);
            //await socket.EmitAsync("live-chat-get", SelingRequestModel2, 10);
            //await socket.EmitAsync("contact-get", SelingRequestModel3, 10);
            //await socket.EmitAsync("chat-get", SelingRequestModel4, 10);
        }


        public static void SendContact(object model, int tenantId)
        {

            client.EmitAsync("contact-get", TocamelCase(model), tenantId).Wait();
        }

        public static async void SendChat(object model, int tenantId)
        {
            await client.EmitAsync("chat-get", TocamelCase(model), tenantId);
        }
        public static async void SendLiveChat(object model, int tenantId)
        {
            await client.EmitAsync("live-chat-get", TocamelCase(model), tenantId);
        }
        public static void SendSellingRequest(object model, int tenantId)
        {
            client.EmitAsync("selling-request-get", TocamelCase(model), tenantId).Wait();
        }   
        
        public static void SendEvaluation(object model, int tenantId)
        {
            client.EmitAsync("evaluation-get", TocamelCase(model), tenantId).Wait();
        }

        public static void SendBooking(object model, int tenantId)
        {
            client.EmitAsync("booking-get", TocamelCase(model), tenantId).Wait();
        }

        public static void SendInvoices(object model, int tenantId)
        {
            client.EmitAsync("invoices-get", TocamelCase(model), tenantId).Wait();
        }
        public static void SendAccount(AccountLoginModel model, int tenantId)
        {
            client.EmitAsync("account-get", TocamelCase(model), tenantId).Wait();
        }

        private static void Socket_OnError(object sender, string e)
        {

        }

        private static object TocamelCase(object model)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            string modelString = JsonSerializer.Serialize<object>(model, options);
            object newmodel = JsonSerializer.Deserialize<object>(modelString);
            return newmodel;
        }

    }
}
