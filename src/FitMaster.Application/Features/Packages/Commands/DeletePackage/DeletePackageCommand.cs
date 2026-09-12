using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Packages.Commands.DeletePackage;

/// <summary>Soft-deletes a package - it stops appearing/being sellable, but
/// existing Memberships that reference it are left untouched.</summary>
public record DeletePackageCommand(long Id) : IRequest<Result>;
