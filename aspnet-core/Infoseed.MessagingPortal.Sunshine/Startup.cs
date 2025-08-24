using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infoseed.MessagingPortal.Sunshine.Interfaces;
using Infoseed.MessagingPortal.Sunshine.Models;
using Infoseed.MessagingPortal.Sunshine.Services;
using Infoseed.MessagingPortal.Sunshine.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infoseed.MessagingPortal.Sunshine
{
    public class Startup
    {
        readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // services.AddSignalR().AddMessagePackProtocol();
            services.AddSignalR();
            services.AddScoped<IDBService, CosmosDBService>();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder.WithOrigins("http://localhost:4200/", "https://localhost:44301/")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true));
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy(name: myAllowSpecificOrigins,
            //                      builder =>
            //                      {
            //                          builder.WithOrigins("http://localhost:4200/",
            //                                              "https://localhost:44301/");
            //                      });
            //});
            //services.AddSignalR()
            //        .AddJsonProtocol(options => {
            //            options.PayloadSerializerOptions.PropertyNamingPolicy = null
            //        });

            //// When constructing your connection:
            //var connection = new HubConnectionBuilder()
            //    .AddJsonProtocol(options => {
            //        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            //    })
            //    .Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "https://localhost:44398/swagger");
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowAll");
            //app.UseCors(myAllowSpecificOrigins);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHub<NotificationHub>("/chathub");
                endpoints.MapHub<TeamInboxHub>("/teaminbox");
            });


          //  app.UseCors("AllowAll");

        }
    }
}
