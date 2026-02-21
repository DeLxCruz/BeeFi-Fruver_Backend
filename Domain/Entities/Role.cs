using Domain.Abstractions;

namespace Domain.Entities;

public class Role : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    private Role() { }

    private Role(Guid id) : base(id) { }

    public static Role Create(string name, string description)
    {
        return new Role(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }
}