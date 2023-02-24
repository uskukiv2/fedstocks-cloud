using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Entities;

public class Ingredient : IEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int ProductNumber { get; set; }
    
    public int UnitId { get; set; }
    
    public virtual Unit Unit { get; set; }
}