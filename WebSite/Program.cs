using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;
using WebSite;
using WebSite.Model;


    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

    builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

//add configurations sections
builder.Services.Configure<AppInfo>(builder.Configuration.GetSection("AppInfo"));
builder.Services.Configure<ContactsInfo>(builder.Configuration.GetSection("ContactsInfo"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[]
        {
                new CultureInfo("en-US"),
                new CultureInfo("it-IT")
            };
        options.DefaultRequestCulture = new RequestCulture("it-IT", "it-IT");

        // You must explicitly state which cultures your application supports.
        // These are the cultures the app supports for formatting 
        // numbers, dates, etc.

        options.SupportedCultures = supportedCultures;

        // These are the cultures the app supports for UI strings, 
        // i.e. we have localized resources for.
        // Ordine dei provider (IMPORTANTE!)
        options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(), // ?culture=en-US
        new CookieRequestCultureProvider(),      // cookie salvato dopo scelta lingua
        new AcceptLanguageHeaderRequestCultureProvider() // fallback browser
    };
        options.SupportedUICultures = supportedCultures;
    });
    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();

    //add configurations sections
    //builder.Services.Configure<AppInfo>(builder.Configuration.GetSection("AppInfo"));
    //builder.Services.Configure<ContactsInfo>(builder.Configuration.GetSection("ContactsInfo"));

    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }


    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);
app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();



