using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.ValueObjects;

public sealed class FullName
{
    private FullName()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public FullName(string firstName, string lastName)
    {
        FirstName = RequireText(firstName, "El nombre es obligatorio.");
        LastName = RequireText(lastName, "Los apellidos son obligatorios.");
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Value => $"{FirstName} {LastName}";

    public string Initials()
    {
        return $"{FirstName[0]}{LastName[0]}".ToUpperInvariant();
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }
}
