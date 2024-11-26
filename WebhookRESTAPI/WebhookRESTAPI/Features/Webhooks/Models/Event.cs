namespace WebhookRESTAPI.Features.Webhooks.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }
        
        public EventType EventType { get; set; }

        public string Payload { get; set; } = default!;
    }
}
