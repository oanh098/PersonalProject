using System;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using PersonalProject.Areas.Identity.Data;
using PersonalProject.Data;
using PersonalProject.Services;

namespace PersonalProject.Extensions;

public static class ServiceExtensions
{
    //1. Database Configuration
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection string is missing or empty.");
        }

        services.AddDbContext<PersonalProjectContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<RazorPagesPersonalProjectAuth>(options =>
            options.UseNpgsql(connectionString));
    }
           
}
