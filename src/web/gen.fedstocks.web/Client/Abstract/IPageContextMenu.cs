namespace gen.fedstocks.web.Client.Abstract;

public interface IPageContextMenu
{
    IEnumerable<(string Item, Action Command)> MenuItems { get; }
}