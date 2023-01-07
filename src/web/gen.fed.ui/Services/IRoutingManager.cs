namespace gen.fed.application.Services;

public interface IRoutingManager
{
    Type GetRouteType(string url);
}