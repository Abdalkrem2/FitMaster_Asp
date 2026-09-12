using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Exercises.Commands.UploadExerciseMedia;

public record UploadExerciseMediaCommand(
    Guid ExerciseId,
    byte[] Content,
    string FileName,
    string ContentType,
    string? Role,
    string? Sex,
    string? ViewAngle) : IRequest<Result<Guid>>;
