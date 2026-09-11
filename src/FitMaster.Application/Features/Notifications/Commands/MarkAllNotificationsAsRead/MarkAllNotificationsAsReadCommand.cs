using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public record MarkAllNotificationsAsReadCommand : IRequest<Result>;
