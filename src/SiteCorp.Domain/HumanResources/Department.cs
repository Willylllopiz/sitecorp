using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.HumanResources;

public sealed class Department
{
    private Department()
    {
        Name = string.Empty;
        ManagerName = string.Empty;
    }

    public Department(string name, string managerName)
    {
        Rename(name);
        ChangeManager(managerName);
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string ManagerName { get; private set; } = string.Empty;

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del departamento es obligatorio.");
        }

        Name = name.Trim();
    }

    public void ChangeManager(string managerName)
    {
        if (string.IsNullOrWhiteSpace(managerName))
        {
            throw new DomainException("El responsable del departamento es obligatorio.");
        }

        ManagerName = managerName.Trim();
    }
}
