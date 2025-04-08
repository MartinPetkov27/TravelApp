using DataLayer;
using BusinessLayer;
using ServiceLayer;
using PresentationLayer.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using Microsoft.Extensions.Options;
using PresentationLayer;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TravelAppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'TravelAppDbContextConnection' not found.");;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<TravelAppDbContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<User>(/*options => options.SignIn.RequireConfirmedAccount = true*/).AddEntityFrameworkStores<TravelAppDbContext>();

//builder.Services.AddScoped<TravelAppDbContext, TravelAppDbContext>();

builder.Services.AddHttpClient<GeoNamesService>();

#region AddScoped(Manager and Content classes dependency injections)

//AddScoped(Manager and Content classes dependency injections)
builder.Services.AddScoped<CountryManager, CountryManager>();
builder.Services.AddScoped<CountryContext, CountryContext>();

builder.Services.AddScoped<BucketListManager, BucketListManager>();
builder.Services.AddScoped<BucketListContext, BucketListContext>();

builder.Services.AddScoped<PlaceManager, PlaceManager>();
builder.Services.AddScoped<PlaceContext, PlaceContext>();

builder.Services.AddScoped<StoryManager, StoryManager>();
builder.Services.AddScoped<StoryContext, StoryContext>();

builder.Services.AddScoped<TripManager, TripManager>();
builder.Services.AddScoped<TripContext, TripContext>();

builder.Services.AddScoped<UserManager, UserManager>();
builder.Services.AddScoped<UserContext, UserContext>();

builder.Services.AddScoped<IdentityContext, IdentityContext>();
builder.Services.AddScoped<IdentityManager, IdentityManager>(); 

#endregion

builder.Services.AddIdentity<User, IdentityRole>(io =>
{
    io.Password.RequiredLength = 5;
    io.Password.RequireNonAlphanumeric = false;
    io.Password.RequiredUniqueChars = 0;
    io.Password.RequireUppercase = false;
    io.Password.RequireDigit = false;

    io.User.RequireUniqueEmail = false;

    //Those should be removeds when the email service is implemented
    io.SignIn.RequireConfirmedAccount = false;
    io.SignIn.RequireConfirmedEmail = false;

    io.Lockout.MaxFailedAccessAttempts = 3;
})
                 .AddEntityFrameworkStores<TravelAppDbContext>()
                 .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender, FakeEmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
