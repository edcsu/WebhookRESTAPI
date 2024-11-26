using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebhookRESTAPI.Data;

namespace WebhookRESTAPI.Features.Webhooks.Endpoints
{
    public static class WebhookEndpoints
    {
        public static void MapWebhookEndpoints(this WebApplication app)
        {
            string groupName = "Webhooks";
            var group = app.MapGroup("api/webhooks");

            group.MapPost("/", (
                [FromServices] ApplicationDbContext dbContext,
                HttpRequest request,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok();
            })
            .WithTags(groupName)
            .WithDescription("Creates a webhook event")
            .WithSummary("Create a webhook event")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
        }
    }
}
