using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebhookRESTAPI.Core.Extensions;
using WebhookRESTAPI.Data;
using WebhookRESTAPI.Features.Webhooks.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebhookRESTAPI.Features.Webhooks.Endpoints
{
    public static class WebhookEndpoints
    {
        public static void MapWebhookEndpoints(this WebApplication app)
        {
            string groupName = "Webhooks";
            var group = app.MapGroup("api/webhooks");

            group.MapPost("/{eventType}", async (
                string eventType,
                [FromServices] ApplicationDbContext dbContext,
                HttpRequest request,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(eventType))
                {
                    return Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                                { "eventType",
                                    [
                                        "eventType cannot be null empty or whitespace"
                                    ]
                                }
                        });
                }

                eventType = eventType.ToLowerInvariant();
                var eventTypeNames = Enum.GetNames<EventType>().ToList();
                if (!eventTypeNames.Contains(eventType))
                {
                    return Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                                { "eventType",
                                    [
                                        $"These are the accepted event types: {eventTypeNames.StringJoin(",")}"
                                    ]
                                }
                        });
                }

                using var stream = new StreamReader(request.Body);
                var requestBody = await stream.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                                { "eventType",
                                    [
                                        "Request body is missing"
                                    ]
                                }
                        });
                }
                var newEvent = new Event
                {
                    EventType = Enum.Parse<EventType>(eventType),
                    CreatedAt = DateTimeOffset.UtcNow,
                    Timestamp = DateTimeOffset.UtcNow,
                    LastUpdated = DateTimeOffset.UtcNow,
                };

                await dbContext.Events.AddAsync(newEvent, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .WithTags(groupName)
            .WithDescription("Creates a webhook event")
            .WithSummary("Create a webhook event")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
