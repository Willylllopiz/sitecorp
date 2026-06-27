using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.ValueObjects;

public sealed class FullName
{
    private FullName()
    {
        FirstName = string.Empty;
        FirstLastName = string.Empty;
        SecondLastName = string.Empty;
    }

    public FullName(string firstName, string firstLastName, string secondLastName)
    {
        FirstName = RequireText(firstName, "El nombre es obligatorio.");
        FirstLastName = RequireText(firstLastName, "El primer apellido es obligatorio.");
        SecondLastName = RequireText(secondLastName, "El segundo apellido es obligatorio.");
    }

    public string FirstName { get; private set; }

    public string FirstLastName { get; private set; }

    public string SecondLastName { get; private set; }

    public string Value => $"{FirstName} {FirstLastName} {SecondLastName}";

    public string Initials()
    {
        return $"{FirstName[0]}{FirstLastName[0]}".ToUpperInvariant();
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
