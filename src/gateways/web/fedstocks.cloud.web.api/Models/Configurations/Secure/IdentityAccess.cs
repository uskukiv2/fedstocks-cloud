namespace fedstocks.cloud.web.api.Models.Secure;

public class IdentityAccess
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    [Obsolete("Should be remove, when dev is finished")]
    public string ForcedClientId { get; set; }
}