using System.Security.Claims;

namespace Creuna.AzureAD.Contracts
{
    public interface IIdentityUpdater
    {
        void UpdateIdentity(ClaimsIdentity identity);
    }
}