using Domain.Extensions;
using ProjectWebNotes.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.ConfigureLoggerService();
//builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureRepositoryWrapper();
builder.Services.ConfigureMySqlContext(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.ConfigureCustomExceptionMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapAreaControllerRoute(
            name: "Manager",
            pattern: "Manager/{controller}/{action}/{id?}",
            areaName: "Manager",
            defaults: new
            {
                controller = "Category",
                action = "index"
            }
        );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
