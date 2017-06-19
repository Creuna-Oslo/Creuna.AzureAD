using System.Collections.Generic;

namespace Creuna.AzureAD.Contracts
{
    public interface ICustomVirtualRolesWatcher
    {
        void RolesChanged(RolesChange change);
        void Initialize(List<string> roles);
    }
}