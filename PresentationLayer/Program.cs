using DataLayer;
using PresentationLayer.Controllers;
using ServiceLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TravelAppDbContext>();
//
builder.Services.AddScoped<CountryManager, CountryManager>();
builder.Services.AddScoped<CountryContext, CountryContext>();
//
builder.Services.AddScoped<BucketListManager, BucketListManager>();
builder.Services.AddScoped<BucketListContext, BucketListContext>();
//
builder.Services.AddScoped<PlaceManager, PlaceManager>();
builder.Services.AddScoped<PlaceContext, PlaceContext>();
//
builder.Services.AddScoped<StoryManager, StoryManager>();
builder.Services.AddScoped<StoryContext, StoryContext>();
//
builder.Services.AddScoped<TripManager, TripManager>();
builder.Services.AddScoped<TripContext, TripContext>();
//
builder.Services.AddScoped<UserManager, UserManager>();
builder.Services.AddScoped<UserContext, UserContext>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
