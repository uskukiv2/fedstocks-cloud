using gen.fedstocks.web.Client.Models;
using MudBlazor;

namespace gen.fedstocks.web.Client.Extensions
{
    public static class EnumsExtensions
    {
        public static Severity ToMud(this SnackbarType snackbarType)
        {
            switch (snackbarType)
            {
                case SnackbarType.Default:
                    return Severity.Normal;
                case SnackbarType.Info:
                    return Severity.Info;
                case SnackbarType.Success:
                    return Severity.Success;
                case SnackbarType.Warning:
                    return Severity.Warning;
                case SnackbarType.Error:
                    return Severity.Error;
                default:
                    return Severity.Normal;
            }
        }
    }
}
