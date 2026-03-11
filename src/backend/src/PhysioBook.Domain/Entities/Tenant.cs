namespace PhysioBook.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = "Elbasan";
    public string Timezone { get; set; } = "Europe/Tirane";
    public string? LogoUrl { get; set; }
    public string SubscriptionPlan { get; set; } = "starter";
    public string SubscriptionStatus { get; set; } = "trial";
    public DateTimeOffset? TrialEndsAt { get; set; }
    public string Settings { get; set; } = "{}";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = [];
}
