namespace gen.fedstocks.web.server.Models;

public class ContextMenuLoaded
{
    public string PageId { get; set; }

    public IEnumerable<MenuItem> MenuItems { get; set; }
}