using Infoseed.MassagingPort.OrderingMenu.Helper;
using Infoseed.MessagingPortal;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.TenantServicesInfo;
using Infoseed.MessagingPortal.Web.Sunshine;
using Microsoft.AspNetCore.Mvc.Razor;
using Infoseed.MassagingPort.OrderingMenu.Services;
using Infoseed.MessagingPortal.Menus;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IMenuCategoriesAppService, ItemCategoryAppService>();
builder.Services.AddScoped<IItemsAppService, ItemsAppService>();
builder.Services.AddScoped<ITenantServicesInfoAppService, TenantServicesInfoAppService>();
builder.Services.AddScoped<IOrdersAppService, OrdersAppService>();
builder.Services.AddScoped<IDBService, CosmosDBService>();
builder.Services.AddScoped <IMenusAppService , MenusAppService>();
builder.Services.AddScoped<ILoyaltyAppService, LoyaltyAppService>();


builder.Services.AddHttpContextAccessor();




AppSettingsCoreModel.SocketIOToken = builder.Configuration["SocketIO:Token"];
  AppSettingsCoreModel.SocketIOURL = builder.Configuration["SocketIO:URL"];
  AppSettingsModel.ConnectionStrings = builder.Configuration["ConnectionStrings:Default"];


AppSettingsModel.AddHour = int.Parse(builder.Configuration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "AddHour").FirstOrDefault().Value);
AppSettingsModel.DivHour = int.Parse(builder.Configuration.GetSection("InfoSeedTime").GetChildren().Where(x => x.Key == "DivHour").FirstOrDefault().Value);
AppSettingsModel.BotApi = builder.Configuration.GetSection("Bot").GetChildren().Where(x => x.Key == "Api").FirstOrDefault().Value;

//the text-resources
//the text-resources
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization
    (LanguageViewLocationExpanderFormat.SubFolder)
    .AddDataAnnotationsLocalization();

builder.Services
    .AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();


builder.Services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(builder.Configuration["CosmosDBSettings:EndPoint"]), builder.Configuration["CosmosDBSettings:AuthKey"], new ConnectionPolicy { EnableEndpointDiscovery = false }));



builder.Services.Configure<RequestLocalizationOptions>(options => {
    var supportedCultures = new[] { "ar","en-US"};
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});


if (!string.IsNullOrEmpty(builder.Configuration["ApplicationInsights:InstrumentationKey"]))
{
    builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:InstrumentationKey"]);
}

builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

builder.Services.AddSingleton<CommonLocalizationService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(70);
    //options.Cookie.HttpOnly = true;
    //options.Cookie.IsEssential = true;
});
SocketIOManager.Init();
//builder.Services.AddTransient<ExampleService>();
//add after registering all the dependencies
//builder.Services.AddMvc();
//builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");


//AppSettingsModel.ConnectionStrings = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
//AppSettingsModel.ConnectionStrings = "Server=.;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=sa;Password=P@ssw0rd;";
//AppSettingsModel.ConnectionStrings = "Server=tcp:info-seed-db-server-stg.database.windows.net,1433;Initial Catalog=InfoSeedDB-Stg;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
//AppSettingsModel.ConnectionStrings = "Server=.;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=sa;Password=P@ssw0rd;MultipleActiveResultSets=False;Connection Timeout=30;";
//AppSettingsModel.ConnectionStrings = "Server=.;Initial Catalog=InfoSeedDb-Stg;Persist Security Info=False;User ID=sa;Password=P@ssw0rd;MultipleActiveResultSets=False;Connection Timeout=30;";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();




var supportedCultures = new[] { "ar","en-US"};
// 5. 
// Culture from the HttpRequest
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization();
app.UseRouting();


app.UseSession();
app.UseAuthorization();



app.MapRazorPages();

app.Run();
