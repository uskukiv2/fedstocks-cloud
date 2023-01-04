using gen.fed.ui.Models.Applications;

namespace gen.fed.ui.Services;

public interface IApplicationService
{
    Task<ApplicationUser> GetCurrentUserAccountAsync();
}