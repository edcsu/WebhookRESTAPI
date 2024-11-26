using Microsoft.EntityFrameworkCore;
using System.Runtime;
using WebhookRESTAPI.Data;

namespace WebhookRESTAPI.Core
{
    public static class ConfigureServices
    {
        /// <summary>
        /// Add services
        /// </summary>
        /// <param name="builder"></param>
        public static void AddApiServices(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.EnableRetryOnFailure();
                });
            });
        }
    }
}
