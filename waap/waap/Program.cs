using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NToastNotify;
using waap;
using waap.Data;
using waap.Data.SeedDataBase;
using wapp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

//builder.Services.AddControllersWithViews();
    

// Localization configuration
const string defaultCulture = "en";

CultureInfo defaultCultureCI = new CultureInfo(defaultCulture);
defaultCultureCI.NumberFormat.CurrencyDecimalSeparator = ".";
defaultCultureCI.NumberFormat.NumberDecimalSeparator = ".";

var supportedCultures = new[]
{
    defaultCultureCI,
    new CultureInfo("pt"),
    new CultureInfo("es")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCultureCI);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services
    .AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    // è necessario que exista a data notation no modelo senão não vai traduzir
    .AddDataAnnotationsLocalization(options =>
    {
            options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resource)) ;
    })
    .AddNToastNotifyToastr(new ToastrOptions()
    {
        ProgressBar = true,
        PositionClass = ToastPositions.TopRight
    });

builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);  

SeedDB(); // Executed every time the application restarts

app.Run();




void SeedDB()
{
    // using var scope = app.Services.CreateScope();
    // var services = scope.ServiceProvider;

    // var dbContext = services.GetRequiredService<ApplicationDbContext>();
    // var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // SeedDatabase.Seed(dbContext, userManager, roleManager);
}