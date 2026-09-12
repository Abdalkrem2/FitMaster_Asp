using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Members.Commands.DeleteMember;

public record DeleteMemberCommand(long MemberId) : IRequest<Result>;
