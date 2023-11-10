var builder = WebApplication.CreateBuilder(args);
// Initialize the builder
// for services to the container.
builder.Services.AddControllersWithViews();
// build the web app
var app = builder.Build();

// Configure the HTTP request. check if running non=development 
if (!app.Environment.IsDevelopment())
{
    //check for  exceptions or take say error 
    app.UseExceptionHandler("/Home/Error");
    //use of HTTPS for secure connections
    app.UseHsts();
}
// redirect the request
app.UseHttpsRedirection();

// for static map 
app.UseStaticFiles();
// route http requests 
app.UseRouting();
// middle authorization 
app.UseAuthorization();
//defualt controller route 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//run the application 
app.Run();
