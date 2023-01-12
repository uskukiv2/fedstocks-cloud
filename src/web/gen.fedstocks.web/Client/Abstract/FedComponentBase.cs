using gen.fedstocks.web.Client.Application.Abstract;
using gen.fedstocks.web.Client.Services;
using Microsoft.AspNetCore.Components;
using ReactiveUI;
using ReactiveUI.Blazor;

namespace gen.fedstocks.web.Client.Abstract
{
    public abstract class FedComponentBase<T> : ReactiveComponentBase<T>, IPageContextMenu where T : BaseViewModel
    {
        [Inject] protected T ViewModel { get; set; }

        [Inject] protected ITopbarItemsService TopbarItemsService { get; set; }

        [Inject] protected NavigationManager Navigation { get; set; }

        public IEnumerable<(string Item, Action Command)> MenuItems { get; private set; }

        protected void UpdateMenu()
        {
            MenuItems = GetMenuItems();

            TopbarItemsService.AddMenuItems(MenuItems.Select(x => x.Item)).WhereNotNull()
                .Subscribe(HandleCommandClicked);
        }

        protected override async Task OnAfterRenderAsync(bool isFirstRender)
        {
            base.ViewModel = ViewModel;
            if (!isFirstRender)
            {
                return;
            }

            await ViewModel.InitAsync();

            UpdateMenu();
            StateHasChanged();
            base.OnAfterRender(isFirstRender);
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
    }
}