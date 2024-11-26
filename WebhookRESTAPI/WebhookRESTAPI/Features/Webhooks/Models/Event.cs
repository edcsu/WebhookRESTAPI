using WebhookRESTAPI.Core;

namespace WebhookRESTAPI.Features.Webhooks.Models
{
    public class Event : BaseModel
    {
        public DateTimeOffset Timestamp { get; set; }
        
        public EventType EventType { get; set; }

        public string Payload { get; set; } = default!;
    }
}
