using FluentValidation;
using WebhookRESTAPI.Features.Webhooks.ViewModels;

namespace WebhookRESTAPI.Features.Webhooks.Validators;

public class SubscriptionCreateModelValidator : AbstractValidator<SubscriptionCreateModel>
{
    public SubscriptionCreateModelValidator()
    {
        RuleFor(sub => sub.SubscriberId).NotEmpty();
        
        RuleFor(sub => sub.EventType).IsInEnum();
        
        RuleFor(sub => sub.CallbackUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(30);
        
        RuleFor(sub => sub.Secret).MaximumLength(60)
            .When(it => string.IsNullOrWhiteSpace(it.CallbackUrl));
    }
}