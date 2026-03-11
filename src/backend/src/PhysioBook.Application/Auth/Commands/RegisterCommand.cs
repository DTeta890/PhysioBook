using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Auth.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone,
    string Role,
    bool IsTherapist,
    string? Specialization,
    string? Color) : IRequest<UserDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentTenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        ICurrentTenantService tenantService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _passwordService = passwordService;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Only owners and admins can register users
        var callerRole = _currentUserService.Role;
        if (callerRole != Domain.Enums.UserRole.ClinicAdmin &&
            callerRole != Domain.Enums.UserRole.SuperAdmin)
        {
            throw new ForbiddenException("Only owners and admins can register new users.");
        }

        // Check email uniqueness within tenant
        var exists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (exists)
        {
            throw new ConflictException($"A user with email '{request.Email}' already exists in this tenant.");
        }

        // Validate role
        var validRoles = new[] { "owner", "admin", "therapist", "receptionist" };
        if (!validRoles.Contains(request.Role.ToLowerInvariant()))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Role", [$"Invalid role. Must be one of: {string.Join(", ", validRoles)}"] }
            });
        }

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantService.TenantId,
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = request.Role.ToLowerInvariant(),
            IsTherapist = request.IsTherapist,
            Specialization = request.Specialization,
            Color = request.Color,
            IsActive = true,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new UserDto(user.Id, user.Email, user.FirstName, user.LastName,
            user.Role, user.IsTherapist, user.Specialization, user.Phone,
            user.AvatarUrl, user.TenantId);
    }
}
