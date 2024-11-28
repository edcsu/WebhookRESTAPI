
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Templates.Themes;
using SerilogTracing;
using SerilogTracing.Expressions;
using WebhookRESTAPI.Core;
using WebhookRESTAPI.Data;
using WebhookRESTAPI.Features.Webhooks.Endpoints;

namespace WebhookRESTAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                .Enrich.WithProperty("Application", "Webhook")
                .WriteTo.Console(Formatters.CreateConsoleTextFormatter(theme: TemplateTheme.Code))
                .WriteTo.Debug()
                .WriteTo.Trace()
                .WriteTo.File("Logs/applog.log", rollingInterval: RollingInterval.Hour)
                .WriteTo.File(new CompactJsonFormatter(), "Logs/applog.json", rollingInterval: RollingInterval.Hour)
                .CreateLogger();

            using var listener = new ActivityListenerConfiguration()
                .Instrument.AspNetCoreRequests()
                .TraceToSharedLogger();

            try
            {
                Log.Information("Starting up"); 
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddAuthorization();

                builder.AddApiServices();

                builder.Services.AddSerilog();
                
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                //builder.Services.AddEndpointsApiExplorer();
                //builder.Services.AddSwaggerGen(); 
                builder.Services.AddOpenApi();

                var app = builder.Build();
                
                app.UseCors(ApiConstants.AllowedClients);

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                }
                app.MapOpenApi();

                app.MapScalarApiReference(options =>
                {
                    options.Title = "Webhook API";
                    options.Theme = ScalarTheme.Default;
                    options.ShowSidebar = true;
                    options.DarkMode = true;
                    options.EndpointPathPrefix = "/docs/{documentName}";
                    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });

                app.UseHttpsRedirection();

                app.UseAuthorization();

                SeedData.Initialize(app);

                app.MapWebhookEndpoints();

                app.Run();
            }
            catch (Exception exception)
            {
                var type = exception.GetType().Name;
                if (type.Equals("StopTheHostException", StringComparison.OrdinalIgnoreCase))
                {
                    throw;
                }

                Log.Fatal(exception, "Unknown exception");
            }
            finally
            {
                Log.Information("Application completely stopped");
                Log.CloseAndFlush();
            }
        }
    }
}



