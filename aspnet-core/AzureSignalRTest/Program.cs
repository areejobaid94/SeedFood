using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;


namespace AzureSignalRTest
{
    class Program
    {
        private static readonly JwtSecurityTokenHandler _jwtTokenHandler = new JwtSecurityTokenHandler();

        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .AddEnvironmentVariables()
           .Build();
            var serviceUrl = "https://infoseedsignalr.service.signalr.net";
            var accessKey = "RDJR68dS9zt2CWFEoTQxdfy4PVzN4ukuEMPVmK9X4wA=";
            var hubName = "OrderHub_" + config.GetValue<string>("UserId");
           
            var url = $"{serviceUrl}/client/?hub={hubName}";

            HubConnection hubConnection = new HubConnectionBuilder()
                .WithUrl(url, option =>
                {
                    option.AccessTokenProvider = () =>
                    {
                        var token = GenerateAccessToken(url, accessKey);
                        return Task.FromResult(token);
                    };
                }).Build();
          
            hubConnection.On("brodCastBotOrder", (string notification) =>
            {
                Console.WriteLine($"Notification received: {notification}");
            });

            await hubConnection.StartAsync();

            Console.WriteLine("Listening");
            Console.ReadLine();

            await hubConnection.DisposeAsync();
        }

        private static string GenerateAccessToken(string audience, string accessKey)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = _jwtTokenHandler.CreateJwtSecurityToken(
                issuer: null,
                audience: audience,
                subject: null,
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                signingCredentials: credentials);

            return _jwtTokenHandler.WriteToken(token);
        }
    }

   
}
