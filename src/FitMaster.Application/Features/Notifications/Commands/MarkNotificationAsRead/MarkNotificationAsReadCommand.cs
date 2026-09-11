using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(long NotificationId) : IRequest<Result>;
