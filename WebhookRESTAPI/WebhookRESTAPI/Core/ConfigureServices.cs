using Microsoft.EntityFrameworkCore;
using System.Runtime;
using FluentValidation;
using WebhookRESTAPI.Data;
using WebhookRESTAPI.Features.Webhooks.Validators;
using WebhookRESTAPI.Features.Webhooks.ViewModels;

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
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), opts =>
                {
                    opts.EnableRetryOnFailure();
                });
            });

            builder.Services.AddScoped<IValidator<SubscriptionCreateModel>, SubscriptionCreateModelValidator>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(ApiConstants.AllowedClients, policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }
    }
}
