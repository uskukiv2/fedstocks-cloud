using gen.fed.application.Models;
using gen.fedstocks.web.server.Abstract;

namespace gen.fedstocks.web.server.Services;

public interface ITitleService : IFedService
{
    public string Title { get; }
}

public class TitleService : ITitleService
{
    public string Title { get; private set; }

    private void OnNavigationServiceViewModelChanged(object sender, ViewModelChangedArgs args)
    {
        UpdateTitle(args.LocalizedName);
    }

    private void UpdateTitle(string toChange)
    {
        Title = toChange;
    }
}