namespace WebhookRESTAPI.Features.Webhooks.Models
{
    public enum EventType
    {
        // Take this as an example, you can implement any event source you like.
        hook, //(Hook created, Hook deleted ...)

        file, // (Some file uploaded, file deleted)

        note, // (Note posted, note updated)

        project, // (Some project created, project dissabled)

        milestone // (Milestone created, milestone is done etc...)

        //etc etc.. You can define your custom events types....
    }
}
