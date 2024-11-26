namespace WebhookRESTAPI.Features.Webhooks.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }
        
        public Guid SubscriberId { get; set; }
        
        public EventType EventType { get; set; }

        public string CallbackUrl { get; set; } = default!;
        
        public string? Secret { get; set; }
    }
}
