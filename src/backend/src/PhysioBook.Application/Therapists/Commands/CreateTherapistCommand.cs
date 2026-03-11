using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Therapists.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Therapists.Commands;

public sealed record CreateTherapistCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string? Phone,
    string? Specialization,
    string? Color) : IRequest<TherapistDto>;

public sealed class CreateTherapistCommandHandler : IRequestHandler<CreateTherapistCommand, TherapistDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentTenantService _currentTenantService;

    public CreateTherapistCommandHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        IDateTimeProvider dateTimeProvider,
        ICurrentTenantService currentTenantService)
    {
        _context = context;
        _passwordService = passwordService;
        _dateTimeProvider = dateTimeProvider;
        _currentTenantService = currentTenantService;
    }

    public async Task<TherapistDto> Handle(CreateTherapistCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException($"A user with email '{request.Email}' already exists.");
        }

        var now = _dateTimeProvider.Now;

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = _currentTenantService.TenantId,
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = "therapist",
            IsTherapist = true,
            Specialization = request.Specialization,
            Color = request.Color,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    private static TherapistDto MapToDto(User user) => new(
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
