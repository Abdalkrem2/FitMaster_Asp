using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Users.Queries.GetStaffUsersList;

public class GetStaffUsersListHandler(IApplicationDbContext db)
    : IRequestHandler<GetStaffUsersListQuery, PagedResult<StaffUserDto>>
{
    public async Task<PagedResult<StaffUserDto>> Handle(GetStaffUsersListQuery request, CancellationToken cancellationToken)
    {
        var query = db.Users
            .Where(u => !u.Deleted && u.Roles.Any(r => r.RoleName == AppRole.Admin || r.RoleName == AppRole.Employee));

        var totalElements = await query.CountAsync(cancellationToken);

        var page = Math.Max(request.Page, 0);
        var size = request.Size <= 0 ? 10 : request.Size;

        var items = await query
            .OrderBy(u => u.FullName)
            .Skip(page * size)
            .Take(size)
            .Select(u => new StaffUserDto(
                u.Id,
                u.FullName,
                u.Phone,
                u.Gender,
                u.IsActivated,
                u.ProfilePicture,
                u.Roles.Select(r => r.RoleName).ToList()))
            .ToListAsync(cancellationToken);

        return PagedResult<StaffUserDto>.Create(items, totalElements, page, size);
    }
}
