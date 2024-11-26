using WebhookRESTAPI.Core;

namespace WebhookRESTAPI.Features.Webhooks.Models
{
    public class Subscription : BaseModel
    {
        public Guid SubscriberId { get; set; }
        
        public EventType EventType { get; set; }

        public string CallbackUrl { get; set; } = default!;
        
        public string? Secret { get; set; }

        public bool IsActive { get; set; }
    }
}
