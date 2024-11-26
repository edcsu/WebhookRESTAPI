namespace WebhookRESTAPI.Core
{
    public class BaseModel
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastUpdated { get; set; }
    }
}
