using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.ValueObjects;

public sealed class PhysicalData
{
    private PhysicalData()
    {
        PantsSize = string.Empty;
        ShirtSize = string.Empty;
        ShoeSize = string.Empty;
    }

    public PhysicalData(decimal height, decimal weight, string? pantsSize, string? shirtSize, string? shoeSize)
    {
        if (height < 0)
        {
            throw new DomainException("La estatura no puede ser negativa.");
        }

        if (weight < 0)
        {
            throw new DomainException("El peso no puede ser negativo.");
        }

        Height = height;
        Weight = weight;
        PantsSize = Normalize(pantsSize);
        ShirtSize = Normalize(shirtSize);
        ShoeSize = Normalize(shoeSize);
    }

    public decimal Height { get; private set; }

    public decimal Weight { get; private set; }

    public string? PantsSize { get; private set; }

    public string? ShirtSize { get; private set; }

    public string? ShoeSize { get; private set; }

    public bool IsValid() => Height >= 0 && Weight >= 0;

    public override string ToString()
    {
        return $"{Height:0.##} cm / {Weight:0.##} kg";
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
