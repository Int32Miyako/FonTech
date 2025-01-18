using FonTech.Domain.Interfaces;

namespace FonTech.Domain.Entity;

public class UserRole : IEntityId<long>
{
    public long Id { get; set; }
    
    public long UserId { get; set; }
    
    public long RoleId { get; set; }
}