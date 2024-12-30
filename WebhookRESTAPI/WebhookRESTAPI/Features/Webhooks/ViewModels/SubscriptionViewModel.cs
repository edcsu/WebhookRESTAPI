using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebhookRESTAPI.Features.Webhooks.Models;

namespace WebhookRESTAPI.Features.Webhooks.ViewModels;

public record SubscriptionViewModel(
    [property: Description("The unique identifier for the subscription")]
    Guid Id,    
    [property: Description("The unique identifier for the subscriber")]
    Guid SubscriberId,
    [property: Description("The event type to be subscribed to")]
    EventType EventType,
    [property: Description("This will be invoked by the API after an event is done")]
    string CallbackUrl, 
    [property: Description("The secret for the subscriber")]
    string? Secret);