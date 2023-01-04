namespace gen.fedstocks.web.server.Abstract;

public interface IPageContextMenu
{
    IEnumerable<(string Item, Action Command)> MenuItems { get; }
}