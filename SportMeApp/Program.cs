using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PusherServer;
using SportMeApp.Models;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();


// add mdf database service
builder.Services.AddDbContext<SportMeAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SportMeAppContext") ?? throw new InvalidOperationException("Connection string 'SportMeAppContext' not found.")));

// pusher configuration
var pusherOptions = new PusherOptions
{
    Cluster = builder.Configuration["Pusher:Cluster"],
    Encrypted = true
};
builder.Services.AddSingleton(new Pusher(
    builder.Configuration["Pusher:AppId"],
    builder.Configuration["Pusher:Key"],
    builder.Configuration["Pusher:Secret"],
    pusherOptions));


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
