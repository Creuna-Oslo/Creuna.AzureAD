using System.Collections.Generic;

namespace EEN.Web.AzureAD
{
    public interface ICustomVirtualRolesWatcher
    {
        void RolesChanged(RolesChange change);
        void Initialize(List<string> roles);
    }
}