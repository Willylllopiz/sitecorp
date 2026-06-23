using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.ValueObjects;

public sealed class Address
{
    private Address()
    {
        Street = string.Empty;
        City = string.Empty;
        Province = string.Empty;
    }

    public Address(string street, string? number, string city, string province, string? postalCode)
    {
        Street = RequireText(street, "La calle o direccion es obligatoria.");
        Number = Normalize(number);
        City = RequireText(city, "La ciudad es obligatoria.");
        Province = RequireText(province, "La provincia es obligatoria.");
        PostalCode = Normalize(postalCode);
    }

    public string Street { get; private set; }

    public string? Number { get; private set; }

    public string City { get; private set; }

    public string Province { get; private set; }

    public string? PostalCode { get; private set; }

    public string FormatAddress()
    {
        var street = string.IsNullOrWhiteSpace(Number) ? Street : $"{Street} #{Number}";
        var postalCode = string.IsNullOrWhiteSpace(PostalCode) ? string.Empty : $" {PostalCode}";

        return $"{street}, {City}, {Province}{postalCode}";
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
