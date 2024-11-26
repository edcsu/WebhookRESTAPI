namespace WebhookRESTAPI.Features.Webhooks.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }
        
        public string EventType { get; set; }
        
        public string Payload { get; set; }
    }
}
