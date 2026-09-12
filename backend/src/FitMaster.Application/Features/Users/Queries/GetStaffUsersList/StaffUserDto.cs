using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Users.Queries.GetStaffUsersList;

public record StaffUserDto(
    long Id,
    string FullName,
    string Phone,
    string? Gender,
    bool IsActivated,
    string? ProfilePicture,
    List<AppRole> Roles);
