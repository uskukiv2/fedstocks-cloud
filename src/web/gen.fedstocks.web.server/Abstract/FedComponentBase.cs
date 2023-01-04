using gen.fed.ui.Abstract;
using gen.fedstocks.web.server.Services;
using Microsoft.AspNetCore.Components;
using ReactiveUI;
using ReactiveUI.Blazor;

namespace gen.fedstocks.web.server.Abstract
{
    public abstract class FedComponentBase<T> : ReactiveComponentBase<T>, IPageContextMenu where T : BaseViewModel
    {

        [Inject]
        protected T ViewModel { get; set; }

        [Inject]
        protected ITopbarItemsService TopbarItemsService { get; set; }

        [Inject]
        protected NavigationManager Navigation { get; set; }

        public IEnumerable<(string Item, Action Command)> MenuItems { get; private set; }

        protected void UpdateMenu()
        {
            MenuItems = GetMenuItems();

            if (MenuItems != null && MenuItems.Any())
            {
                TopbarItemsService.AddMenuItems(MenuItems.Select(x => x.Item)).WhereNotNull().Subscribe(HandleCommandClicked);
            }
        }

        protected override async Task OnAfterRenderAsync(bool isFirstRender)
        {
            base.ViewModel = ViewModel;
            base.OnAfterRender(isFirstRender);
            if (!isFirstRender)
            {
                return;
            }

            await ViewModel.InitAsync();

            UpdateMenu();
        }

        protected virtual void HandleCommandClicked(string itemName)
        {
            var menuItem = GetMenuItemByName(itemName);

            menuItem.executable?.Invoke();
        }

        protected virtual IEnumerable<(string, Action)> GetMenuItems()
        {
            return new (string, Action)[] { };
        }

        protected virtual (string, Action executable) GetMenuItemByName(string name)
        {
            return MenuItems.SingleOrDefault(x => x.Item == name)!;
        }
    }
}
