using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Therapists.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Therapists.Commands;

public sealed record UpdateTherapistCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string? Specialization,
    string? Color,
    bool IsActive) : IRequest<TherapistDto>;

public sealed class UpdateTherapistCommandHandler : IRequestHandler<UpdateTherapistCommand, TherapistDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateTherapistCommandHandler(
        IApplicationDbContext context,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TherapistDto> Handle(UpdateTherapistCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.IsTherapist, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.Specialization = request.Specialization;
        user.Color = request.Color;
        user.IsActive = request.IsActive;
        user.UpdatedAt = _dateTimeProvider.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return new TherapistDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone,
            user.Specialization,
            user.Color,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedAt);
    }
}
