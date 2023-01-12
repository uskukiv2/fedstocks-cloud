using gen.fedstocks.web.Client.Application.Models.Applications;

namespace gen.fedstocks.web.Client.Application.Services;

public interface IApplicationService
{
    Task<ApplicationUser> GetCurrentUserAccountAsync();
    Task<int> GetCurrentUserAccountIdAsync();
}