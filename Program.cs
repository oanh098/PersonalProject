using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalProject.Data;
using PersonalProject.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PersonalProjectContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PersonalProjectContext") ?? throw new InvalidOperationException("Connection string 'PersonalProjectContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PersonalProjectContext>();
    context.Database.Migrate(); // <-- Add this line to apply migrations
    SeedData.Initialize(services);
}
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

app.MapControllerRoute(
    name: "Skills",
    pattern: "{controller=Skills}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Profile",
    pattern: "{controller=Profile}/{action=Index}/{id?}");

app.Run();