using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.Authentication;

public sealed class User
{
    private User()
    {
    }

    public User(
        int companyId,
        string userName,
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        bool mustChangePassword = false)
    {
        if (companyId <= 0)
        {
            throw new DomainException("El usuario debe pertenecer a una empresa valida.");
        }

        CompanyId = companyId;
        ChangeUserName(userName);
        FirstName = RequireText(firstName, "El nombre del usuario es obligatorio.");
        LastName = RequireText(lastName, "El apellido del usuario es obligatorio.");
        ChangeEmail(email);
        ChangePasswordHash(passwordHash, mustChangePassword);
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }

    public int CompanyId { get; private set; }

    public string UserName { get; private set; } = string.Empty;

    public string NormalizedUserName { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string NormalizedEmail { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string? PhoneNumber { get; private set; }

    public bool IsEmailConfirmed { get; private set; }

    public bool IsPhoneConfirmed { get; private set; }

    public bool IsActive { get; private set; }

    public bool MustChangePassword { get; private set; }

    public int FailedLoginAttempts { get; private set; }

    public DateTimeOffset? LockedUntil { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    public DateTimeOffset? PasswordChangedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public void ChangeUserName(string userName)
    {
        UserName = RequireText(userName, "El nombre de usuario es obligatorio.");
        NormalizedUserName = UserName.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeEmail(string email)
    {
        var cleanEmail = RequireText(email, "El email del usuario es obligatorio.");

        if (!cleanEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("El email del usuario no tiene un formato valido.");
        }

        Email = cleanEmail;
        NormalizedEmail = cleanEmail.ToUpperInvariant();
        IsEmailConfirmed = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangePasswordHash(string passwordHash, bool mustChangePassword)
    {
        PasswordHash = RequireText(passwordHash, "El hash de password es obligatorio.");
        PasswordChangedAt = DateTimeOffset.UtcNow;
        MustChangePassword = mustChangePassword;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangePhoneNumber(string? phoneNumber, bool isConfirmed = false)
    {
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        IsPhoneConfirmed = isConfirmed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        LastLoginAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RecordFailedLogin(int maxAttempts, TimeSpan lockoutDuration)
    {
        if (maxAttempts <= 0)
        {
            throw new DomainException("La cantidad maxima de intentos debe ser mayor que cero.");
        }

        FailedLoginAttempts++;

        if (FailedLoginAttempts >= maxAttempts)
        {
            LockedUntil = DateTimeOffset.UtcNow.Add(lockoutDuration);
        }

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
}
