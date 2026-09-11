using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Notifications.Commands.ClearAllNotifications;

public record ClearAllNotificationsCommand : IRequest<Result>;
