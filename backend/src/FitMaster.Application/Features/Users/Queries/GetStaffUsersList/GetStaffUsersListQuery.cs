using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Users.Queries.GetStaffUsersList;

/// <summary>Lists staff accounts (Admin/Employee roles) - never Members.</summary>
public record GetStaffUsersListQuery(int Page = 0, int Size = 10) : IRequest<PagedResult<StaffUserDto>>;
