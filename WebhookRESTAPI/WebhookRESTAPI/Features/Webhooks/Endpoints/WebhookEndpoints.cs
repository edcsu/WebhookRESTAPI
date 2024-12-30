using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebhookRESTAPI.Core.Extensions;
using WebhookRESTAPI.Data;
using WebhookRESTAPI.Features.Webhooks.Models;
using WebhookRESTAPI.Features.Webhooks.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebhookRESTAPI.Features.Webhooks.Endpoints
{
    public static class WebhookEndpoints
    {
        public static void MapWebhookEndpoints(this WebApplication app)
        {
            const string groupName = "Webhooks";
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
                var requestBody = await stream.ReadToEndAsync(cancellationToken);
                
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
                        }
                    );
                }

                var newEvent = new Event
                {
                    EventType = Enum.Parse<EventType>(eventType),
                    Payload = requestBody,
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
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable);
            
            group.MapPost("/subscribe", async (
                [FromBody] SubscriptionCreateModel createModel,
                [FromServices] ApplicationDbContext dbContext,
                [FromServices] IValidator<SubscriptionCreateModel> subscriptionCreateModelValidator,
                HttpRequest request,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await subscriptionCreateModelValidator.ValidateAsync(createModel, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var newSubscription = new Subscription
                {
                    SubscriberId = createModel.SubscriberId,
                    EventType = createModel.EventType,
                    CallbackUrl = createModel.CallbackUrl,
                    Secret = createModel.Secret,
                    CreatedAt = DateTimeOffset.UtcNow,
                    LastUpdated = DateTimeOffset.UtcNow,
                };

                var createdSubscription= await dbContext.Subscriptions.AddAsync(newSubscription, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                var savedSubscription = new SubscriptionViewModel(
                    createdSubscription.Entity.Id,
                    createdSubscription.Entity.SubscriberId, 
                    createdSubscription.Entity.EventType, 
                    createdSubscription.Entity.CallbackUrl,
                    createdSubscription.Entity.Secret);
                
                return Results.Ok(savedSubscription);
            })
            .WithTags(groupName)
            .WithDescription("Creates a subscription to a webhook event")
            .WithSummary("Create a subscription")
            .Produces<SubscriptionViewModel>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable);
            
            group.MapGet("/{subscriberId:guid}", async (
                Guid subscriberId,
                [FromServices] ApplicationDbContext dbContext,
                HttpRequest request,
                CancellationToken cancellationToken) =>
            {
                if (Guid.Empty == subscriberId)
                {
                    return Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            { "subscriberId",
                                [
                                    "Subscriber Id is empty"
                                ]
                            }
                        });
                }

                var subscriptions= await dbContext.Subscriptions
                    .Where(sub => sub.SubscriberId == subscriberId && sub.IsActive)
                    .OrderByDescending(item => item.CreatedAt)
                    .Select(it => new SubscriptionViewModel(
                        it.Id,
                        it.SubscriberId, 
                        it.EventType, 
                        it.CallbackUrl,
                        it.Secret))
                    .ToListAsync(cancellationToken);

                return Results.Ok(subscriptions);
            })
            .WithTags(groupName)
            .WithDescription("Gets a list subscriptions based on the subscriber.")
            .WithSummary("Get subscriptions")
            .Produces<List<SubscriptionViewModel>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable);
            
            group.MapDelete("/{subscriptionId:guid}", async (
                Guid subscriptionId,
                [FromServices] ApplicationDbContext dbContext,
                HttpRequest request,
                CancellationToken cancellationToken) =>
            {
                if (Guid.Empty == subscriptionId)
                {
                    return Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            { "subscriptionId",
                                [
                                    "subscription Id is empty"
                                ]
                            }
                        });
                }

                var subscription= await dbContext.Subscriptions
                                            .FirstOrDefaultAsync(sub => sub.SubscriberId == subscriptionId && sub.IsActive, cancellationToken);
                if (subscription is null)
                    return Results.NotFound();
                
                subscription.IsActive = false;
                subscription.LastUpdated = DateTimeOffset.UtcNow;

                var updatedSubscription = dbContext.Subscriptions.Update(subscription);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .WithTags(groupName)
            .WithDescription("Deletes a subscription")
            .WithSummary("Delete subscription")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status503ServiceUnavailable);
        }
    }
}
