
using WebhookRESTAPI.Core;
using WebhookRESTAPI.Data;
using WebhookRESTAPI.Features.Webhooks.Endpoints;

namespace WebhookRESTAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.AddApiServices();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            
            SeedData.Initialize(app);

            app.MapWebhookEndpoints();

            app.Run();
        }
    }
}
