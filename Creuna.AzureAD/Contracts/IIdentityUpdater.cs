using System.Security.Claims;

namespace EEN.Web.AzureAD
{
    public interface IIdentityUpdater
    {
        void UpdateIdentity(ClaimsIdentity identity);
    }
}