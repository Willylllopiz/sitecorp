using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.Authentication;

public sealed class UserRole
{
    private UserRole()
    {
    }

    public UserRole(int userId, int roleId, int? assignedByUserId = null)
    {
        if (userId <= 0)
        {
            throw new DomainException("La relacion usuario-rol necesita un usuario valido.");
        }

        if (roleId <= 0)
        {
            throw new DomainException("La relacion usuario-rol necesita un rol valido.");
        }

        UserId = userId;
        RoleId = roleId;
        AssignedByUserId = assignedByUserId;
        AssignedAt = DateTimeOffset.UtcNow;
    }

    public int UserId { get; private set; }

    public int RoleId { get; private set; }

    public DateTimeOffset AssignedAt { get; private set; }

    public int? AssignedByUserId { get; private set; }
}

