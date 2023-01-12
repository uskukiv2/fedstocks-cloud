namespace gen.fedstocks.web.Client.Application.Services;

public interface IRoutingManager
{
    Type GetRouteType(string url);
}