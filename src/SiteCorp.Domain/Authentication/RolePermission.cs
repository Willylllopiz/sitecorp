using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.Authentication;

public sealed class RolePermission
{
    private RolePermission()
    {
    }

    public RolePermission(int roleId, int permissionId)
    {
        if (roleId <= 0)
        {
            throw new DomainException("La relacion rol-permiso necesita un rol valido.");
        }

        if (permissionId <= 0)
        {
            throw new DomainException("La relacion rol-permiso necesita un permiso valido.");
        }

        RoleId = roleId;
        PermissionId = permissionId;
        AssignedAt = DateTimeOffset.UtcNow;
    }

    public int RoleId { get; private set; }

    public int PermissionId { get; private set; }

    public DateTimeOffset AssignedAt { get; private set; }
}

