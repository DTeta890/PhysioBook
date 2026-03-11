using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Tenants.DTOs;

namespace PhysioBook.Application.Tenants.Queries;

public sealed record GetTenantsQuery : IRequest<List<TenantDto>>;

public sealed class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, List<TenantDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTenantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TenantDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        // Tenant entity does not extend BaseEntity, so no tenant query filter is applied.
        // This effectively returns all tenants (super-admin query).
        var tenants = await _context.Tenants
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return tenants.Select(t => new TenantDto(
            t.Id,
            t.Name,
            t.Slug,
            t.Email,
            t.Phone,
            t.Address,
            t.City,
            t.Timezone,
            t.LogoUrl,
            t.SubscriptionPlan,
            t.SubscriptionStatus,
            t.TrialEndsAt,
            t.SubscriptionStatus != "inactive",
            t.CreatedAt)).ToList();
    }
}
