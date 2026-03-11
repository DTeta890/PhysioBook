namespace PhysioBook.Application.Tenants.DTOs;

public sealed record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    string Email,
    string? Phone,
    string? Address,
    string City,
    string Timezone,
    string? LogoUrl,
    string SubscriptionPlan,
    string SubscriptionStatus,
    DateTimeOffset? TrialEndsAt,
    bool IsActive,
    DateTimeOffset CreatedAt);
