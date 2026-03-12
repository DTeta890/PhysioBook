namespace PhysioBook.Domain.Enums;

public static class DocumentCategory
{
    public const string Xray = "xray";
    public const string Mri = "mri";
    public const string Referral = "referral";
    public const string Consent = "consent";
    public const string Insurance = "insurance";
    public const string Other = "other";

    public static readonly string[] All =
    [
        Xray,
        Mri,
        Referral,
        Consent,
        Insurance,
        Other
    ];

    public static bool IsValid(string category) =>
        All.Contains(category, StringComparer.OrdinalIgnoreCase);
}
