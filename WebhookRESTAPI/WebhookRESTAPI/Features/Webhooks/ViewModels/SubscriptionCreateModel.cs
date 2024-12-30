using WebhookRESTAPI.Features.Webhooks.Models;

namespace WebhookRESTAPI.Features.Webhooks.ViewModels;

public record SubscriptionCreateModel(Guid SubscriberId, EventType EventType, string CallbackUrl, string? Secret);