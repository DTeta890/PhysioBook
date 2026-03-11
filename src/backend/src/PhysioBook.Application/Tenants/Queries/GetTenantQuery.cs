using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Tenants.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Tenants.Queries;

public sealed record GetTenantQuery(Guid TenantId) : IRequest<TenantDto>;

public sealed class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, TenantDto>
{
    private readonly IApplicationDbContext _context;

    public GetTenantQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TenantDto> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
        {
            throw new NotFoundException(nameof(Tenant), request.TenantId);
        }

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
