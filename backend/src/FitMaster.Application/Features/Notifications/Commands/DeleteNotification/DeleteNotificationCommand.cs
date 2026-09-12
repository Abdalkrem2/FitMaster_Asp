using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Notifications.Commands.DeleteNotification;

public record DeleteNotificationCommand(long NotificationId) : IRequest<Result>;
