using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.People;

public sealed class Document
{
    private Document()
    {
        DocumentType = string.Empty;
        FilePath = string.Empty;
    }

    public Document(Guid personId, string documentType, string filePath)
    {
        if (personId == Guid.Empty)
        {
            throw new DomainException("El documento debe pertenecer a una persona.");
        }

        Id = Guid.NewGuid();
        PersonId = personId;
        DocumentType = RequireText(documentType, "El tipo de documento es obligatorio.");
        FilePath = RequireText(filePath, "La ruta del documento es obligatoria.");
        UploadDate = DateTimeOffset.UtcNow;
        IsValid = true;
    }

    public Guid Id { get; private set; }

    public Guid PersonId { get; private set; }

    public string DocumentType { get; private set; }

    public string FilePath { get; private set; }

    public DateTimeOffset UploadDate { get; private set; }

    public bool IsValid { get; private set; }

    public void MarkInvalid() => IsValid = false;

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }
}
