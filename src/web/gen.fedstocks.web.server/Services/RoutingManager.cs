using gen.fed.application.Services;

namespace gen.fedstocks.web.server.Services;

public class RoutingManager : IRoutingManager
{
    private readonly IDictionary<string, Type> _routs;

    public RoutingManager(IEnumerable<KeyValuePair<string, Type>> routs)
    {
        _routs = new Dictionary<string, Type>(routs);
    }

    public Type GetRouteType(string url)
    {
        if (_routs.TryGetValue(url, out var routeType))
        {
            return routeType;
        }

        throw new KeyNotFoundException($"Route {url} is not found");
    }
}