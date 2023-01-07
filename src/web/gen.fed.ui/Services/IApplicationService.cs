using gen.fed.application.Models.Applications;

namespace gen.fed.application.Services;

public interface IApplicationService
{
    Task<ApplicationUser> GetCurrentUserAccountAsync();
    Task<int> GetCurrentUserAccountIdAsync();
}