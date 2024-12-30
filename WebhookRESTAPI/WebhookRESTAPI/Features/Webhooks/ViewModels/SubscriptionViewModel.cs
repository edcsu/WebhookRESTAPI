using WebhookRESTAPI.Features.Webhooks.Models;

namespace WebhookRESTAPI.Features.Webhooks.ViewModels;

public record SubscriptionViewModel( Guid SubscriberId, EventType EventType, string CallbackUrl, string? Secret);