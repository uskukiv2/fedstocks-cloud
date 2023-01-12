using System.ComponentModel;
using System.Runtime.CompilerServices;
using gen.fedstocks.web.Client.Services;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace gen.fedstocks.web.Client.Abstract;

public class FedCommandStandaloneBase : ComponentBase, INotifyPropertyChanged, IPageContextMenu
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public IEnumerable<(string Item, Action Command)> MenuItems { get; private set; }
    
    [Inject] protected ITopbarItemsService TopbarItemsService { get; set; }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return Task.CompletedTask;
        }
        
        UpdateMenu();
        StateHasChanged();
        return base.OnAfterRenderAsync(firstRender);
    }

    protected void UpdateMenu()
    {
        MenuItems = GetMenuItems();

        TopbarItemsService.AddMenuItems(MenuItems.Select(x => x.Item)).WhereNotNull()
            .Subscribe(HandleCommandClicked);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    protected virtual void HandleCommandClicked(string itemName)
    {
        var menuItem = GetMenuItemByName(itemName);

        menuItem.executable?.Invoke();
    }

    protected virtual IEnumerable<(string, Action)> GetMenuItems()
    {
        return Array.Empty<(string, Action)>();
    }

    protected virtual (string, Action executable) GetMenuItemByName(string name)
    {
        return MenuItems.SingleOrDefault(x => x.Item == name)!;
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}