using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Tenants.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Tenants.Commands;

public sealed record UpdateTenantCommand(
    Guid TenantId,
    string Name,
    string ContactEmail,
    string? Phone,
    string? Address,
    string? City,
    string? Subdomain,
    bool IsActive) : IRequest<TenantDto>;

public sealed class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, TenantDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateTenantCommandHandler(
        IApplicationDbContext context,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(nameof(Tenant), request.TenantId);
        }

        tenant.Name = request.Name;
        tenant.Email = request.ContactEmail;
        tenant.Phone = request.Phone;
        tenant.Address = request.Address;
        tenant.City = request.City ?? tenant.City;
        tenant.UpdatedAt = _dateTimeProvider.Now;

        if (!request.IsActive)
        {
            tenant.SubscriptionStatus = "inactive";
        }
        else if (tenant.SubscriptionStatus == "inactive")
        {
            tenant.SubscriptionStatus = "trial";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new TenantDto(
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
}
