using System.Collections.Generic;
using JetBrains.Annotations;

namespace Creuna.AzureAD.Contracts
{
    public interface ICustomVirtualRolesProvider
    {
        [NotNull]
        List<string> GetCustomRoles();

        void Initialize();
    }
}