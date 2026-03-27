using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalProject.Data;
using PersonalProject.Models;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;    
using Microsoft.AspNetCore.Identity;
using PersonalProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Npgsql;
using StackExchange.Redis;
using PersonalProject.Services;
using PersonalProject.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Humanizer;
using Microsoft.OpenApi.Models; 



var builder = WebApplication.CreateBuilder(args);

// Configure Npgsql to handle DateTimeKind.Unspecified as UTC
// This should be done once at application startup.
// It's crucial to set EnableLegacyTimestampBehavior(false) for modern behavior.
// If you don't want to use NodaTime, use the following:
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false); // Important for modern DateTime handling

// Load user secrets
// only use secrets in development mode
if (builder.Environment.IsDevelopment())
{    
    builder.Configuration.AddUserSecrets<Program>();
}

// Retrieve the connection string ONCE
//var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

// Retrieve the Redis connection string ONCE
var redisConnection = builder.Configuration.GetSection("RedisCache")["Configuration"];

// Services are registered here, including DbContext, Identity, and Caching.

// chỗ này dùng ConfigureDatabase viết trong static file Extensions/ServiceExtensions.cs
// cách này giúp code gọn hơn và tái sử dụng được
builder.Services.ConfigureDatabase(builder.Configuration);

// Configure Identity options
// Login allowed immediately after registration
builder.Services.AddDefaultIdentity<RazorPagesPersonalProjectUser>()
    .AddEntityFrameworkStores<RazorPagesPersonalProjectAuth>();

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options => 
{
    // Configure identity options here if needed
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;

    // Other identity options can be configured here
});

// Cloudinary
var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var account = new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);
var cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);

// Authentication and Authorization
builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin_greetings", policy => policy.RequireAuthenticatedUser());

// Configure distributed caching with Redis
// 2. Parse the string into ConfigurationOptions
if (!string.IsNullOrEmpty(redisConnection))
{
    var opt = ConfigurationOptions.Parse(redisConnection, true);

    // opt.User = "red-d3ph1u8gjchc73aimurg";
    // // Set security options explicitly (Render requires TLS/SSL)
    // // This is the CRITICAL part that may be failing in the simple string parsing
    // opt.Ssl = true;
    // opt.Password = "hQCtbRbsPA2p0acoeT54LWYQ9AYrTZtp"; // Explicitly set it from config or hardcode for testing
    //                                                    // OPTIONAL: Add a Connect Timeout (in milliseconds)
    // These options setting in appsettings.json

    opt.AbortOnConnectFail = false;
    opt.ConnectTimeout = 15000;
    // --- PART 1: Configure Redis for IDistributedCache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        // options.Configuration = redisConnection;
        options.ConfigurationOptions = opt;
        options.InstanceName = "PersonalProject_";
    });
    // --- PART 2: Configure Session Middelware to use Distributed Cache
    builder.Services.AddSession(options =>
    {
        options.Cookie.Name = "MyApp.Session";

        // Set cookie to be sent only over HTTPS (highly recommended in production)
        options.Cookie.IsEssential = true;
        options.Cookie.HttpOnly = true; // prevent client-side scripts from accessing the cookie
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS

        // Set an ideal timeout for sessions
        options.IdleTimeout = TimeSpan.FromMinutes(20);

    });
    // --- PART 3: Add the Session Middleware to the pipeline (in app.Build section below
    // This must be added before UseRouting and after UseAuthorization
}
else
{
    // If Redis connection string is not found, log a warning or handle accordingly
    Console.WriteLine("Warning: Redis connection string is missing or empty. Distributed caching will not be enabled.");
}

// 1. Register the HttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();
// 2. Register the CartService
// builder.Services.AddScoped<CartService>();

// MVC & Razor Pages
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Swagger is registered
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment Aggregator API", Version = "v1" });
    // This part adds the Authorization button to Swagger UI
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Enter your merchant API key (X-Merchant-Key)",
        Name = "X-Merchant-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});

// IServices, CartService, and other dependencies are registered here as needed.
builder.Services.AddScoped<ICartService, CartService>();
//"Whenever a Controller asks for an IOrderService (the Menu), 
//give them an instance of OrderService (the Kitchen)."
builder.Services.AddScoped<IOrderService, OrderService>(); 


// Print the environment and connection string to the console
// this will print the environment and connection string to the console 
// to know what configuration is being used
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {env}");
Console.WriteLine($"ConnectionString: {conn}");
Console.WriteLine($"Redis Configuration: {redisConnection}");

var app = builder.Build();

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PersonalProjectContext>();
    context.Database.Migrate(); // <-- Add this line to apply migrations
    SeedData.Initialize(services);
}
// Add this so Swagger works in your local environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Explicitly set the endpoint to avoid routing confusion
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Aggregator API V1");
    });
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
app.UseAuthentication(); // <-- Add this line before UseAuthorization


// Use Session Middleware
app.UseSession(); // <-- Add this line to enable session handling
app.UseAuthorization();

//
app.Use(async (context, next) =>
{
    // 1. Logic BEFORE next(): Prepare the data
    string merchantId = context.Items["MerchantId"]?.ToString() ?? "Not_Found";

    // 2. Set headers BEFORE next(): This prevents the "Read-Only" crash
    context.Response.Headers["X-Debug-MerchantId"] = merchantId;
    context.Response.Headers.Append("X-Debug-Trace", "Middleware-Active");

    // 3. Now let the request continue to the Controller/View
    await next();
});
//Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Skills",
    pattern: "{controller=Skills}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Profile",
    pattern: "{controller=Profile}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Visible",
    pattern: "{controller=Visible}/{action=Index}/{id?}");

app.MapRazorPages();

// Example of a secure endpoint
app.MapGet("/HW", () => "Hello World!")
    .RequireAuthorization("admin_greetings");

app.MapGet("/debug-claims", (System.Security.Claims.ClaimsPrincipal user) =>
{
    return user.Claims.Select(c => new { c.Type, c.Value });
}).RequireAuthorization();

app.MapGet("/test-redis", async (IDistributedCache cache, HttpContext context) =>
{
    //1. Test Distributed Cache (Working)
    var testKey = "test_time";
    var currentTime = DateTime.Now.ToString();

    // 1. Write to Redis
    await cache.SetStringAsync(testKey, currentTime);

    // 2. Read from Redis
    var cachedValue = await cache.GetStringAsync(testKey);

    // 3. Test Session (This will trigger the cookie)
    context.Session.SetString("SessionTest", "Session Is Active");

    return Results.Ok(new { 
        Success = cachedValue == currentTime, 
        SessionValue = context.Session.GetString("SessionTest"),
        Value = cachedValue 
    });
});

app.MapGet("/Order/Confirmation", () => "Order Confirmation");









app.Run();