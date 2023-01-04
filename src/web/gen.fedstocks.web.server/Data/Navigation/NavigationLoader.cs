using Newtonsoft.Json;
using AppDomain = System.AppDomain;

namespace gen.fedstocks.web.server.Data.Navigation;

public class NavigationLoader
{
    private string _navigationDataFileName = "fed_NavigationData.json";
    private IEnumerable<NavigationModel> _navigationData;


    public async Task<IEnumerable<NavigationModel>> GetNavigation()
    {
        await LoadNavigationData();

        return _navigationData;
    }

    private async Task LoadNavigationData()
    {
        var directory = $"{AppDomain.CurrentDomain.BaseDirectory}\\{_navigationDataFileName}";

        if (!File.Exists(directory))
        {
            throw new FileNotFoundException("Navigation data is not exist. Sorry");
        }

        var content = await File.ReadAllTextAsync(directory);

        _navigationData = JsonConvert.DeserializeObject<IEnumerable<NavigationModel>>(content);
    }
}