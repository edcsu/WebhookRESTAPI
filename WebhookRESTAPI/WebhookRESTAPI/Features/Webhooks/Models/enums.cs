namespace WebhookRESTAPI.Features.Webhooks.Models
{
    public enum EventType
    {
        // Take this as an example, you can implement any event source you like.
        Hook, //(Hook created, Hook deleted ...)

        File, // (Some file uploaded, file deleted)

        Note, // (Note posted, note updated)

        Project, // (Some project created, project disabled)

        Milestone // (Milestone created, milestone is done etc...)

        //etc etc.. You can define your custom events types....
    }
}
