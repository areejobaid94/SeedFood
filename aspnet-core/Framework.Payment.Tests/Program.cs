using Microsoft.Extensions.Configuration;
using System;

namespace Framework.Payment.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IConfiguration Configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json"/*, optional: true, reloadOnChange: true*/)
                        //.AddEnvironmentVariables()
                        //.AddCommandLine(args)
                        .Build();
            PaymentProvider payment = new PaymentProvider(Configuration);
            var response = payment.PrepareCheckout(new CheckoutPaymentRequest()
            {
                Amount = 20,
                Currency = "JOD",
                PaymentBrand = "",
                PaymentType = PaymentType.DB,
                TaxAmount = 2
            });

        }
    }
}
