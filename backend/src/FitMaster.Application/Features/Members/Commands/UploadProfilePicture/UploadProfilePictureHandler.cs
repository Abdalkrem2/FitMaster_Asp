using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Commands.UploadProfilePicture;

public class UploadProfilePictureHandler(IApplicationDbContext db, IFileStorageService fileStorage)
    : IRequestHandler<UploadProfilePictureCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.MemberId && !u.Deleted, cancellationToken);
        if (user is null)
        {
            return Result<string>.Failure("Member not found.");
        }

        await using var stream = new MemoryStream(request.Content);
        var uploaded = await fileStorage.UploadImageAsync(stream, request.FileName, "members", cancellationToken);

        user.ProfilePicture = uploaded.Url;
        await db.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(uploaded.Url);
    }
}
