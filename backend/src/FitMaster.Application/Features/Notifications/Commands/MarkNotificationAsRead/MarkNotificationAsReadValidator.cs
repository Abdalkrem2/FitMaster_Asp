using FluentValidation;

namespace FitMaster.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadValidator()
    {
        RuleFor(x => x.NotificationId).GreaterThan(0);
    }
}
