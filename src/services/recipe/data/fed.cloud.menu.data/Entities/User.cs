using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Entities;

public class User : IEntity
{
    public Guid Id { get; set; }
    
    public string AuthenticationId { get; set; }
    
    public bool IsActive { get; set; }
    
    public virtual IEnumerable<Recipe> Recipes { get; set; }
}