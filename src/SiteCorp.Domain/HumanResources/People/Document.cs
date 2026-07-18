using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources.People;

public sealed class Document
{
    private Document()
    {
        DocumentType = string.Empty;
        FileName = string.Empty;
        ContentType = string.Empty;
        ContentBase64 = string.Empty;
    }

    public Document(
        Guid personId,
        string documentType,
        string fileName,
        string contentType,
        string contentBase64)
    {
        if (personId == Guid.Empty)
        {
            throw new DomainException("El documento debe pertenecer a un candidato.");
        }

        Id = Guid.NewGuid();
        PersonId = personId;
        DocumentType = RequireText(documentType, "El tipo de documento es obligatorio.");
        FileName = RequireText(fileName, "El nombre del documento es obligatorio.");
        ContentType = NormalizeContentType(contentType);
        ContentBase64 = RequireBase64(contentBase64);
        UploadDate = DateTimeOffset.UtcNow;
        IsValid = true;
    }

    public Guid Id { get; private set; }

    public Guid PersonId { get; private set; }

    public string DocumentType { get; private set; }

    public string? FilePath { get; private set; }

    public string FileName { get; private set; }

    public string ContentType { get; private set; }

    public string ContentBase64 { get; private set; }

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

    private static string NormalizeContentType(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "application/octet-stream" : value.Trim();
    }

    private static string RequireBase64(string value)
    {
        var normalized = RequireText(value, "El contenido del documento es obligatorio.");

        try
        {
            _ = Convert.FromBase64String(normalized);
        }
        catch (FormatException exception)
        {
            throw new DomainException("El contenido del documento debe estar en base64.", exception);
        }

        return normalized;
    }
}
