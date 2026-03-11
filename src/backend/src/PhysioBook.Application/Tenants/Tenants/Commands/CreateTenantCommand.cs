using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Tenants.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Tenants.Commands;

public sealed record CreateTenantCommand(
    string Name,
    string Slug,
    string ContactEmail,
    string? Phone,
    string? Address,
    string? City,
    string? Subdomain,
    string AdminEmail,
    string AdminPassword,
    string AdminFirstName,
    string AdminLastName) : IRequest<TenantDto>;

public sealed class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateTenantCommandHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _passwordService = passwordService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Check slug uniqueness (Tenant is not filtered by tenant query filter since it doesn't extend BaseEntity)
        var slugExists = await _context.Tenants
            .AnyAsync(t => t.Slug == request.Slug, cancellationToken);

        if (slugExists)
        {
            throw new ConflictException($"A tenant with slug '{request.Slug}' already exists.");
        }

        // Check email uniqueness across tenants
        var emailExists = await _context.Tenants
            .AnyAsync(t => t.Email == request.ContactEmail, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException($"A tenant with email '{request.ContactEmail}' already exists.");
        }

        var now = _dateTimeProvider.Now;
        var tenantId = Guid.NewGuid();

        var tenant = new Tenant
        {
            Id = tenantId,
            Name = request.Name,
            Slug = request.Slug.ToLowerInvariant(),
            Email = request.ContactEmail,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City ?? "Elbasan",
            Timezone = "Europe/Tirane",
            SubscriptionPlan = "starter",
            SubscriptionStatus = "trial",
            TrialEndsAt = now.AddDays(14),
            Settings = "{}",
            CreatedAt = now,
            UpdatedAt = now,
        };

        _context.Tenants.Add(tenant);

        // Create admin user for the new tenant
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = request.AdminEmail,
            PasswordHash = _passwordService.Hash(request.AdminPassword),
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            Role = "owner",
            IsTherapist = false,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _context.Users.Add(adminUser);

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(tenant);
    }

    private static TenantDto MapToDto(Tenant tenant) => new(
        tenant.Id,
        tenant.Name,
        tenant.Slug,
        tenant.Email,
        tenant.Phone,
        tenant.Address,
        tenant.City,
        tenant.Timezone,
        tenant.LogoUrl,
        tenant.SubscriptionPlan,
        tenant.SubscriptionStatus,
        tenant.TrialEndsAt,
        tenant.SubscriptionStatus != "inactive",
        tenant.CreatedAt);
}
