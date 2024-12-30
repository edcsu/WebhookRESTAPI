using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebhookRESTAPI.Features.Webhooks.Models;

namespace WebhookRESTAPI.Features.Webhooks.ViewModels;

public record SubscriptionUpdateModel(
    [property: Required]
    [property: Description("This will be invoked by the API after an event is done")]
    [property: MaxLength(30)]
    string CallbackUrl, 
    [property: Required]
    [property: Description("The secret for the subscriber")]
    [property: MaxLength(60)]
    string? Secret);