using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Entities;

public class Unit : IEntity
{
    public int Id { get; set; }
    
    public int TypeNumber { get; set; }
    
    public double Rate { get; set; }
}