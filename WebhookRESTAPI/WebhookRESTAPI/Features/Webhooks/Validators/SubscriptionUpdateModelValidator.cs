using FluentValidation;
using WebhookRESTAPI.Features.Webhooks.ViewModels;

namespace WebhookRESTAPI.Features.Webhooks.Validators;

public class SubscriptionUpdateModelValidator : AbstractValidator<SubscriptionUpdateModel>
{
    public SubscriptionUpdateModelValidator()
    {
        
        RuleFor(sub => sub.CallbackUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(30)
            .When(it => string.IsNullOrWhiteSpace(it.CallbackUrl));
        
        RuleFor(sub => sub.Secret).MaximumLength(60)
            .When(it => string.IsNullOrWhiteSpace(it.Secret));
    }
}