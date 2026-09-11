using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Members.Commands.UploadProfilePicture;

public record UploadProfilePictureCommand(long MemberId, byte[] Content, string FileName, string ContentType) : IRequest<Result<string>>;
