using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.Authentication;

public sealed class Company
{
    private Company()
    {
    }

    public Company(string name, string country)
    {
        Name = RequireText(name, "El nombre de la empresa es obligatorio.");
        Country = RequireText(country, "El pais de la empresa es obligatorio.");
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? LegalName { get; private set; }

    public string? TaxId { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Website { get; private set; }

    public string? AddressLine { get; private set; }

    public string? City { get; private set; }

    public string Country { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void UpdateProfile(
        string name,
        string? legalName,
        string? taxId,
        string? email,
        string? phone,
        string? website,
        string? addressLine,
        string? city,
        string country)
    {
        Name = RequireText(name, "El nombre de la empresa es obligatorio.");
        LegalName = CleanOptional(legalName);
        TaxId = CleanOptional(taxId);
        Email = CleanOptional(email);
        Phone = CleanOptional(phone);
        Website = CleanOptional(website);
        AddressLine = CleanOptional(addressLine);
        City = CleanOptional(city);
        Country = RequireText(country, "El pais de la empresa es obligatorio.");
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    private static string? CleanOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

