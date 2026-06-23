using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.ValueObjects;

public sealed class NationalId
{
    private NationalId()
    {
        Number = string.Empty;
    }

    public NationalId(string number)
    {
        var normalized = string.IsNullOrWhiteSpace(number)
            ? throw new DomainException("El identificador nacional es obligatorio.")
            : number.Trim();

        if (normalized.Length < 5)
        {
            throw new DomainException("El identificador nacional debe tener al menos 5 caracteres.");
        }

        Number = normalized;
    }

    public string Number { get; private set; }

    public bool IsValid() => Number.Length >= 5;

    public string Format() => Number;
}
